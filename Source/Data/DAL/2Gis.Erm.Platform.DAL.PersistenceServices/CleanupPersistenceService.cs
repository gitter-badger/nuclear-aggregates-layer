using System;

using DoubleGis.Erm.Platform.DAL.AdoNet;

namespace DoubleGis.Erm.Platform.DAL.PersistenceServices
{
    public class CleanupPersistenceService : ICleanupPersistenceService
    {
        private readonly IDatabaseCaller _databaseCaller;

        public CleanupPersistenceService(IDatabaseCaller databaseCaller)
        {
            _databaseCaller = databaseCaller;
        }

        public void CleanupErm(TimeSpan timeout, int logSizeInDays)
        {
            _databaseCaller.ExecuteProcedure("[Shared].[CleanupERM]", timeout, new Tuple<string, object>("@logSizeInDays", logSizeInDays));
        }

        public void CleanupErmLogging(TimeSpan timeout, int logSizeInDays)
        {
            _databaseCaller.ExecuteProcedure("[Shared].[CleanupERMLogging]", timeout, new Tuple<string, object>("@logSizeInDays", logSizeInDays));
        }

        public void CleanupCrm(TimeSpan timeout, int logSizeInDays)
        {
            _databaseCaller.ExecuteProcedure("[Shared].[CleanupMSCRM]", timeout, new Tuple<string, object>("@logSizeInDays", logSizeInDays));
        } 
    }
} 