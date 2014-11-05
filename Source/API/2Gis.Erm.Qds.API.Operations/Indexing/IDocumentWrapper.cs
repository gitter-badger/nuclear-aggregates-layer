using System;

using DoubleGis.Erm.Qds.Common;

namespace DoubleGis.Erm.Qds.API.Operations.Indexing
{
    public interface IIndexedDocumentWrapper
    {
        Type DocumentType { get; }
        Func<ElasticApi.ErmBulkDescriptor, ElasticApi.ErmBulkDescriptor> IndexFunc { get; }
    }
}