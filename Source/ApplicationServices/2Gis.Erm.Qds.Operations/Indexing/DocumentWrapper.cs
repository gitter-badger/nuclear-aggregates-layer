using System;

using Nest;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public interface IDocumentWrapper
    {
        Type DocumentType { get; }
        Func<BulkDescriptor, BulkDescriptor> IndexFunc { get; }
    }

    public interface IDocumentWrapper<out TDocument> : IDocumentWrapper
    {
        string Id { get; }
        TDocument Document { get; }
    }

    public sealed class DocumentWrapper<TDocument> : IDocumentWrapper<TDocument>
        where TDocument : class
    {
        private static readonly Type DocumentTypePrivate = typeof(TDocument);
        public Type DocumentType { get { return DocumentTypePrivate; } }

        public string Id { get; set; }
        public string Version { get; set; }
        public TDocument Document { get; set; }

        public Func<BulkDescriptor, BulkDescriptor> IndexFunc
        {
            get
            {
                if (string.IsNullOrEmpty(Version))
                {
                    return bulkDescriptor => bulkDescriptor
                        .Create<TDocument>(bulkIndexDescriptor => bulkIndexDescriptor
                            .Id(Id)
                            .Document(Document));
                }

                return bulkDescriptor => bulkDescriptor
                    .Update<TDocument, TDocument>(bulkUpdateDescriptor => bulkUpdateDescriptor
                        .Id(Id)
                        .Doc(Document)
                        .Version(Version));
            }
        }
    }
}