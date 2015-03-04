using System.Collections.Generic;
using System.Linq;

using Nuclear.Settings;
using Nuclear.Settings.API;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Settings
{
    public sealed class OrderValidationCachingSettingsAspect : ISettingsAspect, IOrderValidationCachingSettings
    {
        private readonly StringSetting _rulesExplicitlyDisabledCaching = ConfigFileSetting.String.Optional("ValidationRulesDisabledCaching", string.Empty);
        private readonly BoolSetting _useLegacyCachingMode = ConfigFileSetting.Bool.Optional("UseLegacyCachingMode", false);

        IEnumerable<string> IOrderValidationCachingSettings.RulesExplicitlyDisabledCaching
        {
            get 
            { 
                return string.IsNullOrEmpty(_rulesExplicitlyDisabledCaching.Value) 
                           ? Enumerable.Empty<string>() 
                           : _rulesExplicitlyDisabledCaching.Value.Split(';'); 
            }
        }

        bool IOrderValidationCachingSettings.UseLegacyCachingMode 
        {
            get { return _useLegacyCachingMode.Value; }
        }
    }
}