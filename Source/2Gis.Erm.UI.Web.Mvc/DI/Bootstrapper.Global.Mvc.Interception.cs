using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Model;

using Microsoft.Practices.Unity.InterceptionExtension;

namespace DoubleGis.Erm.UI.Web.Mvc.DI
{
    internal static partial class BootstrapperMvc
    {
        private static readonly IReadOnlyDictionary<BusinessModel, Func<Interception, Interception>>
            ConfiguratorsMap = new Dictionary<BusinessModel, Func<Interception, Interception>>
                {
                    { BusinessModel.Russia, ConfigureRussiaSpecific },
                    { BusinessModel.Cyprus, ConfigureCyprusSpecific },
                    { BusinessModel.Czech, ConfigureCzechSpecific },
                    { BusinessModel.Chile, ConfigureChileSpecific },
                    { BusinessModel.Ukraine, ConfigureUkraineSpecific },
                    { BusinessModel.Emirates, ConfigureEmiratesSpecific },
                };

        public static Interception ConfigureGlobalMvcInterception(this Interception interception, IGlobalizationSettings globalizationSettings)
        {
            var businessModelSpecificConfigurator = ConfiguratorsMap[globalizationSettings.BusinessModel];
            return businessModelSpecificConfigurator(interception);
        }
    }
}