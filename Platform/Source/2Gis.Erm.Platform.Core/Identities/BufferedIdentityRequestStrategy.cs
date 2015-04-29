using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.ServiceModel;
using System.Threading;

using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Metadata;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.Platform.Core.Identities
{
    public sealed class BufferedIdentityRequestStrategy : IIdentityRequestStrategy
    {
        // Максимальное количество идентификаторов, которое можно запросить у сервиса генерации идентификаторов
        private const int MaxRequestedCount = 32767;

        private readonly IClientProxyFactory _clientProxyFactory;
        private readonly IAPIIdentityServiceSettings _identityServiceSettings;
        private readonly ITracer _tracer;

        private readonly ConcurrentQueue<long> _idBuffer = new ConcurrentQueue<long>();
        private int _nextRequestedCount = 1;
        private int _threadsCount;

        public BufferedIdentityRequestStrategy(IClientProxyFactory clientProxyFactory, IAPIIdentityServiceSettings identityServiceSettings, ITracer tracer)
        {
            _clientProxyFactory = clientProxyFactory;
            _identityServiceSettings = identityServiceSettings;
            _tracer = tracer;
        }

        public long[] Request(int count)
        {
            Interlocked.Increment(ref _threadsCount);

            EnsureCount(count);

            var ids = new long[count];

            var i = 0;
            while (i < ids.Length)
            {
                long id;
                if (!_idBuffer.TryDequeue(out id))
                {
                    EnsureCount(ids.Length - i);
                    continue;
                }

                ids[i++] = id;
            }

            Interlocked.Decrement(ref _threadsCount);

            return ids;
        }

        private void EnsureCount(int requestedCount)
        {
            var availableCount = _idBuffer.Count;
            if (availableCount >= requestedCount)
            {
                return;
            }

            var missingCount = requestedCount - availableCount;
            var coercedCount = Math.Min(Math.Max(_nextRequestedCount, missingCount), MaxRequestedCount);

            _tracer.DebugFormat("Requested identifiers coerced count: {0}. Concurrently requesting threads count: {1}.", coercedCount, _threadsCount);

            var sw = Stopwatch.StartNew();

            long[] ids;
            try
            {
                ids = _clientProxyFactory.GetClientProxy<IIdentityProviderApplicationService, WSHttpBinding>().Execute(x => x.GetIdentities(coercedCount));
            }
            catch (Exception ex)
            {
                _tracer.ErrorFormat("An error occurred while requesting identifiers", ex);
                throw;
            }

            // TODO {all, 16.03.2015}: Ловим тормозные запросы к identity service. Убрать, когда решится ситуация с кривыми маршрутами из-за proxy (либо после выпуска фичи ISM)
            var elapsed = sw.Elapsed;
            if (elapsed > TimeSpan.FromSeconds(5))
            {
                var proxy = WebRequest.GetSystemWebProxy().GetProxy(_identityServiceSettings.BaseUrl);
                _tracer.WarnFormat("Too long identity request duration: {0}. Used proxy url: {1}", elapsed, proxy);
            }

            foreach (var id in ids)
            {
                _idBuffer.Enqueue(id);
            }

            _nextRequestedCount = Math.Min(_nextRequestedCount * 2, MaxRequestedCount);
        }
    }
}