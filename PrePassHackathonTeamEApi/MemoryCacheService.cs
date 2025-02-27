using Microsoft.Extensions.Caching.Memory;

namespace PrePassHackathonTeamEApi
{
    public class MemoryCacheService
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheService()
        {
            _cache = new MemoryCache(new MemoryCacheOptions()); // Singleton instance
        }

        // Generic method to get or create cache
        public T GetOrCreate<T>(string key, Func<T> createItem, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
        {
            if (_cache.TryGetValue(key, out T value))
            {
                return value;
            }

            value = createItem(); // Create item if not in cache
            Set(key, value, absoluteExpiration, slidingExpiration); // Store in cache
            return value;
        }

        // Explicitly set cache value
        public void Set<T>(string key, T value, TimeSpan? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
        {
            var cacheOptions = new MemoryCacheEntryOptions();
             cacheOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
            //cacheOptions.SetAbsoluteExpiration(DateTimeOffset.UtcNow.AddHours(10)); // Cache expires in 10 min
            //.SetSlidingExpiration(TimeSpan.FromMinutes(2));   //

            if (absoluteExpiration.HasValue)
                cacheOptions.SetAbsoluteExpiration(absoluteExpiration.Value);

            if (slidingExpiration.HasValue)
                cacheOptions.SetSlidingExpiration(slidingExpiration.Value);

            _cache.Set(key, value, cacheOptions);
        }

        // Retrieve without creating if key exists
        public bool TryGet<T>(string key, out T value)
        {
            return _cache.TryGetValue(key, out value);
        }

        // Remove cache entry
        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
