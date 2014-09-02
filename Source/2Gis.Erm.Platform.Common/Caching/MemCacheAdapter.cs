using System;
using System.Collections;
using System.Linq;
using System.Runtime.Caching;

namespace DoubleGis.Erm.Platform.Common.Caching
{
    public class MemCacheAdapter : ICacheAdapter
    {
        private readonly MemoryCache _cache;

        public MemCacheAdapter()
        {
            _cache = MemoryCache.Default;
        }

        public void Add<T>(string key, T value, DateTime absoluteExpiration)
        {
            if (!value.Equals(default(T)))
            {
                var policy = new CacheItemPolicy
                                 {
                                     AbsoluteExpiration = absoluteExpiration
                                 };

                _cache.Add(key, value, policy);
            }
        }

        public void Add<T>(string key, T value, TimeSpan slidingExpiration)
        {
            if (!value.Equals(default(T)))
            {
                var policy = new CacheItemPolicy
                                 {
                                     SlidingExpiration = slidingExpiration
                                 };
                _cache.Add(key, value, policy);
            }
        }

        public void Add<T>(string key, T value)
        {
            if (!value.Equals(default(T)))
            {
                var policy = new CacheItemPolicy();
                _cache.Add(key, value, policy);
            }
        }

        public T Get<T>(string key)
        {
            return (T)_cache.Get(key);
        }
        
        public bool Contains(string key)
        {
            return _cache.Get(key) != null;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void ClearCache()
        {
            var keys = _cache.Cast<IDictionaryEnumerator>().Select(x => x.Key.ToString());
            foreach (var key in keys)
            {
                _cache.Remove(key);
            }
        }
    }
}
