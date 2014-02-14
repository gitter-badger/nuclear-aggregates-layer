using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings
{
    public sealed class ConnectionStringsSettingsAspect
    {
        private readonly IReadOnlyDictionary<ConnectionStringName, string> _connectionStringsMap;

        public ConnectionStringsSettingsAspect()
        {
            var specifiedConnectionStringsMap =
                ConfigurationManager
                    .ConnectionStrings
                        .Cast<ConnectionStringSettings>()
                        .ToDictionary(connection => connection.Name);

            var availableConnectionStringsMap = new Dictionary<ConnectionStringName, string>();
            foreach (var connectionStringAlias in Enum.GetValues(typeof(ConnectionStringName)).Cast<ConnectionStringName>().Where(x => x != ConnectionStringName.None))
            {
                var connectionStringName = connectionStringAlias.ToDefaultConnectionStringName();
                ConnectionStringSettings connection;
                if (specifiedConnectionStringsMap.TryGetValue(connectionStringName, out connection))
                {
                    availableConnectionStringsMap[connectionStringAlias] = connection.ConnectionString;
                }
            }

            _connectionStringsMap = availableConnectionStringsMap;
        }

        public string GetConnectionString(ConnectionStringName connectionStringNameAlias)
        {
            string connectionString;
            if (!_connectionStringsMap.TryGetValue(connectionStringNameAlias, out connectionString))
            {
                throw new ConfigurationErrorsException(string.Format("Can't find connection string for alias '{0}'", connectionStringNameAlias));
            }

            return connectionString;
        }

        public string ResolveConnectionStringName(ConnectionStringName connectionStringNameAlias)
        {
            return connectionStringNameAlias.ToDefaultConnectionStringName();
        }

        public IReadOnlyDictionary<ConnectionStringName, string> AllConnections
        {
            get { return _connectionStringsMap; }
        }
    }
}
