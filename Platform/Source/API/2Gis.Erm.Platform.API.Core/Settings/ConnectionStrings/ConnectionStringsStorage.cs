using System.Collections.Generic;
using System.Configuration;
using System.Linq;

using NuClear.Storage.ConnectionStrings;

namespace DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings
{
    public class ConnectionStringsStorage
    {
        private static readonly IDictionary<IConnectionStringIdentity, string> DefaultName2ConnectionStringMap = new Dictionary<IConnectionStringIdentity, string>
        {
            { LoggingConnectionStringIdentity.Instance, "ErmLogging" },
            { ErmConnectionStringIdentity.Instance, "Erm" },
            { OrderValidationConnectionStringIdentity.Instance, "ErmValidation" },
            { MsCrmConnectionStringIdentity.Instance, "CrmConnection" },
            { ErmReportsConnectionStringIdentity.Instance, "ErmReports" },
            { SearchStorageConnectionStringIdentity.Instance, "ErmSearch" },
            { PerformedOperationsServiceBusConnectionStringIdentity.Instance, "ErmPerformedOperationsServiceBus" },
            { InfrastructureConnectionStringIdentity.Instance, "ErmInfrastructure" },
        };

        public ConnectionStringsStorage()
        {
            var specifiedConnectionStringsMap = ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>().ToDictionary(connection => connection.Name);
            var availableConnectionStringsMap = new Dictionary<IConnectionStringIdentity, string>();
            foreach (var item in DefaultName2ConnectionStringMap)
            {
                var connectionStringName = item.Value;
                ConnectionStringSettings connection;
                if (specifiedConnectionStringsMap.TryGetValue(connectionStringName, out connection))
                {
                    availableConnectionStringsMap[item.Key] = connection.ConnectionString;
                }
            }

            ConnectionStringsMap = availableConnectionStringsMap;
        }

        public IReadOnlyDictionary<IConnectionStringIdentity, string> ConnectionStringsMap { get; private set; }
    }
}