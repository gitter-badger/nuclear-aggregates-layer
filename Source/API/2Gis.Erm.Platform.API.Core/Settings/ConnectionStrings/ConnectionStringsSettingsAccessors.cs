namespace DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings
{
    public static class ConnectionStringsSettingsAccessors
    {
        public static string LoggingConnectionString(this IConnectionStringSettings connectionStringSettings)
        {
            return connectionStringSettings.GetConnectionString(ConnectionStringName.Logging);
        }
    }
}