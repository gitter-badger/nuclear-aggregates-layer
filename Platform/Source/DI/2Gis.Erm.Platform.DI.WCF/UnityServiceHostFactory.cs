using System;
using System.IdentityModel.Policy;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;

using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Common.Logging.Log4Net.Config;
using DoubleGis.Erm.Platform.Common.Logging.SystemInfo;
using DoubleGis.Erm.Platform.Common.Settings;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Logging;
using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.ServiceHost;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.WCF
{
    public abstract class UnityServiceHostFactoryBase<TConcreteSettings> : ServiceHostFactory
        where TConcreteSettings : class, ISettingsContainer
    {
        protected readonly ILoggerContextManager LoggerContextManager;
        protected readonly IUnityContainer DIContainer;

        protected UnityServiceHostFactoryBase(
            TConcreteSettings settingsContainer,
            Func<ISettingsContainer, ICommonLog, ILoggerContextManager, IUnityContainer> unityContainerFactory)
        {
            var environmentSettings = settingsContainer.AsSettings<IEnvironmentSettings>();
            var loggerContextEntryProviders =
                    new ILoggerContextEntryProvider[] 
                    {
                        new LoggerContextConstEntryProvider(LoggerContextKeys.Required.Environment, environmentSettings.EnvironmentName),
                        new LoggerContextConstEntryProvider(LoggerContextKeys.Required.EntryPoint, environmentSettings.EntryPointName),
                        new LoggerContextConstEntryProvider(LoggerContextKeys.Required.EntryPointHost, NetworkInfo.ComputerFQDN),
                        new LoggerContextConstEntryProvider(LoggerContextKeys.Required.EntryPointInstanceId, Guid.NewGuid().ToString()),
                        new LoggerContextEntryWcfProvider(LoggerContextKeys.Required.UserAccount),
                        new LoggerContextEntryWcfProvider(LoggerContextKeys.Optional.UserSession),
                        new LoggerContextEntryWcfProvider(LoggerContextKeys.Optional.UserAddress),
                        new LoggerContextEntryWcfProvider(LoggerContextKeys.Optional.UserAgent)
                    };

            LoggerContextManager = new LoggerContextManager(loggerContextEntryProviders);
            var logger = Log4NetLoggerBuilder.Use
                                             .DefaultXmlConfig
                                             .EventLog
                                             .DB(settingsContainer.AsSettings<IConnectionStringSettings>().LoggingConnectionString())
                                             .Build;

            DIContainer = unityContainerFactory(settingsContainer, logger, LoggerContextManager);
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
