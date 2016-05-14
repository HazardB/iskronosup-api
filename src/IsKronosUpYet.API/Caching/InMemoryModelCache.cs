using System;
using Microsoft.Extensions.Caching.Memory;

namespace IsKronosUpYet.API.Caching
{
    /// <summary>
    /// Singleton - InMemoryModelCache - should last for as long as the app pool does.
    /// NOTE: turn on azure's "always-on" functionality if you want this to last as long as it can. (that said, there's no guarantee this will stick around for a long time)
    /// </summary>
    public sealed class InMemoryModelCache
    {
        private static readonly int CacheExpirationSeconds = 30;

        private static InMemoryModelCache _instance;

        private object gate = new object();

        public static InMemoryModelCache Instance => _instance ?? (_instance = new InMemoryModelCache());

        private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions() { ExpirationScanFrequency = TimeSpan.FromSeconds(15) });

        public object Retrieve(string key)
        {
            lock (gate)
            {
                return CacheExtensions.Get(_cache, key);
            }
        }

        public void Save(string key, object value)
        {
            lock (gate)
            {
                _cache.Set(key, value, new MemoryCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(CacheExpirationSeconds) });
            }
        }

        public void Remove(string key)
        {
            lock (gate)
            {
                object value;
                if (_cache.TryGetValue(key, out value))
                {
                    _cache.Remove(key);
                }
            }
        }
    }
}