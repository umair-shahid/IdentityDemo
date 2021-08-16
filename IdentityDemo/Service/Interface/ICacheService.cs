using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace IdentityDemo.Service.Interface
{
    public interface ICacheService
    { 
        Task<T> GetFromCache<T>(string key) where T : class;
        Task SetCache<T>(string key, T value) where T : class;
        Task ClearCache(string key);
    }
}
