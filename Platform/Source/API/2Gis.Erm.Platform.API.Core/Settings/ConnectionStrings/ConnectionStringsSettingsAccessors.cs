namespace DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings
{
    public static class ConnectionStringsSettingsAccessors
    {
        public static string LoggingConnectionString(this NuClear.Storage.ConnectionStrings.IConnectionStringSettings connectionStringSettings)
        {
            return connectionStringSettings.GetConnectionString(LoggingConnectionStringIdentity.Instance);
        }
    }
}
