using System;
using System.Collections.Generic;
using System.Configuration;

using Nuclear.Settings.API;

namespace Nuclear.Settings
{
    public static class ConfigFileSetting
    {
        private static readonly HashSet<string> AllAvailableSettings = new HashSet<string>(ConfigurationManager.AppSettings.AllKeys);
        private static string EvaluateSettingRawValue(string settingName)
        {
            return ConfigurationManager.AppSettings[settingName];
        }

        private static bool EvaluateSettingAvailability(string settingName)
        {
            return AllAvailableSettings.Contains(settingName);
        }

        private static void GuaranteeSettingAvailability(string settingName)
        {
            if (!AllAvailableSettings.Contains(settingName))
            {
                throw new ApplicationException(string.Format("Required application setting \"{0}\" is not specified in config", settingName));
            } 
        }

        public static class Int
        {
            public static IntSetting Required(string settingName)
            {
                GuaranteeSettingAvailability(settingName);
                return Create(settingName, true, IntSetting.DefaultValue);
            }

            public static IntSetting Optional(string settingName, int defaultValue)
            {
                return Create(settingName, EvaluateSettingAvailability(settingName), defaultValue);
            }

            private static IntSetting Create(string settingName, bool isContainedInAppSetting, int defaultValue)
            {
                return new IntSetting(settingName, isContainedInAppSetting, defaultValue, EvaluateSettingRawValue); 
            }
        }

        public static class Bool
        {
            public static BoolSetting Required(string settingName)
            {
                GuaranteeSettingAvailability(settingName);
                return Create(settingName, true, BoolSetting.DefaultValue);
            }

            public static BoolSetting Optional(string settingName, bool defaultValue)
            {
                return Create(settingName, EvaluateSettingAvailability(settingName), defaultValue);
            }

            private static BoolSetting Create(string settingName, bool isContainedInAppSetting, bool defaultValue)
            {
                return new BoolSetting(settingName, isContainedInAppSetting, defaultValue, EvaluateSettingRawValue); 
            }
        }

        public static class String
        {
            public static StringSetting Required(string settingName)
            {
                GuaranteeSettingAvailability(settingName);
                return Create(settingName, true, StringSetting.DefaultValue);
            }

            public static StringSetting Optional(string settingName, string defaultValue)
            {
                return Create(settingName, EvaluateSettingAvailability(settingName), defaultValue);
            }

            private static StringSetting Create(string settingName, bool isContainedInAppSetting, string defaultValue)
            {
                return new StringSetting(settingName, isContainedInAppSetting, defaultValue, EvaluateSettingRawValue); 
            }
        }

        public static class Decimal
        {
            public static DecimalSetting Required(string settingName)
            {
                GuaranteeSettingAvailability(settingName);
                return Create(settingName, true, DecimalSetting.DefaultValue);
            }

            public static DecimalSetting Optional(string settingName, decimal defaultValue)
            {
                return Create(settingName, EvaluateSettingAvailability(settingName), defaultValue);
            }

            private static DecimalSetting Create(string settingName, bool isContainedInAppSetting, decimal defaultValue)
            {
                return new DecimalSetting(settingName, isContainedInAppSetting, defaultValue, EvaluateSettingRawValue); 
            }
        }

        public static class Enum
        {
            public static EnumSetting<TSetting> Required<TSetting>(string settingName) 
                where TSetting : struct, IConvertible
            {
                GuaranteeSettingAvailability(settingName);
                return Create(settingName, true, EnumSetting<TSetting>.DefaultValue);
            }

            public static EnumSetting<TSetting> Optional<TSetting>(string settingName, TSetting defaultValue)
                where TSetting : struct, IConvertible
            {
                return Create(settingName, EvaluateSettingAvailability(settingName), defaultValue);
            }

            private static EnumSetting<TSetting> Create<TSetting>(string settingName, bool isContainedInAppSetting, TSetting defaultValue)
                where TSetting : struct, IConvertible
            {
                return new EnumSetting<TSetting>(settingName, isContainedInAppSetting, defaultValue, EvaluateSettingRawValue); 
            }
        }
    }
}
