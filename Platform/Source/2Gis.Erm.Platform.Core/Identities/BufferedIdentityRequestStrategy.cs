using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Threading;

using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Metadata;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.Platform.Core.Identities
{
    public sealed class BufferedIdentityRequestStrategy : IIdentityRequestStrategy
    {
        // Максимальное количество идентификаторов, которое можно запросить у сервиса генерации идентификаторов
        private const int MaxRequestedCount = 32767;

        private readonly IClientProxyFactory _clientProxyFactory;
        private readonly ITracer _logger;

        private readonly ConcurrentQueue<long> _idBuffer = new ConcurrentQueue<long>();
        private int _nextRequestedCount = 1;
        private int _threadsCount;

        public BufferedIdentityRequestStrategy(IClientProxyFactory clientProxyFactory, ITracer logger)
        {
            _clientProxyFactory = clientProxyFactory;
            _logger = logger;
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
            int coercedCount = Math.Min(Math.Max(_nextRequestedCount, missingCount), MaxRequestedCount);

            _logger.DebugFormat("Requested identifiers coerced count: {0}. Concurrently requesting threads count: {1}.", requestedCount, _threadsCount);

            long[] ids;
            try
            {
                ids = _clientProxyFactory.GetClientProxy<IIdentityProviderApplicationService, WSHttpBinding>().Execute(x => x.GetIdentities(coercedCount));
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat("An error occurred while requesting identifiers", ex);
                throw;
            }

            foreach (var id in ids)
            {
                _idBuffer.Enqueue(id);
            }

            _nextRequestedCount = Math.Min(_nextRequestedCount * 2, MaxRequestedCount);
        }
    }
}