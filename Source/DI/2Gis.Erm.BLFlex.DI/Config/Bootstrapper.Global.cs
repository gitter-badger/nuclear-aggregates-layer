using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Globalization;
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

        public static IUnityContainer ConfigureGlobal(this IUnityContainer container, IGlobalizationSettings globalizationSettings)
        {
            var businessModelSpecificConfigurator = ConfiguratorsMap[globalizationSettings.BusinessModel];
            return businessModelSpecificConfigurator(container, globalizationSettings);
        }
    }
}
