using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Qds.API.Operations.Indexers;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Docs;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Indexers
{
    public sealed class TerritoryIndexer :
        IEntityIndexer<Territory>,
        IEntityIndexerIndirect<Territory>,
        IDocumentIndexer<TerritoryDoc>
    {
        private readonly IFinder _finder;
        private readonly IEntityIndexerIndirect<Client> _clientIndexer;
        private readonly IElasticApi _elasticApi;

        public TerritoryIndexer(IFinder finder, IEntityIndexerIndirect<Client> clientIndexer, IElasticApi elasticApi)
        {
            _finder = finder;
            _clientIndexer = clientIndexer;
            _elasticApi = elasticApi;
        }

        void IEntityIndexer<Territory>.IndexEntities(params long[] ids)
        {
            var query = _finder.Find<Territory>(x => ids.Contains(x.Id));
            var result = GetTerritoryDoc(query);
            _elasticApi.Bulk(result);

            // indirect updates
            var userIds = query.Select(x => x.Id).ToArray();
            var userClients = _finder.FindAll<Client>().Join(userIds, x => x.OwnerCode, x => x, (x, y) => x);
            _clientIndexer.IndexEntitiesIndirectly(userClients);
        }

        void IEntityIndexerIndirect<Territory>.IndexEntitiesIndirectly(IQueryable<Territory> query)
        {
            var result = GetTerritoryDoc(query, true);
            _elasticApi.Bulk(result);
        }

        void IDocumentIndexer<TerritoryDoc>.IndexAllDocuments()
        {
            var allEntities = _finder.FindAll<Territory>();
            var result = GetTerritoryDoc(allEntities);
            _elasticApi.Bulk(result);
            _elasticApi.Refresh(x => x.Index<TerritoryDoc>());
        }

        private IEnumerable<Func<BulkDescriptor, BulkDescriptor>> GetTerritoryDoc(IQueryable<Territory> query, bool indirectly = false)
        {
            var dtos = query.Select(x => new
            {
                x.Id,
                x.Name,
                x.Timestamp,
            });

            var bulkDescriptors = dtos
                .AsEnumerable()
                .Select(x => new Func<BulkDescriptor, BulkDescriptor>(bulkDescriptor => bulkDescriptor
                    .Index<TerritoryDoc>(bulkIndexDescriptor => bulkIndexDescriptor
                        .Id(x.Id.ToString(CultureInfo.InvariantCulture))
                        .Object(new TerritoryDoc
                        {
                            Id = x.Id.ToString(),
                            Name = x.Name,
                        })
                    ))
            );

            return bulkDescriptors;
        }
    }
}