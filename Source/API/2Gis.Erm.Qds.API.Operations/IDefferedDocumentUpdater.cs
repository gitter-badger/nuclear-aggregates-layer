using DoubleGis.Erm.Qds.Docs;

namespace DoubleGis.Erm.Qds.API.Operations
{
    public interface IDocumentWrapper<TDocument>
    {
        string Id { get; set; }
        TDocument Document { get; set; }
    }

    public sealed class DocumentWrapper<TDocument> : IDocumentWrapper<TDocument>
    {
        public string Id { get; set; }
        public TDocument Document { get; set; }
    }

    public interface IReplicationQueueHelper
    {
        void Add(ReplicationQueue replicationQueue);
        void Delete(string id);

        IDocumentWrapper<ReplicationQueue>[] LoadQueue();
    }

    public interface IDefferedDocumentUpdater
    {
        void IndexAllDocuments();
    }
}