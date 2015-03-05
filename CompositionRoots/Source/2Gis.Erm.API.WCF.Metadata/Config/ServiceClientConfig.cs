using System;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.MoDi.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Operations.Special.Remote.Settings;
using DoubleGis.Erm.BLCore.API.OrderValidation.Remote.Settings;
using DoubleGis.Erm.BLCore.API.Releasing.Remote.Release.Settings;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Config;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.API.WCF.Metadata.Config
{
    public static class ServiceClientConfig
    {
        public static IUnityContainer ConfigureServiceClient(this IUnityContainer container)
        {
            var discoveryEndpointContainer = new DiscoveryEndpointContainer();

            var releasingSvcSettings = container.Resolve<IAPIReleasingServiceSettings>();
            var modiSvcSettings = container.Resolve<IAPIMoDiServiceSettings>();
            var operationsSvcSettings = container.Resolve<IAPIOperationsServiceSettings>();
            var specialOperationsSvcSettings = container.Resolve<IAPISpecialOperationsServiceSettings>();
            var orderValidationSvcSettings = container.Resolve<IAPIOrderValidationServiceSettings>();

            var binding = BindingConfig.WsHttp
                                       .UseTransportSecurity(HttpClientCredentialType.None)
                                       .Timeouts(TimeSpan.FromMinutes(10), TimeSpan.FromMinutes(10));

            discoveryEndpointContainer.AddEndpoint("ActionsHistory", binding, operationsSvcSettings.BaseUrl, "ActionsHistory.svc")
                                      .AddEndpoint("Activate", binding, operationsSvcSettings.BaseUrl, "Activate.svc")
                                      .AddEndpoint("Append", binding, operationsSvcSettings.BaseUrl, "Append.svc")
                                      .AddEndpoint("Assign", binding, operationsSvcSettings.BaseUrl, "Assign.svc")
                                      .AddEndpoint("ChangeClient", binding, operationsSvcSettings.BaseUrl, "ChangeClient.svc")
                                      .AddEndpoint("ChangeTerritory", binding, operationsSvcSettings.BaseUrl, "ChangeTerritory.svc")
                                      .AddEndpoint("CheckForDebts", binding, operationsSvcSettings.BaseUrl, "CheckForDebts.svc")
                                      .AddEndpoint("Deactivate", binding, operationsSvcSettings.BaseUrl, "Deactivate.svc")
                                      .AddEndpoint("Delete", binding, operationsSvcSettings.BaseUrl, "Delete.svc")
                                      .AddEndpoint("Disqualify", binding, operationsSvcSettings.BaseUrl, "Disqualify.svc")
                                      .AddEndpoint("Qualify", binding, operationsSvcSettings.BaseUrl, "Qualify.svc")

                                      .AddEndpoint("Validate", binding, orderValidationSvcSettings.BaseUrl, "Validate.svc")

                                      .AddEndpoint("AccountingSystem", binding, modiSvcSettings.BaseUrl, "AccountingSystem.svc")
                                      .AddEndpoint("Reports", binding, modiSvcSettings.BaseUrl, "Reports.svc/Discovery")
                                      .AddEndpoint("WithdrawalInfo", binding, modiSvcSettings.BaseUrl, "WithdrawalInfo.svc")
                                      .AddEndpoint("PrintRegional", binding, modiSvcSettings.BaseUrl, "PrintRegional.svc")

                                      .AddEndpoint("Release", binding, releasingSvcSettings.BaseUrl, "Release.svc")

                                      .AddEndpoint("Calculate", binding, specialOperationsSvcSettings.BaseUrl, "Calculate.svc")

                                      .AddEndpoint("Cancel", binding, operationsSvcSettings.BaseUrl, "Cancel.svc");

            return container.RegisterInstance<IDiscoveryEndpointContainer>(discoveryEndpointContainer);
        }
    }
}