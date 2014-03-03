using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Qds.API.Core.Settings;
using DoubleGis.Erm.Qds.API.Operations.Indexers;
using DoubleGis.Erm.Qds.Common;
using DoubleGis.Erm.Qds.Etl.Extract.EF;
using DoubleGis.Erm.Qds.Operations.Extensions;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Indexers
{
    public sealed class RecordIdStateIndexer :
        IDocumentIndexer<RecordIdState>
    {
        private readonly IFinder _finder;
        private readonly IElasticApi _elasticApi;
        private readonly ISearchSettings _searchSettings;

        public RecordIdStateIndexer(IFinder finder, IElasticApi elasticApi, ISearchSettings searchSettings)
        {
            _finder = finder;
            _elasticApi = elasticApi;
            _searchSettings = searchSettings;
        }

        void IDocumentIndexer<RecordIdState>.IndexAllDocuments()
        {
            var lastRecordId = _finder.FindAll<PerformedBusinessOperation>().Select(pbo => pbo.Id).Max();

            _elasticApi.BulkExclusive<RecordIdState>(GetTerritoryDoc(lastRecordId));
        }

        private IEnumerable<BulkDescriptor> GetTerritoryDoc(long lastRecordId)
        {
            var indexDescriptors = new[] { new BulkIndexDescriptor<RecordIdState>().Id(0).Object(new RecordIdState(0, lastRecordId)) };

            return indexDescriptors.Batch(_searchSettings.BatchSize).Select(batch =>
            {
                var bulkDescriptor = new BulkDescriptor();
                foreach (var iterator in batch)
                {
                    var indexDescriptor = iterator;
                    bulkDescriptor.Index<RecordIdState>(x => indexDescriptor);
                }

                return bulkDescriptor;
            });
        }
    }
}