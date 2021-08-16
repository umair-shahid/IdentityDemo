using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityDemo.Model;
using IdentityDemo.Service.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IdentityDemo.Service
{
    public class SecurityService: ISecurity
    {
        private readonly IConfiguration _config;
        public SecurityService(IConfiguration config)
        {
            _config = config;
        }
        public Token GenerateToken(string name, string id, string role)
        {

            var myClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, name),
                    new Claim(JwtRegisteredClaimNames.Sub, id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, role),

                };
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _config["JWT:ValidIssuer"],
                audience: _config["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddHours(Convert.ToDouble(_config["JWT:AccessTokenExpirationHour"])),
                claims: myClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );
            return new Token { AccessToken = new JwtSecurityTokenHandler().WriteToken(token), Expire = token.ValidTo };
        }
    }
}
