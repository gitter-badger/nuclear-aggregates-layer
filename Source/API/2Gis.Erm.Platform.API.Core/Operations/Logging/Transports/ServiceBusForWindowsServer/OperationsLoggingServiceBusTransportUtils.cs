﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging.Transports.ServiceBusForWindowsServer
{
    public static class OperationsLoggingServiceBusTransportUtils
    {
        public static ICollection<ISettingsAspect> IfRequiredUseOperationLogging2ServiceBus(this ICollection<ISettingsAspect> aspects)
        {
            var operationLoggingSettingsAspect = aspects.OfType<OperationLoggingSettingsAspect>().Single();
            if (operationLoggingSettingsAspect.OperationLoggingTargets.HasFlag(LoggingTargets.Queue))
            {
                var connectionStringsAspect = aspects.OfType<ConnectionStringsSettingsAspect>().Single();
                var serviceBusConnectionString = connectionStringsAspect.GetConnectionString(ConnectionStringName.ErmPerformedOperationsServiceBus);
                var aspect = new ServiceBusSenderSettingsAspect(serviceBusConnectionString);
                aspects.Add(aspect);
            }

            return aspects;
        }
    }
}