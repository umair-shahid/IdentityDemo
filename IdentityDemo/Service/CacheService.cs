using System;
using IdentityDemo.Service.Interface;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace IdentityDemo.Service
{
    public class CacheService: ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _config;

        public CacheService(IDistributedCache cache, IConfiguration config)
        {
            _cache = cache;
            _config = config;
        }

        public async Task<T> GetFromCache<T>(string key) where T : class
        {
            var cachedResponse = await _cache.GetStringAsync(key);
            return cachedResponse == null ? null : JsonSerializer.Deserialize<T>(cachedResponse);
        }

        public async Task SetCache<T>(string key, T value) where T : class
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(Convert.ToInt32(_config["Redis:AbsoluteExpirationRelativeToNowInHours"])),
                // SlidingExpiration = TimeSpan.FromHours(3)
            };
            var response = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, response, options);
        }

        public async Task ClearCache(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
