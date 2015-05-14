using System;
using System.IdentityModel.Policy;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Logging;
using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.ServiceHost;

using Microsoft.Practices.Unity;

using NuClear.Settings.API;
using NuClear.Storage.ConnectionStrings;
using NuClear.Tracing.API;
using NuClear.Tracing.Environment;
using NuClear.Tracing.Log4Net;
using NuClear.Tracing.Log4Net.Config;

namespace DoubleGis.Erm.Platform.DI.WCF
{
    public abstract class UnityServiceHostFactoryBase<TConcreteSettings> : ServiceHostFactory
        where TConcreteSettings : class, ISettingsContainer
    {
        protected readonly ITracerContextManager TracerContextManager;
        protected readonly IUnityContainer DIContainer;

        protected UnityServiceHostFactoryBase(
            TConcreteSettings settingsContainer,
            Func<ISettingsContainer, ITracer, ITracerContextManager, IUnityContainer> unityContainerFactory)
        {
            var environmentSettings = settingsContainer.AsSettings<IEnvironmentSettings>();
            var tracerContextEntryProviders =
                    new ITracerContextEntryProvider[]
                    {
                        new TracerContextConstEntryProvider(TracerContextKeys.Required.Environment, environmentSettings.EnvironmentName),
                        new TracerContextConstEntryProvider(TracerContextKeys.Required.EntryPoint, environmentSettings.EntryPointName),
                        new TracerContextConstEntryProvider(TracerContextKeys.Required.EntryPointHost, NetworkInfo.ComputerFQDN),
                        new TracerContextConstEntryProvider(TracerContextKeys.Required.EntryPointInstanceId, Guid.NewGuid().ToString()),
                        new TracerContextEntryWcfProvider(TracerContextKeys.Required.UserAccount),
                        new TracerContextEntryWcfProvider(TracerContextKeys.Optional.UserSession),
                        new TracerContextEntryWcfProvider(TracerContextKeys.Optional.UserAddress),
                        new TracerContextEntryWcfProvider(TracerContextKeys.Optional.UserAgent)
                    };

            TracerContextManager = new TracerContextManager(tracerContextEntryProviders);
            var tracer = Log4NetTracerBuilder.Use
                                             .DefaultXmlConfig
                                             .EventLog
                                             .DB(settingsContainer.AsSettings<IConnectionStringSettings>().LoggingConnectionString())
                                             .Build;

            DIContainer = unityContainerFactory(settingsContainer, tracer, TracerContextManager);
        }

        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            var serviceHost = new CustomAuthorizationServiceHost(DIContainer.ResolveAll<IAuthorizationPolicy>().ToArray(),
                                                                 DIContainer.Resolve<IServiceBehavior>(),
                                                                 serviceType,
                                                                 baseAddresses);

            return serviceHost;
        }
    }
}
