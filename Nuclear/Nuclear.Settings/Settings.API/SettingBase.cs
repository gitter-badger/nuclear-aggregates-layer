using System;
using System.Configuration;

namespace DoubleGis.Erm.Platform.Common.Settings
{
    public abstract class SettingBase<TSetting>
    {
        private readonly string _settingName;
        private readonly bool _isContainedInAppSetting;
        private readonly TSetting _defaultValue;
        private readonly Func<string, string> _rawValueEvaluator;

        private readonly Lazy<TSetting> _valueLoader;

        protected SettingBase(
            string settingName,
            bool isContainedInAppSetting,
            TSetting defaultValue,
            Func<string, string> rawValueEvaluator)
        {
            _settingName = settingName;
            _isContainedInAppSetting = isContainedInAppSetting;
            _defaultValue = defaultValue;
            _rawValueEvaluator = rawValueEvaluator;

            _valueLoader = new Lazy<TSetting>(ValueLoader);
        }

        public static TSetting DefaultValue
        {
            get
            {
                return default(TSetting);
            }
        }

        public TSetting Value
        {
            get
            {
                return _valueLoader.Value;
            }
        }

        public bool IsContainedInAppSetting
        {
            get
            {
                return _isContainedInAppSetting;
            }
        }

        public string SettingName
        {
            get
            {
                return _settingName;
            }
        }

        protected abstract SettingEvaluationResult<TSetting> ProcessSettingValue(string rawSettingValue);

        private TSetting ValueLoader()
        {
            if (_isContainedInAppSetting)
            {
                string settingValue = _rawValueEvaluator(_settingName);
                if (settingValue != null)
                {
                    var result = ProcessSettingValue(settingValue);
                    if (!result.Successed)
                    {
                        throw new ConfigurationErrorsException(string.Format("Can't process settings value. Setting name: {0}, raw value: {1}", _settingName, settingValue));
                    }

                    return result.Value;
                }
            }

            return _defaultValue;
        }
    }
}