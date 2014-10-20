using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Settings
{
    public sealed class OrderValidationRulesSettingsAspect : ISettingsAspect, IOrderValidationRulesSettings
    {
        private readonly StringSetting _rulesExplicitlyDisabledCaching = ConfigFileSetting.String.Optional("ValidationRulesDisabledCaching", string.Empty);
        private readonly BoolSetting _useLegacyCachingMode = ConfigFileSetting.Bool.Optional("UseLegacyCachingMode", false);

        IEnumerable<string> IOrderValidationRulesSettings.RulesExplicitlyDisabledCaching
        {
            get 
            { 
                return string.IsNullOrEmpty(_rulesExplicitlyDisabledCaching.Value) 
                           ? Enumerable.Empty<string>() 
                           : _rulesExplicitlyDisabledCaching.Value.Split(';'); 
            }
        }

        bool IOrderValidationRulesSettings.UseLegacyCachingMode 
        {
            get { return _useLegacyCachingMode.Value; }
        }
    }
}