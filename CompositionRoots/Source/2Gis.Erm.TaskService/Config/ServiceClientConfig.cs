using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.MoDi.Remote.Settings;
using DoubleGis.Erm.BLCore.API.MoDi.Remote.WithdrawalInfo;

using DoubleGis.Erm.Platform.WCF.Infrastructure.Config;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.TaskService.Config
{
    internal static class ServiceClientConfig
    {
        public static IUnityContainer ConfigureServiceClient(this IUnityContainer container)
        {
            var provider = new ServiceClientSettingsProvider();

            var withdrawalInfoSvcSettings = container.Resolve<IAPIMoDiServiceSettings>();

            var wsHttpBinding = BindingConfig.WsHttp.UseTransportSecurity(HttpClientCredentialType.None);

            provider

                // modi
                .AddEndpoint<IWithdrawalInfoApplicationService>(wsHttpBinding, withdrawalInfoSvcSettings.BaseUrl, "WithdrawalInfo.svc");

            return container.RegisterInstance<IServiceClientSettingsProvider>(provider);
        }
    }
}