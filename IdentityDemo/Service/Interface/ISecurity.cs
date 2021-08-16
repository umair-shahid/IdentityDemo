using System;
using IdentityDemo.Model;

namespace IdentityDemo.Service.Interface
{
    public interface ISecurity
    {
        Token GenerateToken(string name, string id, string role);
    }
}
