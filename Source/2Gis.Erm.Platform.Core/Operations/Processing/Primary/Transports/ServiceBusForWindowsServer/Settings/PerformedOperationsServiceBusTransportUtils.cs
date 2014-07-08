using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Operations.Processing;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.Core.Operations.Processing.Primary.Transports.ServiceBusForWindowsServer.Settings
{
    public static class PerformedOperationsServiceBusTransportUtils
    {
        public static ICollection<ISettingsAspect> IfRequiredUsePerformedOperationsFromServiceBusAspect(this ICollection<ISettingsAspect> aspects)
        {
            var performedOperationsTransportSettingsAspect = aspects.OfType<PerformedOperationsTransportSettingsAspect>().Single();
            if (performedOperationsTransportSettingsAspect.OperationsTransport == PerformedOperationsTransport.ServiceBus)
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