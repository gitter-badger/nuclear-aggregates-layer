namespace DoubleGis.Erm.Platform.Migration.Core
{
    public interface IMigrationsProvider
    {
        IMigration GetMigrationImplementation(MigrationDescriptor descriptor);
    }
}
