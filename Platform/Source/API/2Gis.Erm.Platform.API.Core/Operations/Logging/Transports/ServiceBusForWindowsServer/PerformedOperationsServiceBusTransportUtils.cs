using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Operations.Processing;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;

using NuClear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer
{
    public static class PerformedOperationsServiceBusTransportUtils
    {
        public static ICollection<ISettingsAspect> IfRequiredUsePerformedOperationsFromServiceBusAspect(this ICollection<ISettingsAspect> aspects)
        {
            var connectionStringsSettings = aspects.OfType<IConnectionStringSettings>().Single();
            if (!connectionStringsSettings.AllConnections.ContainsKey(ConnectionStringName.ErmPerformedOperationsServiceBus))
            {
                var performedOperationsTransportSettingsAspect = aspects.OfType<PerformedOperationsTransportSettingsAspect>().Single();
                if (performedOperationsTransportSettingsAspect.OperationsTransport == PerformedOperationsTransport.ServiceBus)
                {
                    var msg = string.Format(
                        "Can't get required connection string {0} for specified performed operations transport {1}", 
                        ConnectionStringName.ErmPerformedOperationsServiceBus, 
                        performedOperationsTransportSettingsAspect.OperationsTransport);
                    throw new InvalidOperationException(msg);
                }
            }
            else
            {
                var connectionStringsAspect = aspects.OfType<ConnectionStringsSettingsAspect>().Single();
                var serviceBusConnectionString = connectionStringsAspect.GetConnectionString(ConnectionStringName.ErmPerformedOperationsServiceBus);
                var aspect = new ServiceBusReceiverSettingsAspect(serviceBusConnectionString);
                aspects.Add(aspect);
            }

            return aspects;
        }
    }
}