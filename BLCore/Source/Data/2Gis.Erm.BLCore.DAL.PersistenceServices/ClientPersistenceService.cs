using System;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL.AdoNet;
using DoubleGis.Erm.Platform.DAL.PersistenceServices.Utils;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices
{
    public class ClientPersistenceService : IClientPersistenceService
    {
        private readonly IDatabaseCaller _databaseCaller;

        public ClientPersistenceService(IDatabaseCaller databaseCaller)
        {
            _databaseCaller = databaseCaller;
        }

        public EntityChangesContext CalculateClientPromising(long modifiedBy, TimeSpan timeout)
        {
            var changedEntitiesReport = _databaseCaller.ExecuteProcedureWithResultTable("Integration.CalculateClientPromising",
                                                                                        timeout,
                                                                                        new Tuple<string, object>("ModifiedBy", modifiedBy));

            return changedEntitiesReport.ToEntityChanges();
        }
    }
}