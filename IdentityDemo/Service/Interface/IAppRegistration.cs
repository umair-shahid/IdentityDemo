using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityDemo.Model;

namespace IdentityDemo.Service.Interface
{
    public interface IAppRegistration
    {
        Task<List<ApplicationRegisteration>> GetRegisteredApps();
        Task<bool> IsAppExist(string name);
        Task<ApplicationRegisteration> RegisterAppAndGetKey(ApplicationRegisteration model);
        Task<Token> GenerateToken(TokenGeneration model);
    }
}
