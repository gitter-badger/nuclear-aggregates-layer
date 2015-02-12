using System.Web.Mvc;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings
{
    public static class WebAppSettingsExtension
    {
        public const string MsCrmSettingsKey = "MsCrmSettingsKey";
        public const string BasicOperationsServiceRestUrlKey = "BasicOperationsServiceRestUrlKey";
        public const string SpecialOperationsServiceRestUrlKey = "SpecialOperationsServiceRestUrlKey";
        public const string IdentityServiceRestUrlKey = "IdentityServiceRestUrlKey";
        public const string ErmBaseCurrencyKey = "ErmBaseCurrencyKey";

        public static string GetBasicOperationsServiceRestUrl(this ViewDataDictionary viewData)
        {
            return GetSetting(viewData, BasicOperationsServiceRestUrlKey);
        }

        public static string GetSpecialOperationsServiceRestUrl(this ViewDataDictionary viewData)
        {
            return GetSetting(viewData, SpecialOperationsServiceRestUrlKey);
        }

        public static string GetIdentityServiceRestUrl(this ViewDataDictionary viewData)
        {
            return GetSetting(viewData, IdentityServiceRestUrlKey);
        }

        public static string GetErmBaseCurrencySymbol(this ViewDataDictionary viewData)
        {
            return GetSetting(viewData, ErmBaseCurrencyKey);
        }

        private static string GetSetting(ViewDataDictionary viewData, string settingName)
        {
            object setting;
            if (!viewData.TryGetValue(settingName, out setting))
            {
                return null;
            }

            return (string)setting;
        }
    }
}