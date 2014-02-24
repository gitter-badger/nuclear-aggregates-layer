using System;
using System.Collections.Generic;

using DoubleGis.Erm.Qds;
using DoubleGis.Erm.Qds.Common.ElasticClient;

using Nest.Resolvers;

namespace DoubleGis.Erm.Elastic.Nest.Qds
{
    public class ElasticDocsStorage : IDocsStorage
    {
        private readonly IElasticClientFactory _elasticClientFactory;
        private readonly IElasticMeta _elasticMeta;
        private readonly IElasticResponseHandler _responseHandler;

        public ElasticDocsStorage(IElasticClientFactory elasticClientFactory, IElasticMeta elasticMeta, IElasticResponseHandler responseHandler)
        {
            if (elasticClientFactory == null)
            {
                throw new ArgumentNullException("elasticClientFactory");
            }
            if (elasticMeta == null)
            {
                throw new ArgumentNullException("elasticMeta");
            }
            if (responseHandler == null)
            {
                throw new ArgumentNullException("responseHandler");
            }

            _elasticClientFactory = elasticClientFactory;
            _elasticMeta = elasticMeta;
            _responseHandler = responseHandler;
        }

        public IEnumerable<TDoc> Find<TDoc>(IDocsQuery query) where TDoc : class, IDoc
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            var searchDescriptor = _elasticMeta.CreatePage<TDoc>(query);

            while (searchDescriptor != null)
            {
                var sd = searchDescriptor;
                var response = _elasticClientFactory.UsingElasticClient(ec => ec.Search(sd.SearchDescriptor));
                _responseHandler.ThrowWhenError(response);

                foreach (var doc in response.Documents)
                {
                    yield return doc;
                }

                searchDescriptor = _elasticMeta.NextPage(searchDescriptor, response);
            }
        }

        public void Update(IEnumerable<IDoc> docs)
        {
            if (docs == null)
            {
                throw new ArgumentNullException("docs");
            }

            foreach (var doc in docs)
            {
                IDoc d = doc;

                // TODO Формирование имени типа делегировать в _elasticMeta
                var indexResponse = _elasticClientFactory.UsingElasticClient(ec => ec.Index(d, _elasticMeta.GetIndexName(d.GetType().Name), d.GetType().Name.MakePlural().ToLowerInvariant()));
                _responseHandler.ThrowWhenError(indexResponse);
            }

            var shardResponse = _elasticClientFactory.UsingElasticClient(ec => ec.Refresh());
            _responseHandler.ThrowWhenError(shardResponse);
        }
    }
}
