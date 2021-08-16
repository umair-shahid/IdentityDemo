using System;
using IdentityDemo.Service.Interface;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Threading.Tasks;

namespace IdentityDemo.Service
{
    public class CacheService: ICacheService
    {
        private readonly IDistributedCache _cache;

        public CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        //public T Get<T>(string key)
        //{
        //    var value = _cache.GetString(key);

        //    if (value != null)
        //    {
        //        return JsonSerializer.Deserialize<T>(value);
        //    }

        //    return default;
        //}

        //public T Set<T>(string key, T value)
        //{
        //    //var options = new DistributedCacheEntryOptions
        //    //{
        //    //    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
        //    //    SlidingExpiration = TimeSpan.FromDays(10)
        //    //};

        //    _cache.SetString(key, JsonSerializer.Serialize(value));

        //    return value;
        //}
        public async Task<T> GetFromCache<T>(string key) where T : class
        {
            var cachedResponse = await _cache.GetStringAsync(key);
            return cachedResponse == null ? null : JsonSerializer.Deserialize<T>(cachedResponse);
        }

        public async Task SetCache<T>(string key, T value, DistributedCacheEntryOptions options) where T : class
        {
            var response = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, response, options);
        }

        public async Task ClearCache(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
