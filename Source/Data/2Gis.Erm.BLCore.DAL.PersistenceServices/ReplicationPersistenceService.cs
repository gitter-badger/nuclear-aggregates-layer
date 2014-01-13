using System;

using DoubleGis.Erm.Platform.DAL.AdoNet;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices
{
    public class ReplicationPersistenceService : IReplicationPersistenceService
    {
        private readonly IDatabaseCaller _databaseCaller;

        public ReplicationPersistenceService(IDatabaseCaller databaseCaller)
        {
            _databaseCaller = databaseCaller;
        }

        public void ReplicateEntitiesToMscrm(int chunkSize, int timeout)
        {
            _databaseCaller.ExecuteProcedure("Shared.ReplicateEntitiesToCrm", timeout, new Tuple<string, object>("chunkSize", chunkSize));
        }
    }
}
