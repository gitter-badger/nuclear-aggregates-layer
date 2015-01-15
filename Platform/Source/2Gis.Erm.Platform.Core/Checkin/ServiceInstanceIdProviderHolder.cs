using System;
using System.Threading;

using DoubleGis.Erm.Platform.API.Core.Checkin;

namespace DoubleGis.Erm.Platform.Core.Checkin
{    
    /// <summary>
    /// Класс нужен для доступа к instance IServiceInstanceIdProvider, закрепленному за текущим открытым ServiceHost.
    /// Предполагается, что для одной ServiceHostFactory в отдельно взятый момент времени может быть только один открытый ServiceHost;
    /// Если это не так, можно хранить множество IServiceInstanceIdProvider с соответствующими ServiceHost, и при вызове 
    /// декорируемых методов указывать текущий ServiceHost из OperationContext
    /// </summary>
    public class ServiceInstanceIdProviderHolder : IServiceInstanceIdProviderHolder, IServiceInstanceIdProvider
    {
        private readonly object _sync = new object();
        private readonly ManualResetEventSlim _idProviderSetSignal = new ManualResetEventSlim(false);
        private IServiceInstanceIdProvider _currentServiceInstanceIdProvider;

        // тут и в TryGetInstanceId на самом деле можно проторчать 2 * timeout + время ожидания блокировки, что не совсем честно по отношению к вызывающему коду
        public Guid GetInstanceId(TimeSpan timeout)
        {
            _idProviderSetSignal.Wait(timeout);
            lock (_sync)
            {
                if (_currentServiceInstanceIdProvider == null)
                {
                    throw new InvalidOperationException("Current service instance id provider is not set");
                }

                return _currentServiceInstanceIdProvider.GetInstanceId(timeout);
            }
        }

        public bool TryGetInstanceId(TimeSpan timeout, out Guid id)
        {
            _idProviderSetSignal.Wait(timeout);
            lock (_sync)
            {
                if (_currentServiceInstanceIdProvider == null)
                {
                    throw new InvalidOperationException("Current service instance id provider is not set");
                }

                return _currentServiceInstanceIdProvider.TryGetInstanceId(timeout, out id);
            }
        }

        public void SetProvider(IServiceInstanceIdProvider serviceInstanceIdProvider)
        {
            lock (_sync)
            {
                _currentServiceInstanceIdProvider = serviceInstanceIdProvider;
                _idProviderSetSignal.Set();
            }
        }

        public void RemoveProvider(IServiceInstanceIdProvider serviceInstanceIdProvider)
        {
            lock (_sync)
            {
                if (ReferenceEquals(_currentServiceInstanceIdProvider, serviceInstanceIdProvider))
                {
                    _currentServiceInstanceIdProvider = null;
                    _idProviderSetSignal.Reset();
                }
            }
        }
    }
}