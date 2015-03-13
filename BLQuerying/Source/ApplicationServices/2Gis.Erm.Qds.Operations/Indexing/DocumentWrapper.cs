using System;

using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.Common;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public sealed class IndexedDocumentWrapper<TDocument> : DocumentWrapper<TDocument>, IIndexedDocumentWrapper
        where TDocument : class
    {
        private static readonly Type DocumentTypePrivate = typeof(TDocument);
        public Type DocumentType
        {
            get { return DocumentTypePrivate; }
        }

        public UpdateType UpdateType { get; set; }

        public Func<ErmBulkDescriptor, ErmBulkDescriptor> IndexFunc
        {
            get
            {
                if (Version == null)
                {
                    return bulkDescriptor => (ErmBulkDescriptor)bulkDescriptor
                        .Create<TDocument>(bulkIndexDescriptor => bulkIndexDescriptor
                            .Id(Id)
                            .Document(Document));
                }

                return bulkDescriptor => bulkDescriptor
                    .Update<TDocument>(bulkUpdateDescriptor => bulkUpdateDescriptor
                    .Id(Id)
                    .Doc(Document)
                    .Version(Version.Value.ToString()), UpdateType);
            }
        }
    }
}