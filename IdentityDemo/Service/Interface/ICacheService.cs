using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace IdentityDemo.Service.Interface
{
    public interface ICacheService
    {
        //T Get<T>(string key);
        //T Set<T>(string key, T value);

        Task<T> GetFromCache<T>(string key) where T : class;
        Task SetCache<T>(string key, T value, DistributedCacheEntryOptions options) where T : class;
        Task ClearCache(string key);
    }
}
