using System;

namespace DoubleGis.Erm.Platform.Common.Settings
{
    public sealed class EnumSetting<TSetting> : SettingBase<TSetting>
        where TSetting : struct, IConvertible
    {
        public EnumSetting(string settingName, bool isContainedInAppSetting, TSetting defaultValue, Func<string, string> rawValueEvaluator) 
            : base(settingName, isContainedInAppSetting, defaultValue, rawValueEvaluator)
        {
        }

        protected override SettingEvaluationResult<TSetting> ProcessSettingValue(string rawSettingValue)
        {
            TSetting buffer;
            var result = new SettingEvaluationResult<TSetting>();
            if (Enum.TryParse(rawSettingValue, out buffer))
            {
                result.Value = buffer;
            }

            return result;
        }
    }
}