using System;

namespace Nuclear.Settings.API
{
    public sealed class DecimalSetting : SettingBase<decimal>
    {
        #region Overrides of SettingBase<string>

        public DecimalSetting(string settingName, bool isContainedInAppSetting, decimal defaultValue, Func<string, string> rawValueEvaluator) 
            : base(settingName, isContainedInAppSetting, defaultValue, rawValueEvaluator)
        {
        }

        protected override SettingEvaluationResult<decimal> ProcessSettingValue(string rawSettingValue)
        {
            decimal buffer;
            var result = new SettingEvaluationResult<decimal>();
            if (decimal.TryParse(rawSettingValue, out buffer))
            {
                result.Value = buffer;
            }

            return result;
        }

        #endregion
    }
}