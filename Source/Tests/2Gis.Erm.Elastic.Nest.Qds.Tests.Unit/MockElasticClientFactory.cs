using System;

using DoubleGis.Erm.Qds.Common.ElasticClient;

using Nest;

namespace DoubleGis.Erm.Elastic.Nest.Qds.Tests.Unit
{
    public class MockElasticClientFactory : IElasticClientFactory
    {
        private readonly IElasticClient _elasticClient;

        public MockElasticClientFactory(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public void UsingElasticClient(Action<IElasticClient> action)
        {
            action(_elasticClient);
        }

        public TResult UsingElasticClient<TResult>(Func<IElasticClient, TResult> func)
        {
            return func(_elasticClient);
        }
    }
}