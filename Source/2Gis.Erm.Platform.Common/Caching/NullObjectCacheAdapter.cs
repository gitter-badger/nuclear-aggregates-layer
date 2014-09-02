using System;

namespace DoubleGis.Erm.Platform.Common.Caching
{
    public class NullObjectCacheAdapter : ICacheAdapter
    {
        public void Add<T>(string key, T value, DateTime absoluteExpiration)
        {
        }

        public void Add<T>(string key, T value, TimeSpan slidingExpiration)
        {
        }

        public void Add<T>(string key, T value)
        {
        }

        public T Get<T>(string key)
        {
            return default(T);
        }
        
        public bool Contains(string key)
        {
            return false;
        }

        public void Remove(string key)
        {
        }

        public void ClearCache()
        {
        }
    }
}
