using System.ServiceModel;

using DoubleGis.Erm.BL.API.MoDi.Remote.AccountingSystem;
using DoubleGis.Erm.BL.API.MoDi.Remote.PrintRegional;
using DoubleGis.Erm.BL.API.MoDi.Remote.Reports;
using DoubleGis.Erm.BL.API.MoDi.Remote.WithdrawalInfo;
using DoubleGis.Erm.BL.API.OrderValidation.Remote;
using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Metadata;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Config;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.WCF.BasicOperations.Config
{
    internal static class ServiceClientConfig
    {
        public static IUnityContainer ConfigureServiceClient(this IUnityContainer container)
        {
            var provider = new ServiceClientSettingsProvider();

            var identityServiceSettings = container.Resolve<IAPIIdentityServiceSettings>();
            var orderValidationServiceSettings = container.Resolve<IAPIOrderValidationServiceSettings>();
            var moDiServiceSettings = container.Resolve<IAPIMoDiServiceSettings>();

            var wsHttpBinding = BindingConfig.WsHttp.UseTransportSecurity(HttpClientCredentialType.None);
            var basicHttpBinding = BindingConfig.BasicHttp.UseTransportSecurity(HttpClientCredentialType.None).MaxReceivedMessageSize(20000000);

            provider

                // order validation service
                .AddEndpoint<IOrderValidationApplicationService>(wsHttpBinding, orderValidationServiceSettings.BaseUrl, "Validate.svc/Soap")

                // identity service
                .AddEndpoint<IIdentityProviderApplicationService>(wsHttpBinding, identityServiceSettings.BaseUrl, "Identity.svc/Soap")

                // modi service
                .AddEndpoint<IPrintRegionalApplicationService>(wsHttpBinding, moDiServiceSettings.BaseUrl, "PrintRegional.svc")
                .AddEndpoint<IAccountingSystemApplicationService>(wsHttpBinding, moDiServiceSettings.BaseUrl, "AccountingSystem.svc")
                .AddEndpoint<IWithdrawalInfoApplicationService>(wsHttpBinding, moDiServiceSettings.BaseUrl, "WithdrawalInfo.svc")
                .AddEndpoint<IReportsApplicationService>(basicHttpBinding, moDiServiceSettings.BaseUrl, "Reports.svc");

            return container.RegisterInstance<IServiceClientSettingsProvider>(provider);
        }
    }
}