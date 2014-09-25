using System;

using DoubleGis.Erm.Qds.Common;

namespace DoubleGis.Erm.Qds.API.Operations.Indexing
{
    public interface IDocumentWrapper
    {
        Type DocumentType { get; }
        Func<ElasticApi.ErmBulkDescriptor, ElasticApi.ErmBulkDescriptor> IndexFunc { get; }
    }

    public interface IDocumentWrapper<out TDocument> : IDocumentWrapper
    {
        string Id { get; }
        TDocument Document { get; }
    }
}