using System;

using DoubleGis.Erm.Platform.DAL.AdoNet;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices
{
    public class ClientPersistenceService : IClientPersistenceService
    {
        private readonly IDatabaseCaller _databaseCaller;

        public ClientPersistenceService(IDatabaseCaller databaseCaller)
        {
            _databaseCaller = databaseCaller;
        }

        public void CalculateClientPromising(long modifiedBy, int timeout, bool enableReplication)
        {
            _databaseCaller.ExecuteProcedure("Integration.CalculateClientPromising",
                                             timeout,
                                             new Tuple<string, object>("ModifiedBy", modifiedBy),
                                             new Tuple<string, object>("EnableReplication", enableReplication));
        }
    }
}