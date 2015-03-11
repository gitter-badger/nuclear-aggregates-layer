using System;
using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.MoDi.Remote.Reports;
using DoubleGis.Erm.BLCore.API.MoDi.Remote.Settings;
using DoubleGis.Erm.BLCore.API.MoDi.Remote.WithdrawalInfo;
using DoubleGis.Erm.BLCore.API.OrderValidation.Remote;
using DoubleGis.Erm.BLCore.API.OrderValidation.Remote.Settings;
using DoubleGis.Erm.Platform.API.Metadata;
using DoubleGis.NuClear.IdentityService.Client.Settings;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Config;

using Microsoft.Practices.Unity;

using ServiceClientSettingsProvider = DoubleGis.Erm.Platform.WCF.Infrastructure.Config.ServiceClientSettingsProvider;

namespace DoubleGis.Erm.UI.Web.Mvc.Config
{
    internal static class ServiceClientConfig
    {
        public static IUnityContainer ConfigureServiceClient(this IUnityContainer container)
        {
            var provider = new ServiceClientSettingsProvider();

            var identityServiceSettings = container.Resolve<IIdentityServiceClientSettings>();
            var orderValidationServiceSettings = container.Resolve<IAPIOrderValidationServiceSettings>();
            var moDiServiceSettings = container.Resolve<IAPIMoDiServiceSettings>();

            var wsHttpBinding = BindingConfig.WsHttp.UseTransportSecurity(HttpClientCredentialType.None)
                                             .Timeouts(TimeSpan.FromHours(1), TimeSpan.FromHours(1))
                                             .MaxReceivedMessageSize(10000000);

            var basicHttpBinding = BindingConfig.BasicHttp.UseTransportSecurity(HttpClientCredentialType.None).MaxReceivedMessageSize(20000000);



            provider

                // order validation service
                .AddEndpoint<IOrderValidationApplicationService>(wsHttpBinding, orderValidationServiceSettings.BaseUrl, "Validate.svc/Soap")

                // modi service
                .AddEndpoint<IWithdrawalInfoApplicationService>(wsHttpBinding, moDiServiceSettings.BaseUrl, "WithdrawalInfo.svc")
                .AddEndpoint<IReportsApplicationService>(basicHttpBinding, moDiServiceSettings.BaseUrl, "Reports.svc");

            return container.RegisterInstance<IServiceClientSettingsProvider>(provider);
        }
    }
}