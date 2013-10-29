using System;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Caching;

using DoubleGis.Erm.Platform.Common.Caching;

namespace DoubleGis.Erm.Platform.Web.Mvc.Caching
{
    public class WebCacheAdapter : ICacheAdapter
    {
        private readonly Cache _cache;
        public WebCacheAdapter()
        {
            if (HttpContext.Current == null)
            {
                throw new InvalidOperationException("Not in a web context, unable to use the web cache.");
            }

            _cache = HttpRuntime.Cache;
        }

        public object this[string key]
        {
            get { return _cache.Get(key); }
        }

        public void Add(string key, object value, DateTime absoluteExpiration)
        {
            if (value != null)
            {
                _cache.Add(key, value, null, absoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
            }
        }

        public void Add(string key, object value, TimeSpan slidingExpiration)
        {
            if (value != null)
            {
                _cache.Add(key, value, null, Cache.NoAbsoluteExpiration, slidingExpiration, CacheItemPriority.Normal, null);
            }
        }

        public void Add(string key, object value)
        {
            if (value != null)
            {
                _cache.Add(key, value, null, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
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