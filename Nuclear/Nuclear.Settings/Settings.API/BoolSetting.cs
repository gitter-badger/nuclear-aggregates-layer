using System;

namespace DoubleGis.Erm.Platform.Common.Settings
{
    public sealed class BoolSetting : SettingBase<bool>
    {
        #region Overrides of SettingBase<string>

        public BoolSetting(string settingName, bool isContainedInAppSetting, bool defaultValue, Func<string, string> rawValueEvaluator) 
            : base(settingName, isContainedInAppSetting, defaultValue, rawValueEvaluator)
        {
        }

        protected override SettingEvaluationResult<bool> ProcessSettingValue(string rawSettingValue)
        {
            bool buffer;
            var result = new SettingEvaluationResult<bool>();
            if (bool.TryParse(rawSettingValue, out buffer))
            {
                result.Value = buffer;
            }

            return result;
        }

        #endregion
    }
}