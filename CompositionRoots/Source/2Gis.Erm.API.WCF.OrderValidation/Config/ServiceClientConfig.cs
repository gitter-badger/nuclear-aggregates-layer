using DoubleGis.Erm.Platform.WCF.Infrastructure.Config;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.API.WCF.OrderValidation.Config
{
    public static class ServiceClientConfig
    {
        internal static IUnityContainer ConfigureServiceClient(this IUnityContainer container)
        {
            var provider = new ServiceClientSettingsProvider();

            return container.RegisterInstance<IServiceClientSettingsProvider>(provider);
        }
    }
}