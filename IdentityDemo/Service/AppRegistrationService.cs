using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using IdentityDemo.Model;
using IdentityDemo.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace IdentityDemo.Service
{
    public class AppRegistrationService:IAppRegistrationService
    {

        private readonly DbContext _context;
        private readonly IConfiguration _config;
        private readonly ISecurityService _security;
        private readonly ICacheService _cache;
        private readonly IRSAEncryptionService _rsa;


        public AppRegistrationService(
            DbContext context,
            IConfiguration config,
            ISecurityService security,
            ICacheService cache,
            IRSAEncryptionService rsa)
        {
            _context = context;
            _config = config;
            _security = security;
            _cache = cache;
            _rsa = rsa;
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

        public async Task<ApplicationRegisteration> RegisterAppAndGetKey(ApplicationRegisteration model)
        {
            if (await IsAppExist(model.AppName))
            {
                return null;
            }
            var app = new ApplicationRegisteration() {
                AppName = model.AppName,
                ClientId = Guid.NewGuid().ToString(),
                // SecretKey = EncryptionDecryption.EncryptText(CreateClientSecretKey(model.AppName)),
                SecretKey = _rsa.Encrypt(CreateClientSecretKey(model.AppName)),
                CreatedAt = DateTime.Now,
                ExpireDate = model.ExpireDate!=null?Convert.ToDateTime(model.ExpireDate):null
            };

            await _context.AddAsync<ApplicationRegisteration>(app);
            await _context.SaveChangesAsync();
            return app;
        }

        public async Task<ApplicationRegisteration> UpdateSecretKey(ApplicationRegisteration model)
        {
            var data =  await _context.RegisteredApps.SingleOrDefaultAsync(x => x.AppName == model.AppName && x.ClientId==model.ClientId);
            if (data!=null)
            {
                data.SecretKey = _rsa.Encrypt(CreateClientSecretKey(model.AppName));
                data.CreatedAt = DateTime.Now;
                data.ExpireDate = model.ExpireDate != null ? Convert.ToDateTime(model.ExpireDate) : null;
                await _context.SaveChangesAsync();
                return data;
            }
            return null;

        }

        public async Task<Token> GenerateToken(TokenGeneration model)
        {
            ApplicationRegisteration res = await _context.RegisteredApps.Where(x=>x.AppName==model.AppName).FirstAsync();
            bool isValid = IsDetailValid(model, res);
            if (isValid)
            {
                Token token = _security.GenerateToken(model.AppName, model.ClientId, "App", res.SecretKey);
               
                await _cache.SetCache<Token>(res.AppName, token);
                return token;
            }
            return null ;
        }


        #region Private Methods
        private bool IsDetailValid(TokenGeneration currentModel, ApplicationRegisteration storedModel)
        {
           // if (currentModel.ClientId == storedModel.ClientId && EncryptionDecryption.DecryptText(currentModel.ClientSecret) == EncryptionDecryption.DecryptText(storedModel.SecretKey))
            if (currentModel.ClientId == storedModel.ClientId && _rsa.Decrypt(currentModel.ClientSecret) == _rsa.Decrypt(storedModel.SecretKey))
            {
                if (storedModel.ExpireDate!=null)
                {
                    DateTime dt1 = DateTime.Parse(storedModel.ExpireDate.ToString());
                    DateTime dt2 = DateTime.Now;
                    return dt1.Date >= dt2.Date;
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
        #endregion
    }
}