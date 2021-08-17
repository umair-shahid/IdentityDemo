using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityDemo.Model;
using IdentityDemo.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdentityDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VerificationController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly IRSAEncryptionService _rsa;

        public VerificationController(ICacheService cacheService,IRSAEncryptionService rsa)
        {
            _cacheService = cacheService;
            _rsa = rsa;

        }
        [HttpGet]
        [AllowAnonymous]
        public Response Get()
        {
            return new Response() { Success = true, Message = "Verification controller", Data = null };
        }
        [HttpGet("PublicKey")]
        public IActionResult PublicKey()
        {
            try
            {
                return Ok(new Response { Data = _rsa.GetPublicKey(), Message = "Public key fetched successfully", Status = "Success", Success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Something went wrong", Data = ex.Message });
            }
        }

        [HttpGet("Verify")]
        [Authorize]
        public async Task<IActionResult> Verify()
        {
            try
            {
                var context = HttpContext.User;
                var identity = context.Identity as ClaimsIdentity;
                if (identity != null && context.HasClaim(x => x.Value == "App"))
                {
                    IEnumerable<Claim> claim = identity.Claims;
                    var name = claim.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
                    var localStore = await _cacheService.GetFromCache<Token>(name);
                    if (localStore != null)
                    {
                        var tokenString = HttpContext.Request.Headers["Authorization"][0];
                        var jwtEncodedString = tokenString.Substring(7); 
                        var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
                        
                        var RequestToken = GetTokenDetail(null, true, token);
                        TokenDetail LocalSaved = GetTokenDetail(localStore.AccessToken,false,null);
                        if (VerifyDetail(RequestToken, LocalSaved))
                        {
                            return Ok(new Response { Data = "", Message = "Verified successfully", Status = "Success", Success = true });
                        }
                        return StatusCode(StatusCodes.Status401Unauthorized, new Response { Status = "false", Message = "Not valid detail!" });
                    }
                    return StatusCode(StatusCodes.Status401Unauthorized, new Response { Status = "false", Message = "Token expired, re-generate it" });
                }
                return StatusCode(StatusCodes.Status401Unauthorized, new Response { Status = "false", Message = "Not allowed to access this end point" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Success = false, Message = "Something went wrong", Data = ex.Message });
            }
        }
        #region Private Methods

        private bool VerifyDetail(TokenDetail requestToken, TokenDetail savedToken)
        {

            return (
                requestToken.Jti.Equals(savedToken.Jti) &&
                requestToken.Sub.Equals(savedToken.Sub) &&
                requestToken.CHash.Equals(savedToken.CHash)
                );
        }

        private TokenDetail GetTokenDetail(object stream, bool isBearer = false, JwtSecurityToken token = null)
        {
            if (isBearer)
            {

                return new TokenDetail
                {
                    Jti = token.Claims.First(c => c.Type == "jti").Value,
                    Sub = token.Claims.First(c => c.Type == "sub").Value,
                    // CHash = EncryptionDecryption.DecryptText(token.Claims.First(c => c.Type == "c_hash").Value)
                    CHash = _rsa.Decrypt(token.Claims.First(c => c.Type == "c_hash").Value)
                };
            }
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream.ToString());
            JwtSecurityToken tokenS = jsonToken as JwtSecurityToken;

            return new TokenDetail
            {
                Jti = tokenS.Claims.First(claim => claim.Type == "jti").Value,
                Sub = tokenS.Claims.First(claim => claim.Type == "sub").Value,
               // CHash = EncryptionDecryption.DecryptText(tokenS.Claims.First(claim => claim.Type == "c_hash").Value)
                CHash = _rsa.Decrypt(tokenS.Claims.First(claim => claim.Type == "c_hash").Value)
            };
        }
        #endregion
    }
}
