namespace DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings
{
    public static class ConnectionStringsSettingsAccessors
    {
        public static string LoggingConnectionString(this IConnectionStringSettingsHost connectionStringSettingsHost)
        {
            return connectionStringSettingsHost.ConnectionStrings.GetConnectionString(ConnectionStringName.Logging);
        }
    }
}