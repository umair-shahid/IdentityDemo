using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityDemo.Model;
using IdentityDemo.Service.Interface;
using IdentityDemo.Utils;
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

        public VerificationController(ICacheService cacheService)
        {
            _cacheService = cacheService;

        }
        [HttpGet]
        [AllowAnonymous]
        public Response Get()
        {
            return new Response() { Success = true, Message = "Verification controller", Data = null };
        }

        [HttpGet("Verify")]
        [Authorize]
        public async Task<IActionResult> Verify()
        {
            try
            {
                var httpContext = HttpContext;
                var context = HttpContext.User;
                var identity = context.Identity as ClaimsIdentity;
                if (identity != null && context.HasClaim(x => x.Value == "App"))
                {
                    IEnumerable<Claim> claim = identity.Claims;
                    var name = claim.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault().Value;
                    var localStore = await _cacheService.GetFromCache<Token>(name);
                    if (localStore != null)
                    {
                        var tokenString = httpContext.Request.Headers["Authorization"][0];
                        var jwtEncodedString = tokenString.Substring(7); 
                        var token = new JwtSecurityToken(jwtEncodedString: jwtEncodedString);
                        
                        var RequestToken = Helper.GetTokenDetail(null, true, token);
                        TokenDetail LocalSaved = Helper.GetTokenDetail(localStore.AccessToken,false,null);
                        if (Helper.VerifyDetail(RequestToken, LocalSaved))
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
        
    }
}
