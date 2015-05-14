using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Operations.Processing;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;

using NuClear.Settings.API;
using NuClear.Storage.ConnectionStrings;

using IConnectionStringSettings = NuClear.Storage.ConnectionStrings.IConnectionStringSettings;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer
{
    public static class PerformedOperationsServiceBusTransportUtils
    {
        public static ICollection<ISettingsAspect> IfRequiredUsePerformedOperationsFromServiceBusAspect(this ICollection<ISettingsAspect> aspects)
        {
            var connectionStringsSettings = aspects.OfType<IConnectionStringSettings>().Single();
            if (!connectionStringsSettings.AllConnectionStrings.ContainsKey(PerformedOperationsServiceBusConnectionStringIdentity.Instance))
            {
                var performedOperationsTransportSettingsAspect = aspects.OfType<PerformedOperationsTransportSettingsAspect>().Single();
                if (performedOperationsTransportSettingsAspect.OperationsTransport == PerformedOperationsTransport.ServiceBus)
                {
                    var msg = string.Format(
                        "Can't get required connection string {0} for specified performed operations transport {1}",
                        PerformedOperationsServiceBusConnectionStringIdentity.Instance, 
                        performedOperationsTransportSettingsAspect.OperationsTransport);
                    throw new InvalidOperationException(msg);
                }
            }
            else
            {
                var connectionStringsAspect = aspects.OfType<ConnectionStringSettingsAspect>().Single();
                var serviceBusConnectionString = connectionStringsAspect.GetConnectionString(PerformedOperationsServiceBusConnectionStringIdentity.Instance);
                var aspect = new ServiceBusReceiverSettingsAspect(serviceBusConnectionString);
                aspects.Add(aspect);
            }

            return aspects;
        }
    }
}