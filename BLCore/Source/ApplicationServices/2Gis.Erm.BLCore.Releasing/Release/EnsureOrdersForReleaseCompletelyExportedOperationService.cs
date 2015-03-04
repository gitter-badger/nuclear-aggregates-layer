using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.BLCore.API.Releasing.Releases;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.ServiceBusBroker;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Release;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Proxy;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    public sealed class EnsureOrdersForReleaseCompletelyExportedOperationService : IEnsureOrdersForReleaseCompletelyExportedOperationService
    {
        private const string OrdersFlowName = "flowOrders";

        private readonly IEnsureOrderExportedStrategyContainer _ensureOrderExportedStrategyContainer;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IIntegrationSettings _integrationSettings;
        private readonly IClientProxyFactory _clientProxyFactory;
        private readonly ITracer _logger;

        public EnsureOrdersForReleaseCompletelyExportedOperationService(IEnsureOrderExportedStrategyContainer ensureOrderExportedStrategyContainer,
                                                                        IOperationScopeFactory scopeFactory,
                                                                        IIntegrationSettings integrationSettings,
                                                                        IClientProxyFactory clientProxyFactory,
                                                                        ITracer logger)
        {
            _ensureOrderExportedStrategyContainer = ensureOrderExportedStrategyContainer;
            _scopeFactory = scopeFactory;
            _integrationSettings = integrationSettings;
            _clientProxyFactory = clientProxyFactory;
            _logger = logger;
        }

        public bool IsExported(long releaseId, long organizationUnitId, int organizationUnitDgppId, TimePeriod period, bool isBeta)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<EnsureOrdersForReleaseCompletelyExportedIdentity>())
            {
                _logger.InfoFormat("Starting ensure process that all orders for release are exported. " +
                                     "Release detail: id = {0}, organization unit id {1}, period {2}, type - {3}",
                                     releaseId,
                                     organizationUnitId,
                                     period,
                                     isBeta ? "beta" : "final");

                var allRequiredOrdersExported = _ensureOrderExportedStrategyContainer.Strategies.All(s => s.IsExported(releaseId,
                                                                                                                       organizationUnitId,
                                                                                                                       organizationUnitDgppId,
                                                                                                                       period,
                                                                                                                       isBeta));
                if (!allRequiredOrdersExported)
                {
                    _logger.InfoFormat("Ensure process that all orders for release are exported finished. Not all required orders are exported. " +
                                         "Release detail: id = {0}, organization unit id {1}, period {2}, type - {3}",
                                         releaseId,
                                         organizationUnitId,
                                         period,
                                         isBeta ? "beta" : "final");

                    scope.Complete();

                    return false;
                }

                _logger.InfoFormat("Ensured that all orders for release are exported. Trying notify external listeners about that fact. " +
                                     "Release detail: id = {0}, organization unit id {1}, period {2}, type - {3}",
                                     releaseId,
                                     organizationUnitId,
                                     period,
                                     isBeta ? "beta" : "final");

                NotifyListenersThatAllOrdersForReleaseAreExported(releaseId, organizationUnitDgppId, period);

                _logger.InfoFormat("Ensured that all orders for release are exported. Notification for external listeners was sended. " +
                                     "Release detail: id = {0}, organization unit id {1}, period {2}, type - {3}",
                                     releaseId,
                                     organizationUnitId,
                                     period,
                                     isBeta ? "beta" : "final");

                scope.Complete();
            }

            return true;
        }

        private static XElement CreateSyncronizationMessage(long releaseInfoId, int organizationUnitDgppId, TimePeriod period)
        {
            return new XElement("ReleaseStartedEvent",
                                new XAttribute("ReleaseInfoCode", releaseInfoId),
                                new XAttribute("StartDate", period.Start),
                                new XAttribute("EndDate", period.End),
                                new XAttribute("BranchCode", organizationUnitDgppId));
        }

        private void NotifyListenersThatAllOrdersForReleaseAreExported(long releaseInfoId, int organizationUnitDgppId, TimePeriod period)
        {
            // TODO {m.pashuk, 27.01.2013}: Передавать название endpoint-а через конфигурационный объект (_integrationSettings?)
            var brokerApiSenderProxy = _clientProxyFactory.GetClientProxy<IBrokerApiSender>("NetTcpBinding_IBrokerApiSender");

            brokerApiSenderProxy.Execute(brokerApiSender =>
                {
                    brokerApiSender.BeginSending(_integrationSettings.IntegrationApplicationName, OrdersFlowName);

                    var syncMessage = CreateSyncronizationMessage(releaseInfoId, organizationUnitDgppId, period);
                    brokerApiSender.SendDataObject(syncMessage.ToString());

                    brokerApiSender.Commit();
                    brokerApiSender.EndSending();
                });
        }
    }
}
