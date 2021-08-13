using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IdentityDemo.Model;
using IdentityDemo.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace IdentityDemo.Service
{
    public class AppRegistrationService
    {

        private readonly DbContext _context;
        private readonly IConfiguration _config;
        public AppRegistrationService(DbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }
        public async Task<List<ApplicationRegisteration>> GetRegisteredApps()
        {
            var res = await _context.RegisteredApps.Select(x=>x).ToListAsync();
            return res;
        }
        public async Task<bool> IsAppExist(string name)
        {
            var res = await _context.RegisteredApps.Where(x => x.AppName == name).ToListAsync();
            return res.Any();
        }
        public async Task<ApplicationRegistrationResponse> Register(ApplicationRegisteration model)
        {
            if (await IsAppExist(model.AppName))
            {
                return new ApplicationRegistrationResponse() { Status=false,AppName = model.AppName, ClientId="",SecretKey="", };
            }
            var app = new ApplicationRegisteration() {
                AppName = model.AppName,
                ClientId = Guid.NewGuid().ToString(),
                 SecretKey = EncryptionDecryption.EncryptText(CreateClientSecretKey(model.AppName)),
                CreatedAt = DateTime.Now };

            await _context.AddAsync<ApplicationRegisteration>(app);
            await _context.SaveChangesAsync();

            return new ApplicationRegistrationResponse() {
                Status=true ,
                AppName = app.AppName,
                ClientId = app.ClientId,
                SecretKey = app.SecretKey,
                CreatedAt = app.CreatedAt
            };
           
        }
        public async Task<Token> GenerateToken(TokenGeneration model)
        {
            ApplicationRegisteration res = await _context.RegisteredApps.Where(x=>x.AppName==model.AppName).FirstAsync();
            bool isValid = IsDetailValid(model, res);
            if (isValid)
            {
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _config["JWT:ValidIssuer"],
                    audience: _config["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );
                return new Token { AccessToken = new JwtSecurityTokenHandler().WriteToken(token), Expire= token.ValidTo };
            }
            return null ;
        }

        private bool IsDetailValid(TokenGeneration currentModel, ApplicationRegisteration storedModel)
        {
            return (currentModel.ClientId == storedModel.ClientId && EncryptionDecryption.DecryptText(currentModel.ClientSecret) == EncryptionDecryption.DecryptText(storedModel.SecretKey));
            
        }
        private string CreateClientSecretKey(string name)
        {
            using (var cryptoProvider = new RNGCryptoServiceProvider())
            {
                byte[] secretKeyByteArray = new byte[32]; //256 bit
                cryptoProvider.GetBytes(secretKeyByteArray);
               return Convert.ToBase64String(secretKeyByteArray);
            }
          
        }
    }
}
