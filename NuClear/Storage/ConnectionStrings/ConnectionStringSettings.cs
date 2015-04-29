using System.Collections.Generic;

using NuClear.Settings.API;

namespace NuClear.Storage.ConnectionStrings
{
    public class ConnectionStringSettings : ISettingsAspect, IConnectionStringSettings
    {
        private readonly IReadOnlyDictionary<IConnectionStringIdentity, string> _connectionStringsStorage;

        public ConnectionStringSettings(IReadOnlyDictionary<IConnectionStringIdentity, string> connectionStringsStorage)
        {
            _connectionStringsStorage = connectionStringsStorage;
        }

        public string GetConnectionString(IConnectionStringIdentity connectionStringIdentity)
        {
            string connectionString;
            if (!_connectionStringsStorage.TryGetValue(connectionStringIdentity, out connectionString))
            {
                throw new KeyNotFoundException(string.Format("Can't find connection string for identity '{0}'", connectionStringIdentity));
            }

            return connectionString;
        }
    }
}