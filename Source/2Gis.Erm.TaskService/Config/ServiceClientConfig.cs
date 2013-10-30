using System.ServiceModel;

using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Metadata;
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

            var wsHttpBinding = BindingConfig.WsHttp.UseTransportSecurity(HttpClientCredentialType.None);

            provider

                // identity service
                .AddEndpoint<IIdentityProviderApplicationService>(wsHttpBinding, identityServiceSettings.BaseUrl, "Identity.svc/Soap");


            return container.RegisterInstance<IServiceClientSettingsProvider>(provider);
        }
    }
}