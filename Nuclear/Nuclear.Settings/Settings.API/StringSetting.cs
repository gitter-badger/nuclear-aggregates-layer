using System;

namespace DoubleGis.Erm.Platform.Common.Settings
{
    public sealed class StringSetting : SettingBase<string>
    {
        #region Overrides of SettingBase<string>

        public StringSetting(string settingName, bool isContainedInAppSetting, string defaultValue, Func<string, string> rawValueEvaluator) 
            : base(settingName, isContainedInAppSetting, defaultValue, rawValueEvaluator)
        {
        }

        protected override SettingEvaluationResult<string> ProcessSettingValue(string rawSettingValue)
        {
            return new SettingEvaluationResult<string> { Value = rawSettingValue };
        }

        #endregion
    }
}