namespace DoubleGis.Erm.Platform.Migration.Base
{
    public interface INonDefaultDatabaseMigration
    {
        ErmConnectionStringKey ConnectionStringKey { get; }
    }
}
