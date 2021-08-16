using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using IdentityDemo.Model;

namespace IdentityDemo.Utils
{
    public static class Helper
    {
        public static bool VerifyDetail(TokenDetail requestToken, TokenDetail savedToken)
        {

            return (
                requestToken.Jti.Equals(savedToken.Jti) &&
                requestToken.Sub.Equals(savedToken.Sub) &&
                requestToken.CHash.Equals(savedToken.CHash)
                );
        }

        public static TokenDetail GetTokenDetail(object stream, bool isBearer = false, JwtSecurityToken token = null)
        {
            if (isBearer)
            {

                return new TokenDetail
                {
                    Jti = token.Claims.First(c => c.Type == "jti").Value,
                    Sub = token.Claims.First(c => c.Type == "sub").Value,
                    CHash = EncryptionDecryption.DecryptText(token.Claims.First(c => c.Type == "c_hash").Value)
                };
            }
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(stream.ToString());
            JwtSecurityToken tokenS = jsonToken as JwtSecurityToken;

            return new TokenDetail
            {
                Jti = tokenS.Claims.First(claim => claim.Type == "jti").Value,
                Sub = tokenS.Claims.First(claim => claim.Type == "sub").Value,
                CHash = EncryptionDecryption.DecryptText(tokenS.Claims.First(claim => claim.Type == "c_hash").Value)
            };
        }
    }
}
