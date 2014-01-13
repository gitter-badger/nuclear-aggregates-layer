using DoubleGis.Erm.Platform.DAL;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices
{
    public interface IReplicationPersistenceService : ISimplifiedPersistenceService
    {
        void ReplicateEntitiesToMscrm(int chunkSize, int timeout);
    }
}
