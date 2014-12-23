using System.Web.Mvc;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings
{
    public static class WebAppSettingsExtension
    {
        public const string MsCrmSettingsKey = "MsCrmSettingsKey";
        public const string BasicOperationsServiceRestUrlKey = "BasicOperationsServiceRestUrlKey";
        public const string SpecialOperationsServiceRestUrlKey = "SpecialOperationsServiceRestUrlKey";
        public const string ErmBaseCurrencyKey = "ErmBaseCurrencyKey";

        public static string GetBasicOperationsServiceRestUrl(this ViewDataDictionary viewData)
        {
            object url;
            if (!viewData.TryGetValue(BasicOperationsServiceRestUrlKey, out url))
            {
                return null;
            }

            return (string)url;
        }

        public static string GetSpecialOperationsServiceRestUrl(this ViewDataDictionary viewData)
        {
            object url;
            if (!viewData.TryGetValue(SpecialOperationsServiceRestUrlKey, out url))
            {
                return null;
            }

            return (string)url;
        }

        public static string GetErmBaseCurrencySymbol(this ViewDataDictionary viewData)
        {
            object baseCurrencySymbol;
            if (!viewData.TryGetValue(ErmBaseCurrencyKey, out baseCurrencySymbol))
            {
                return null;
            }

            return (string)baseCurrencySymbol;
        }
    }
}