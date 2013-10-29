using System;

namespace DoubleGis.Erm.Platform.Common.Caching
{
    public class NullObjectCacheAdapter : ICacheAdapter
    {
        public void Add(string key, object value, DateTime absoluteExpiration){}
        public void Add(string key, object value, TimeSpan slidingExpiration){}
        public void Add(string key, object value){}
        public object Get(string key)
        {
            return null;
        }
        public T Get<T>(string key) where T : class
        {
            return null;
        }
        public object this[string key]
        {
            get { return null; }
        }
        public bool Contains(string key)
        {
            return false;
        }
        public void Remove(string key){}
        public void ClearCache(){}
    }
}
