using DoubleGis.Erm.Qds.Common.ElasticClient;

namespace DoubleGis.Erm.Qds.Operations.Infrastructure
{
    public sealed class FilterHelper
    {
        private readonly IElasticClientFactory _elasticClientFactory;

        public FilterHelper(IElasticClientFactory elasticClientFactory)
        {
            _elasticClientFactory = elasticClientFactory;
        }


    }
}
