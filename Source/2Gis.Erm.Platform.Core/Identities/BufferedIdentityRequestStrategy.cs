﻿using System;
using System.Collections.Concurrent;
using System.ServiceModel;
using System.Threading;

using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Metadata;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.Platform.Core.Identities
{
    public sealed class BufferedIdentityRequestStrategy : IIdentityRequestStrategy
    {
        // Максимальное количество идентификаторов, которое можно запросить у сервиса генерации идентификаторов
        private const int MaxRequestedCount = 32767;

        private readonly IClientProxyFactory _clientProxyFactory;
        private readonly ICommonLog _logger;

        private readonly ConcurrentQueue<long> _idBuffer = new ConcurrentQueue<long>();
        private int _nextRequestedCount = 1;
        private int _threadsCount;

        public BufferedIdentityRequestStrategy(IClientProxyFactory clientProxyFactory, ICommonLog logger)
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

        private void EnsureCount(int min)
        {
            var bufferedCount = _idBuffer.Count;

            if (bufferedCount >= min)
            {
                return;
            }

            var missingCount = min - bufferedCount;

            var num = Math.Max(_nextRequestedCount, missingCount);

            _logger.InfoFormatEx("Requesting {0} identifiers from identity service. Number of concurrently executing threads is {1}.", num, _threadsCount);

            long[] ids;
            try
            {
                ids = _clientProxyFactory.GetClientProxy<IIdentityProviderApplicationService, WSHttpBinding>().Execute(x => x.GetIdentities(num));
            }
            catch (Exception ex)
            {
                _logger.ErrorFormatEx("An error occurred while requesting identifiers", ex);
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