using System;
using System.Collections;
using System.Linq;
using System.Runtime.Caching;

namespace DoubleGis.Erm.Platform.Common.Caching
{
    public class MemCacheAdapter: ICacheAdapter
    {
        private readonly MemoryCache _cache;
        public MemCacheAdapter()
        {
            _cache = MemoryCache.Default;
        }
        public void Add(string key, object value, DateTime absoluteExpiration)
        {
            if (value != null)
            {
                var policy = new CacheItemPolicy
                                 {
                                     AbsoluteExpiration = absoluteExpiration
                                 };

                _cache.Add(key, value, policy);
            }

        }
        public void Add(string key, object value, TimeSpan slidingExpiration)
        {
            if (value != null)
            {
                var policy = new CacheItemPolicy
                                 {
                                     SlidingExpiration = slidingExpiration
                                 };
                _cache.Add(key, value, policy);
            }

        }
        public void Add(string key, object value)
        {
            if (value != null)
            {
                var policy = new CacheItemPolicy();
                _cache.Add(key, value, policy);
            }
        }
        public object Get(string key)
        {
            return _cache.Get(key);
        }
        public T Get<T>(string key) where T : class
        {
            return _cache.Get(key) as T;
        }
        public object this[string key]
        {
            get { return _cache.Get(key); }
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
