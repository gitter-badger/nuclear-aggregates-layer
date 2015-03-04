using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using Nuclear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings
{
    public sealed class ConnectionStringsSettingsAspect : ISettingsAspect, IConnectionStringSettings
    {
        private readonly IReadOnlyDictionary<ConnectionStringName, ConnectionStringSettings> _connectionStringsMap;

        public ConnectionStringsSettingsAspect()
        {
            var specifiedConnectionStringsMap = ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>().ToDictionary(connection => connection.Name);

            var availableConnectionStringsMap = new Dictionary<ConnectionStringName, ConnectionStringSettings>();
            foreach (var connectionStringAlias in Enum.GetValues(typeof(ConnectionStringName)).Cast<ConnectionStringName>().Where(x => x != ConnectionStringName.None))
            {
                var connectionStringName = connectionStringAlias.ToDefaultConnectionStringName();
                ConnectionStringSettings connection;
                if (specifiedConnectionStringsMap.TryGetValue(connectionStringName, out connection))
                {
                    availableConnectionStringsMap[connectionStringAlias] = connection;
                }
            }

            _connectionStringsMap = availableConnectionStringsMap;
        }

        public IReadOnlyDictionary<ConnectionStringName, ConnectionStringSettings> AllConnections
        {
            get { return _connectionStringsMap; }
        }

        public string GetConnectionString(ConnectionStringName connectionStringNameAlias)
        {
            return GetConnectionStringSettings(connectionStringNameAlias).ConnectionString;
        }

        public ConnectionStringSettings GetConnectionStringSettings(ConnectionStringName connectionStringNameAlias)
        {
            ConnectionStringSettings connectionStringSettings;
            if (!_connectionStringsMap.TryGetValue(connectionStringNameAlias, out connectionStringSettings))
            {
                throw new ConfigurationErrorsException(string.Format("Can't find connection string for alias '{0}'", connectionStringNameAlias));
            }

            return connectionStringSettings;
        }

        public string ResolveConnectionStringName(ConnectionStringName connectionStringNameAlias)
        {
            return connectionStringNameAlias.ToDefaultConnectionStringName();
        }
    }
}
