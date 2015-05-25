using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.MoDi.Remote.Settings;
using DoubleGis.Erm.BLCore.API.MoDi.Remote.WithdrawalInfo;
using DoubleGis.Erm.BLCore.API.OrderValidation.Remote;
using DoubleGis.Erm.BLCore.API.OrderValidation.Remote.Settings;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Config;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.API.WCF.Operations.Special.Config
{
    internal static class ServiceClientConfig
    {
        public static IUnityContainer ConfigureServiceClient(this IUnityContainer container)
        {
            var provider = new ServiceClientSettingsProvider();
             
            var orderValidationServiceSettings = container.Resolve<IAPIOrderValidationServiceSettings>();
            var moDiServiceSettings = container.Resolve<IAPIMoDiServiceSettings>();

            var wsHttpBinding = BindingConfig.WsHttp.UseTransportSecurity(HttpClientCredentialType.None);

            provider
                // order validation service
                .AddEndpoint<IOrderValidationApplicationService>(wsHttpBinding, orderValidationServiceSettings.BaseUrl, "Validate.svc/Soap")

                // modi service
                .AddEndpoint<IWithdrawalInfoApplicationService>(wsHttpBinding, moDiServiceSettings.BaseUrl, "WithdrawalInfo.svc");

            return container.RegisterInstance<IServiceClientSettingsProvider>(provider);
        }
    }
}