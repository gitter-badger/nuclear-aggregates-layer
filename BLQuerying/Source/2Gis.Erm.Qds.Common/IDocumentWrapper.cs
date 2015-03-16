namespace DoubleGis.Erm.Qds.Common
{
    public interface IDocumentWrapper<out TDocument>
    {
        string Id { get; }
        TDocument Document { get; }
        long? Version { get; }
    }
}