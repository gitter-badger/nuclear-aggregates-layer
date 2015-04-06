using System;

using DoubleGis.Erm.Qds.Common;

namespace DoubleGis.Erm.Qds.API.Operations.Indexing
{
    public interface IIndexedDocumentWrapper
    {
        Type DocumentType { get; }
        Func<ErmBulkDescriptor, ErmBulkDescriptor> IndexFunc { get; }
    }
}