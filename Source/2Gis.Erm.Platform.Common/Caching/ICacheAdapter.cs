using System;

namespace DoubleGis.Erm.Platform.Common.Caching
{
    public interface ICacheAdapter
    {
        T Get<T>(string key);
        void Add<T>(string key, T value, DateTime absoluteExpiration);
        void Add<T>(string key, T value, TimeSpan slidingExpiration);
        void Add<T>(string key, T value);
        bool Contains(string key);
        void Remove(string key);
        void ClearCache();
    }
}
