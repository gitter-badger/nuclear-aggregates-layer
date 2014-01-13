using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

using DoubleGis.Erm.BLCore.MsCRM.Plugins.ErmServiceReference;

namespace DoubleGis.Erm.BLCore.MsCRM.Plugins
{
    public class ErmWcfServiceHelper
    {
        public static void SendRequest(String ermServiceAddress, Action<IMsCrm2ErmApplicationService> serviceAction)
        {
            var defaultBinding = CreateDefaultWcfBinding();
            var ermServiceEndpoint = new EndpointAddress(new Uri(ermServiceAddress));

            // do not use "using" statement, this is wrong then calling wcf service
            var ermService = new MsCrm2ErmApplicationServiceClient(defaultBinding, ermServiceEndpoint);
            try
            {
                serviceAction(ermService);
                ermService.Close();
            }
            catch (FaultException ex)
            {
                ermService.Abort();
                EventLog.WriteEntry("MSCRMAsyncService", ex.ToString(), EventLogEntryType.Error);
                //throw new WorkflowTerminatedException("Error then calling ErmService", ex);
            }
            catch (CommunicationException ex)
            {
                ermService.Abort();
                EventLog.WriteEntry("MSCRMAsyncService", ex.ToString(), EventLogEntryType.Error);
                //throw new WorkflowTerminatedException("Error then calling ErmService", ex);
            }
            catch (TimeoutException ex)
            {
                ermService.Abort();
                EventLog.WriteEntry("MSCRMAsyncService", ex.ToString(), EventLogEntryType.Error);
                //throw new WorkflowTerminatedException("Error then calling ErmService", ex);
            }

            //return ActivityExecutionStatus.Closed;
        }

        private static Binding CreateDefaultWcfBinding()
        {
            var wsHttpBinding = new WSHttpBinding
            {
                CloseTimeout = TimeSpan.FromMinutes(1),
                OpenTimeout = TimeSpan.FromMinutes(1),
                ReceiveTimeout = TimeSpan.FromMinutes(1),
                SendTimeout = TimeSpan.FromMinutes(1),
                BypassProxyOnLocal = false,
                TransactionFlow = false,
                HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                MaxBufferPoolSize = 524288,
                MaxReceivedMessageSize = 65536,
                MessageEncoding = WSMessageEncoding.Text,
                TextEncoding = Encoding.UTF8,
                UseDefaultWebProxy = true,
                AllowCookies = false
            };

            // todo: remove this then implement security
            wsHttpBinding.Security.Mode = SecurityMode.None;

            return wsHttpBinding;
        }
    }
}
