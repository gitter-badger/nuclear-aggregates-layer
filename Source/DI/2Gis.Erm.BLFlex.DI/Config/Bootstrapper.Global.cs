using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Model;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLFlex.DI.Config
{
    public static partial class Bootstrapper
    {
        private static readonly IReadOnlyDictionary<BusinessModel, Func<IUnityContainer, IGlobalizationSettings, IUnityContainer>>
            ConfiguratorsMap = new Dictionary<BusinessModel, Func<IUnityContainer, IGlobalizationSettings, IUnityContainer>>
                {
                    { BusinessModel.Russia, ConfigureRussiaSpecific },
                    { BusinessModel.Cyprus, ConfigureCyprusSpecific },
                    { BusinessModel.Czech, ConfigureCzechSpecific },
                    { BusinessModel.Chile, ConfigureChileSpecific },
                };

        private static readonly IReadOnlyDictionary<BusinessModel, Action>
            ConfiguratorsListingMap = new Dictionary<BusinessModel, Action>
                {
                    { BusinessModel.Cyprus, ConfigureCyprusListingMetadata },
                    { BusinessModel.Czech, ConfigureCzechListingMetadata },
                    { BusinessModel.Chile, ConfigureChileListingMetadata },
                };

        public static IUnityContainer ConfigureGlobal(this IUnityContainer container, IGlobalizationSettings globalizationSettings)
        {
            var businessModelSpecificConfigurator = ConfiguratorsMap[globalizationSettings.BusinessModel];
            return businessModelSpecificConfigurator(container, globalizationSettings);
        }

        // HACK дико извиняюсь, но пока метаданные для листинга регистрируются только так, скоро поправим
        public static void ConfigureGlobalListing(IGlobalizationSettings globalizationSettings)
        {
            Action action;
            if (ConfiguratorsListingMap.TryGetValue(globalizationSettings.BusinessModel, out action))
            {
                action();
            }
        }
    }
}
