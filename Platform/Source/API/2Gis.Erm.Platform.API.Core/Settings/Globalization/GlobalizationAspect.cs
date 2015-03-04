using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using DoubleGis.Erm.Platform.Model;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using Nuclear.Settings;
using Nuclear.Settings.API;

namespace DoubleGis.Erm.Platform.API.Core.Settings.Globalization
{
    public sealed class GlobalizationAspect : ISettingsAspect, IGlobalizationSettings
    {
        private readonly IntSetting _significantDigitsNumber = ConfigFileSetting.Int.Required("SignificantDigitsNumber");

        public GlobalizationAspect(IEnumerable<Type> supportedBusinessModelIndicators)
        {
            var targetBusinessModel = ConfigFileSetting.Enum.Required<BusinessModel>("BusinessModel");
            var targetBusinessModelInfo = supportedBusinessModelIndicators
                .Select(supportedBusinessModelIndicator =>
                        new
                            {
                                ModelIndicator = supportedBusinessModelIndicator,
                                GlobalSpec = supportedBusinessModelIndicator.GetCustomAttribute<GlobalizationSpecsAttribute>()
                            })
                .Single(x => x.GlobalSpec != null && x.GlobalSpec.BusinessModel == targetBusinessModel.Value);

            BusinessModel = targetBusinessModelInfo.GlobalSpec.BusinessModel;
            BusinessModelIndicator = targetBusinessModelInfo.ModelIndicator;
            DefaultCulture = targetBusinessModelInfo.GlobalSpec.CulturesSequence.Last();
            ApplicationCulture = targetBusinessModelInfo.GlobalSpec.CulturesSequence.First();
        }

        public BusinessModel BusinessModel { get; private set; }
        public Type BusinessModelIndicator { get; private set; }
        
        public int SignificantDigitsNumber
        {
            get { return _significantDigitsNumber.Value; }
        }

        public CultureInfo[] SupportedCultures
        {
            get { return LocalizationSettings.SupportedCultures; }
        }

        public CultureInfo DefaultCulture { get; private set; }
        public CultureInfo ApplicationCulture { get; private set; }
    }
}
