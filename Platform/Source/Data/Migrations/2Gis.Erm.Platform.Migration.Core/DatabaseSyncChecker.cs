using System.Linq;

namespace DoubleGis.Erm.Platform.Migration.Core
{
    public class DatabaseSyncChecker : IDatabaseSyncChecker
    {
        private readonly IAppliedVersionsReader _appliedVersionsReader;
        private readonly IMigrationDescriptorsProvider _migrationsProvider;

        public DatabaseSyncChecker(IAppliedVersionsReader appliedVersionsReader, IMigrationDescriptorsProvider migrationsProvider)
        {
            _appliedVersionsReader = appliedVersionsReader;
            _migrationsProvider = migrationsProvider;
        }

        public bool HasPendingMigrations()
        {
            RefreshAppliedVersions();
            var alreadyAppliedMigrations = _appliedVersionsReader.AppliedVersionsInfo.GetAppliedMigrations();
            return _migrationsProvider.MigrationDescriptors.Where(x => !alreadyAppliedMigrations.Contains(x.Version)).Any();
        }

        public void RefreshAppliedVersions()
        {
            _appliedVersionsReader.LoadVersionInfo();
        }

        public void Close()
        {
        }
    }
}
