using System;

namespace Nuclear.Settings.API
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

            if (Attribute.IsDefined(typeof(TSetting), typeof(FlagsAttribute)))
            {
                if (TryParseFlagsEnum(rawSettingValue, out buffer))
                {
                    result.Value = buffer;
                }
            }
            else if (Enum.TryParse(rawSettingValue, out buffer))
            {
                result.Value = buffer;
            }

            return result;
        }

        private bool TryParseFlagsEnum(string rawValue, out TSetting parsed)
        {
            parsed = default(TSetting);

            if (string.IsNullOrWhiteSpace(rawValue))
            {
                return false;
            }

            ulong? accum = null;
            var elements = rawValue.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var element in elements)
            {
                TSetting buffer;
                if (Enum.TryParse(element, out buffer))
                {
                    ulong value = Convert.ToUInt64(buffer);
                    accum = (accum.HasValue ? accum.Value : 0) | value;
                }
            }

            if (accum.HasValue)
            {
                parsed = (TSetting)Enum.ToObject(typeof(TSetting), accum);
                return true;
            }

            return false;
        }
    }
}