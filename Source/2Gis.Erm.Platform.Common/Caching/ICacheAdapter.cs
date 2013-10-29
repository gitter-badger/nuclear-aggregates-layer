using System;

namespace DoubleGis.Erm.Platform.Common.Caching
{
    public interface ICacheAdapter
    {
        void Add(string key, object value, DateTime absoluteExpiration);
        void Add(string key, object value, TimeSpan slidingExpiration);
        void Add(string key, object value);

        object Get(string key);
        T Get<T>(string key) where T : class;
        
        object this[string key] { get; }

        bool Contains(string key);

        void Remove(string key);
        void ClearCache();
    }
}
