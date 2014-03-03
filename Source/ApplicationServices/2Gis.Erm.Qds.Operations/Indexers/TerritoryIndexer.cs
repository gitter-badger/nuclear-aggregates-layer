using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Qds.API.Core.Settings;
using DoubleGis.Erm.Qds.API.Operations.Indexers;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Docs;
using DoubleGis.Erm.Qds.Operations.Extensions;

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
        private readonly ISearchSettings _searchSettings;

        public TerritoryIndexer(IFinder finder, IEntityIndexerIndirect<Client> clientIndexer, IElasticApi elasticApi, ISearchSettings searchSettings)
        {
            _finder = finder;
            _clientIndexer = clientIndexer;
            _elasticApi = elasticApi;
            _searchSettings = searchSettings;
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
            var result = GetTerritoryDoc(allEntities, true);
            _elasticApi.BulkExclusive<TerritoryDoc>(result);
        }

        private IEnumerable<BulkDescriptor> GetTerritoryDoc(IQueryable<Territory> query, bool indirectly = false)
        {
            var dtos = query.Select(x => new
            {
                x.Id,
                x.Name,
                x.Timestamp,
            });

            var indexDescriptors = dtos
                .AsEnumerable()
                .Select(x => new BulkIndexDescriptor<TerritoryDoc>()
                .Id(x.Id.ToString(CultureInfo.InvariantCulture))
                .Version(x.Timestamp, indirectly)
                .Object(new TerritoryDoc
                {
                    Id = x.Id,
                    Name = x.Name,
                }));

            return indexDescriptors.Batch(_searchSettings.BatchSize).Select(batch =>
            {
                var bulkDescriptor = new BulkDescriptor();
                foreach (var iterator in batch)
                {
                    var indexDescriptor = iterator;
                    bulkDescriptor.Index<TerritoryDoc>(x => indexDescriptor);
                }

                return bulkDescriptor;
            });
        }
    }
}