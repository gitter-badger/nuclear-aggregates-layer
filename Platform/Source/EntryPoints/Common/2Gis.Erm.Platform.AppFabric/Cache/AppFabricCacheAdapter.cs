using System;

using DoubleGis.Erm.Platform.Common.Caching;
using DoubleGis.Erm.Platform.Common.Logging;

using Microsoft.ApplicationServer.Caching;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace DoubleGis.Erm.Platform.AppFabric.Cache
{
    public class AppFabricCacheAdapter : ICacheAdapter, IDisposable
    {
        private readonly ICommonLog _logger;
        private readonly string _cacheName;
        private readonly DataCacheFactory _dataCacheFactory = new DataCacheFactory();
        private readonly RetryPolicy<CacheTransientErrorDetectionStrategy> _retryPolicy =
            new RetryPolicy<CacheTransientErrorDetectionStrategy>(3, TimeSpan.FromSeconds(1));
        
        public AppFabricCacheAdapter(ICommonLog logger, string cacheName)
        {
            _logger = logger;
            _cacheName = cacheName;
            _retryPolicy.Retrying += OnRetrying;
        }

        private DataCache Cache
        {
            get { return _retryPolicy.ExecuteAction(() => _dataCacheFactory.GetCache(_cacheName)); }
        }

        public T Get<T>(string key)
        {
            return _retryPolicy.ExecuteAction(() => (T)Cache.Get(key));
        }

        public void Add<T>(string key, T value, DateTime absoluteExpiration)
        {
            if (!value.Equals(default(T)))
            {
                _retryPolicy.ExecuteAction(() => Cache.Put(key, value, absoluteExpiration.Subtract(DateTime.UtcNow)));
            }
        }

        public void Add<T>(string key, T value, TimeSpan slidingExpiration)
        {
            if (!value.Equals(default(T)))
            {
                _retryPolicy.ExecuteAction(() => Cache.Put(key, value, slidingExpiration));
            }
        }

        public void Add<T>(string key, T value)
        {
            if (!value.Equals(default(T)))
            {
                _retryPolicy.ExecuteAction(() => Cache.Put(key, value));
            }
        }

        public bool Contains(string key)
        {
            return _retryPolicy.ExecuteAction(() => Cache.Get(key) != null);
        }

        public void Remove(string key)
        {
            _retryPolicy.ExecuteAction(() => Cache.Remove(key));
        }

        public void ClearCache()
        {
            throw new NotSupportedException("Cache clearing is not supported in AppFabric Cache");
        }

        public void Dispose()
        {
            _retryPolicy.Retrying -= OnRetrying;
            _dataCacheFactory.Dispose();
        }

        private void OnRetrying(object sender, RetryingEventArgs args)
        {
            _logger.WarnFormat("Retrying to execute action within AppFabric Cache adapter. " +
                                 "Current retry count = {0}, last exception: {1}",
                                 args.CurrentRetryCount,
                                 args.LastException.ToString());
        }
    }
}