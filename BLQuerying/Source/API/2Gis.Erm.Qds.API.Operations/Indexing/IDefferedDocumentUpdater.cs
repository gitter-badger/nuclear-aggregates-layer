using System.Threading;

namespace DoubleGis.Erm.Qds.API.Operations.Indexing
{
    public interface IDefferedDocumentUpdater
    {
        void IndexAllDocuments(CancellationToken cancellationToken);
    }
}