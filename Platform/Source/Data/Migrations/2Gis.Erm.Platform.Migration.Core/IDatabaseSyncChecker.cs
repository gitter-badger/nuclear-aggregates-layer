namespace DoubleGis.Erm.Platform.Migration.Core
{
    public interface IDatabaseSyncChecker 
    {
        bool HasPendingMigrations();
        void RefreshAppliedVersions();
        void Close();
    }
}
