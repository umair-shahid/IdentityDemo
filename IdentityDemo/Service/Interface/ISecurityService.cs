using System;
using IdentityDemo.Model;

namespace IdentityDemo.Service.Interface
{
    public interface ISecurityService
    {
        Token GenerateToken(string name, string id, string role, string hashKey = null);
    }
}
