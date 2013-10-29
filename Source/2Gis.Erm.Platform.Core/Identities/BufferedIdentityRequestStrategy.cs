using System;
using System.Collections.Generic;
using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Metadata;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

namespace DoubleGis.Erm.Platform.Core.Identities
{
    public sealed class BufferedIdentityRequestStrategy : IIdentityRequestStrategy
    {
        // Максимальное количество идентификаторов, которое можно запросить у сервиса генерации идентификаторов
        private const int MaxRequestedCount = 32767;

        private readonly IClientProxyFactory _clientProxyFactory;

        private readonly Queue<long> _idBuffer = new Queue<long>();
        private int _nextRequestedCount = 1;
        public BufferedIdentityRequestStrategy(IClientProxyFactory clientProxyFactory)
        {
            _clientProxyFactory = clientProxyFactory;
        }

        public long[] Request(int count)
        {
            EnsureCount(count);

            var ids = new long[count];
            for (int i = 0; i < ids.Length; i++)
            {
                ids[i] = _idBuffer.Dequeue();
            }

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

            var ids = _clientProxyFactory.GetClientProxy<IIdentityProviderApplicationService, WSHttpBinding>().Execute(x => x.GetIdentities(num));

            foreach (var id in ids)
            {
                _idBuffer.Enqueue(id);
            }

            _nextRequestedCount = Math.Min(_nextRequestedCount * 2, MaxRequestedCount);
        }
    }
}