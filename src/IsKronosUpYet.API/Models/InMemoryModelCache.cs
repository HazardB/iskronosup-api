using Microsoft.Extensions.Caching.Memory;
using System;

namespace IsKronosUpYet.API.Models
{
    public sealed class InMemoryModelCache
    {
        private static readonly int CacheExpirationSeconds = 30;

        private static InMemoryModelCache _instance;

        public static InMemoryModelCache Instance
        {
            get { return _instance ?? (_instance = new InMemoryModelCache()); }
        }

        private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions() { ExpirationScanFrequency = TimeSpan.FromSeconds(15) });

        public object Retrieve(string key)
        {
            return _cache.Get(key);
        }

        public void Save(string key, object value)
        {
            _cache.Set(key, value, new MemoryCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(CacheExpirationSeconds) });
        }
    }
}