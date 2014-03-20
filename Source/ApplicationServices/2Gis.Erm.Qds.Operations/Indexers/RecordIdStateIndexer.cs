using System;
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
            if (finder == null)
            {
                throw new ArgumentNullException("finder");
            }
            if (elasticApi == null)
            {
                throw new ArgumentNullException("elasticApi");
            }
            if (searchSettings == null)
            {
                throw new ArgumentNullException("searchSettings");
            }

            _finder = finder;
            _elasticApi = elasticApi;
            _searchSettings = searchSettings;
        }

        void IDocumentIndexer<RecordIdState>.IndexAllDocuments()
        {
            var lastPbo = _finder.FindAll<PerformedBusinessOperation>().OrderByDescending(pbo => pbo.Id).FirstOrDefault();
            var lastRecordId = lastPbo == null ? long.MinValue : lastPbo.Id;

            _elasticApi.BulkExclusive<RecordIdState>(GetRecordBulk(lastRecordId));
        }

        private IEnumerable<BulkDescriptor> GetRecordBulk(long lastRecordId)
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