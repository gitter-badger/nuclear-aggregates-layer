using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;

using NuClear.Settings.API;
using NuClear.Storage.ConnectionStrings;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer
{
    public static class OperationsLoggingServiceBusTransportUtils
    {
        public static ICollection<ISettingsAspect> IfRequiredUseOperationLogging2ServiceBus(this ICollection<ISettingsAspect> aspects)
        {
            var operationLoggingSettingsAspect = aspects.OfType<OperationLoggingSettingsAspect>().Single();
            if (operationLoggingSettingsAspect.OperationLoggingTargets.HasFlag(LoggingTargets.Queue))
            {
                var connectionStringsAspect = aspects.OfType<ConnectionStringSettingsAspect>().Single();
                var serviceBusConnectionString = connectionStringsAspect.GetConnectionString(PerformedOperationsServiceBusConnectionStringIdentity.Instance);
                var aspect = new ServiceBusSenderSettingsAspect(serviceBusConnectionString);
                aspects.Add(aspect);
            }

            return aspects;
        }
    }
}