using System;

using Nest;

namespace DoubleGis.Erm.Qds.Common.ElasticClient
{
    public interface IElasticClientFactory
    {
        void UsingElasticClient(Action<IElasticClient> action);
        TResult UsingElasticClient<TResult>(Func<IElasticClient, TResult> func);
    }
}