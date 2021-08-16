using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using IdentityDemo.Model;
using IdentityDemo.Service.Interface;
using IdentityDemo.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IdentityDemo.Service
{
    public class AppRegistrationService:IAppRegistration
    {

        private readonly DbContext _context;
        private readonly IConfiguration _config;
        private readonly ISecurity _security;


        public AppRegistrationService(DbContext context, IConfiguration config, ISecurity security)
        {
            _context = context;
            _config = config;
            _security = security;
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

        public async Task<ApplicationRegisteration> Register(ApplicationRegisteration model)
        {
            if (await IsAppExist(model.AppName))
            {
                return null;
            }
            var app = new ApplicationRegisteration() {
                AppName = model.AppName,
                ClientId = Guid.NewGuid().ToString(),
                SecretKey = EncryptionDecryption.EncryptText(CreateClientSecretKey(model.AppName)),
                CreatedAt = DateTime.Now,
                ExpireDate = model.ExpireDate!=null?Convert.ToDateTime(model.ExpireDate):null
            };

            await _context.AddAsync<ApplicationRegisteration>(app);
            await _context.SaveChangesAsync();

            return app;
        }

        public async Task<Token> GenerateToken(TokenGeneration model)
        {
            ApplicationRegisteration res = await _context.RegisteredApps.Where(x=>x.AppName==model.AppName).FirstAsync();
            bool isValid = IsDetailValid(model, res);
            if (isValid)
            {
                Token token = _security.GenerateToken(model.AppName, model.ClientId, "App");
                return token;
            }
            return null ;
        }

        private bool IsDetailValid(TokenGeneration currentModel, ApplicationRegisteration storedModel)
        {
            if (currentModel.ClientId == storedModel.ClientId && EncryptionDecryption.DecryptText(currentModel.ClientSecret) == EncryptionDecryption.DecryptText(storedModel.SecretKey))
            {
                if (storedModel.ExpireDate!=null)
                {
                    DateTime dt1 = DateTime.Parse(storedModel.ExpireDate.ToString());
                    DateTime dt2 = DateTime.Now;
                    return dt1.Date <= dt2.Date;
                }
                return true;
            }
            return false;
              
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


// expire date for secret key
// option (never expire/ expire date) if expire date not set then it would be consider never expire
// key encryption puplic and private key
// token save
// redis cache (key value pair) quick search