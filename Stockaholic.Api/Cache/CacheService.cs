
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Stockaholic.API.Cache
{
    public class CacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CacheService> _logger;

        public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T?> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiry = null)
        {
            var cached = await _cache.GetStringAsync(key);
            if (cached != null)
            {
                _logger.LogInformation("Using cached value for key={}", key);
                return JsonSerializer.Deserialize<T>(cached);
            }

            var value = await factory();
            if (value == null) return default;

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(5)
            };

            await _cache.SetStringAsync(key, JsonSerializer.Serialize(value), options);
            return value;
        }
        public void Remove(string key)=> _cache.Remove(key);    
    }
}