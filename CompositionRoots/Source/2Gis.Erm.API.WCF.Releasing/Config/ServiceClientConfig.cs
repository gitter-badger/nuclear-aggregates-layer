using System.ServiceModel;

using DoubleGis.Erm.BLCore.API.OrderValidation.Remote;
using DoubleGis.Erm.BLCore.API.OrderValidation.Remote.Settings;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Config;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.API.WCF.Releasing.Config
{
    internal static class ServiceClientConfig
    {
        public static IUnityContainer ConfigureServiceClient(this IUnityContainer container)
        {
            var provider = new ServiceClientSettingsProvider();

            var orderValidationServiceSettings = container.Resolve<IAPIOrderValidationServiceSettings>();

            var wsHttpBinding = BindingConfig.WsHttp.UseTransportSecurity(HttpClientCredentialType.None);

            provider

                // order validation service
                .AddEndpoint<IOrderValidationApplicationService>(wsHttpBinding, orderValidationServiceSettings.BaseUrl, "Validate.svc/Soap");


            return container.RegisterInstance<IServiceClientSettingsProvider>(provider);
        }
    }
}