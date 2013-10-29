using DoubleGis.Erm.Platform.API.Core.PersistenceCleanup;
using DoubleGis.Erm.Platform.DAL.PersistenceServices;

namespace DoubleGis.Erm.Platform.Core.PersistenceCleanup
{
    public class DbCleaner : IDbCleaner
    {
        private readonly IDBCleanupSettings _settings;
        private readonly ICleanupPersistenceService _cleanupPersistenceService;

        public DbCleaner(IDBCleanupSettings settings, ICleanupPersistenceService cleanupPersistenceService)
        {
            _settings = settings;
            _cleanupPersistenceService = cleanupPersistenceService;
        }

        // TODO : таймауты увеличены для того, чтобы в первый запуск процедуры отработали, после этого можно будет таймаут уменьшить.
        public void CleanupErm()
        {
            _cleanupPersistenceService.CleanupErmLogging(7200, _settings.LogSizeInDays);
            _cleanupPersistenceService.CleanupErm(7200, _settings.LogSizeInDays);
        }

        public void CleanupCrm()
        {
            _cleanupPersistenceService.CleanupCrm(7200, _settings.LogSizeInDays);
        }
    }
}
