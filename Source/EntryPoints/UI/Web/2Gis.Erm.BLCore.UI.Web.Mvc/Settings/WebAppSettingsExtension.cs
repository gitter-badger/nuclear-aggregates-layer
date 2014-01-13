using System.Web;
using System.Web.Mvc;

using DoubleGis.Erm.Platform.API.Core.Settings.CRM;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings
{
    public static class WebAppSettingsExtension
    {
        public const string MsCrmSettingsKey = "MsCrmSettingsKey";
        public const string BasicOperationsServiceRestUrlKey = "BasicOperationsServiceRestUrlKey";
        public const string ErmBaseCurrencyKey = "ErmBaseCurrencyKey";

        public static IHtmlString GetMsCrmSettingsUrl(this ViewDataDictionary viewData)
        {
            var crmSettings = viewData.GetMsCrmSettings();
            if (crmSettings == null || !crmSettings.EnableReplication)
            {
                return new HtmlString(string.Empty);
            }

            var crmUrl = crmSettings.CrmHost + "/" + crmSettings.CrmOrganizationName;
            return new HtmlString(crmUrl);
        }

        public static string GetBasicOperationsServiceRestUrl(this ViewDataDictionary viewData)
        {
            object url;
            if (!viewData.TryGetValue(BasicOperationsServiceRestUrlKey, out url))
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

        private static IMsCrmSettings GetMsCrmSettings(this ViewDataDictionary viewData)
        {
            object crmSettings;
            if (!viewData.TryGetValue(MsCrmSettingsKey, out crmSettings))
            {
                return null;
            }

            return (IMsCrmSettings)crmSettings;
        }
    }
}