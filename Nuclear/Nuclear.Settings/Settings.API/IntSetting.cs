using System;

namespace Nuclear.Settings.API
{
    public sealed class IntSetting : SettingBase<int>
    {
        #region Overrides of SettingBase<string>

        public IntSetting(string settingName, bool isContainedInAppSetting, int defaultValue, Func<string, string> rawValueEvaluator) 
            : base(settingName, isContainedInAppSetting, defaultValue, rawValueEvaluator)
        {
        }

        protected override SettingEvaluationResult<int> ProcessSettingValue(string rawSettingValue)
        {
            int buffer;
            var result = new SettingEvaluationResult<int>();
            if (int.TryParse(rawSettingValue, out buffer))
            {
                result.Value = buffer;
            }

            return result;
        }

        #endregion
    }
}