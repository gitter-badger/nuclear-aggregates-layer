using System.Collections.Generic;

using NuClear.Settings.API;

namespace NuClear.Storage.ConnectionStrings
{
    public class ConnectionStringSettingsAspect : ISettingsAspect, IConnectionStringSettings
    {
        private readonly IReadOnlyDictionary<IConnectionStringIdentity, string> _connectionStringsMap;

        public ConnectionStringSettingsAspect(IReadOnlyDictionary<IConnectionStringIdentity, string> connectionStringsMap)
        {
            _connectionStringsMap = connectionStringsMap;
        }

        public string GetConnectionString(IConnectionStringIdentity connectionStringIdentity)
        {
            string connectionString;
            if (!_connectionStringsMap.TryGetValue(connectionStringIdentity, out connectionString))
            {
                throw new KeyNotFoundException(string.Format("Can't find connection string for identity '{0}'", connectionStringIdentity));
            }

            return connectionString;
        }

        public IReadOnlyDictionary<IConnectionStringIdentity, string> AllConnectionStrings
        {
            get { return _connectionStringsMap; }
        }
    }
}