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
            var loggerContextEntryProviders =
                    new ILoggerContextEntryProvider[] 
                    {
                        new LoggerContextEntryWcfProvider(LoggerContextKeys.Required.SessionId),
                        new LoggerContextEntryWcfProvider(LoggerContextKeys.Required.UserName),
                        new LoggerContextEntryWcfProvider(LoggerContextKeys.Required.UserIP),
                        new LoggerContextEntryWcfProvider(LoggerContextKeys.Required.UserBrowser),
                        new LoggerContextConstEntryProvider(LoggerContextKeys.Required.SeanceCode, Guid.NewGuid().ToString()),
                        new LoggerContextConstEntryProvider(LoggerContextKeys.Required.Module, settingsContainer.AsSettings<IEnvironmentSettings>().EntryPointName)
                    };

            LoggerContextManager = new LoggerContextManager(loggerContextEntryProviders);
            var logger = Log4NetLoggerBuilder.Use
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
