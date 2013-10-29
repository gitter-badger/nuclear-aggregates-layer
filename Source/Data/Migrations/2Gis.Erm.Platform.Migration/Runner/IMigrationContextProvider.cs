using DoubleGis.Erm.Platform.Migration.Base;

namespace DoubleGis.Erm.Platform.Migration.Runner
{
    internal interface IMigrationContextProvider
    {
        IMigrationContext GetDatabaseContext(ErmConnectionStringKey connectionStringKey = ErmConnectionStringKey.Default);
    }
}
