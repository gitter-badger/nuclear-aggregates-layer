using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Settings;

namespace DoubleGis.Erm.BLCore.API.OrderValidation.Settings
{
    public interface IOrderValidationRulesSettings : ISettings
    {
        IEnumerable<string> RulesExplicitlyDisabledCaching { get; }
        bool UseLegacyCachingMode { get; }
    }
}
