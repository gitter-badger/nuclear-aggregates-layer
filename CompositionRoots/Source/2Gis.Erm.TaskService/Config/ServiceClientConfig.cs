using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.MoDi.Remote.Settings;
using DoubleGis.Erm.BLCore.API.MoDi.Remote.WithdrawalInfo;
using DoubleGis.Erm.Platform.API.Metadata;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Config;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.TaskService.Config
{
    internal static class ServiceClientConfig
    {
        public static IUnityContainer ConfigureServiceClient(this IUnityContainer container)
        {
            var provider = new ServiceClientSettingsProvider();

            var identityServiceSettings = container.Resolve<IAPIIdentityServiceSettings>();
            var withdrawalInfoSvcSettings = container.Resolve<IAPIMoDiServiceSettings>();

            var wsHttpBinding = BindingConfig.WsHttp.UseTransportSecurity(HttpClientCredentialType.None);

            provider

                // identity service
                .AddEndpoint<IIdentityProviderApplicationService>(wsHttpBinding, identityServiceSettings.BaseUrl, "Identity.svc/Soap")

                // modi
                .AddEndpoint<IWithdrawalInfoApplicationService>(wsHttpBinding, withdrawalInfoSvcSettings.BaseUrl, "WithdrawalInfo.svc");

            return container.RegisterInstance<IServiceClientSettingsProvider>(provider);
        }
    }
}