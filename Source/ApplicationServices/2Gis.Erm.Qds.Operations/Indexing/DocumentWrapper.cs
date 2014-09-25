using System;

using DoubleGis.Erm.Qds.API.Operations.Indexing;
using DoubleGis.Erm.Qds.Common;

namespace DoubleGis.Erm.Qds.Operations.Indexing
{
    public sealed class DocumentWrapper<TDocument> : IDocumentWrapper<TDocument>
        where TDocument : class
    {
        private static readonly Type DocumentTypePrivate = typeof(TDocument);
        
        public string Id { get; set; }
        public string Version { get; set; }
        public TDocument Document { get; set; }
        public Type DocumentType
        {
            get { return DocumentTypePrivate; }
        }

        public Func<ElasticApi.ErmBulkDescriptor, ElasticApi.ErmBulkDescriptor> IndexFunc
        {
            get
            {
                if (string.IsNullOrEmpty(Version))
                {
                    return bulkDescriptor => (ElasticApi.ErmBulkDescriptor)bulkDescriptor
                        .Create<TDocument>(bulkIndexDescriptor => bulkIndexDescriptor
                            .Id(Id)
                            .Document(Document));
                }

                return bulkDescriptor => bulkDescriptor
                    .UpdateWithMerge<TDocument>(bulkUpdateDescriptor => bulkUpdateDescriptor
                        .Id(Id)
                        .Doc(Document)
                        .Version(Version));
            }
        }
    }
}