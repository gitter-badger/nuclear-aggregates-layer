using System;
using System.IdentityModel.Policy;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Description;

using DoubleGis.Erm.Platform.API.Core.Settings;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.WCF.Infrastructure.Logging;
using DoubleGis.Erm.Platform.WCF.Infrastructure.ServiceModel.ServiceHost;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Platform.DI.WCF
{
    public abstract class UnityServiceHostFactoryBase<TSettingsContract, TConcreteSettings> : ServiceHostFactory
        where TSettingsContract : IAppSettings
        where TConcreteSettings : class, TSettingsContract, new()
    {
        protected readonly TSettingsContract EntryPointSettingsSettings;
        protected readonly ILoggerContextManager LoggerContextManager;
        protected readonly IUnityContainer DIContainer;

        protected UnityServiceHostFactoryBase(Func<TSettingsContract, ILoggerContextManager, IUnityContainer> unityContainerFactory)
        {
            EntryPointSettingsSettings = new TConcreteSettings();

            var loggerContextEntryProviders =
                new ILoggerContextEntryProvider[] 
                {
                    new LoggerContextEntryWcfProvider(LoggerContextKeys.Required.SessionId),
                    new LoggerContextEntryWcfProvider(LoggerContextKeys.Required.UserName),
                    new LoggerContextEntryWcfProvider(LoggerContextKeys.Required.UserIP),
                    new LoggerContextEntryWcfProvider(LoggerContextKeys.Required.UserBrowser),
                    new LoggerContextConstEntryProvider(LoggerContextKeys.Required.SeanceCode, Guid.NewGuid().ToString()),
                    new LoggerContextConstEntryProvider(LoggerContextKeys.Required.Module, EntryPointSettingsSettings.EntryPointName)
                };

            LoggerContextManager = 
                LogUtils.InitializeLoggingInfrastructure(
                    EntryPointSettingsSettings.LoggingConnectionString(),
                    LogUtils.DefaultLogConfigFileFullPath,
                    loggerContextEntryProviders);

            DIContainer = unityContainerFactory(EntryPointSettingsSettings, LoggerContextManager);
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
