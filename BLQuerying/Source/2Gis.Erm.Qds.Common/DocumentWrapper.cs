namespace DoubleGis.Erm.Qds.Common
{
    public class DocumentWrapper<TDocument> : IDocumentWrapper<TDocument>
    {
        public string Id { get; set; }
        public TDocument Document { get; set; }
        public long? Version { get; set; }
    }
}