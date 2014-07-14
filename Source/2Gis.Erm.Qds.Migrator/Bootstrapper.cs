﻿using DoubleGis.Erm.BLCore.DI.Config;
using DoubleGis.Erm.BLQuerying.DI.Config;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.Settings.Environments;
using DoubleGis.Erm.Platform.Common.Settings;
using DoubleGis.Erm.Platform.DI.Common.Config;
using DoubleGis.Erm.Qds.API.Core.Settings;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.Qds.Migrator
{
    internal static class Bootstrapper
    {
        public static IUnityContainer ConfigureUnity(this IUnityContainer container, ISettingsContainer settingsContainer)
        {
            container.InitializeDIInfrastructure();

            return container
                        .ConfigureSettingsAspects(settingsContainer)
                        .ConfigureDAL(EntryPointSpecificLifetimeManagerFactory,
                                      settingsContainer.AsSettings<IEnvironmentSettings>(),
                                      settingsContainer.AsSettings<IConnectionStringSettings>())
                        .ConfigureQds(EntryPointSpecificLifetimeManagerFactory, settingsContainer.AsSettings<INestSettings>());
        }

        private static LifetimeManager EntryPointSpecificLifetimeManagerFactory()
        {
            return Lifetime.Singleton;
        }
    }
}