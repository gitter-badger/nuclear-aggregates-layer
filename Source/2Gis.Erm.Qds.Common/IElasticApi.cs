using System.Collections.Generic;

using Nest;

namespace DoubleGis.Erm.Qds.Common
{
    public interface IElasticApi
    {
        string GetIsolatedIndexName(string indexName);
        void Bulk(IEnumerable<BulkDescriptor> bulkDescriptors);
        void BulkExclusive<TDocument>(IEnumerable<BulkDescriptor> bulkDescriptors);
    }
}