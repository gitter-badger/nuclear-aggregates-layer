using System;
using System.Web;
using System.Web.Mvc;

using NuClear.Security.API.UserContext.Identity;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.Security
{
    public static class IdentityWebExtensions
    {
        public const String UserIdentityInfoKey = "UserIdentityInfoKey";

        public static IUserInfo GetUserIdentityInfo(this ViewDataDictionary viewData)
        {
            Object identityInfo = null;
            if (!viewData.TryGetValue(UserIdentityInfoKey, out identityInfo))
            {
                return null;
            }

            return (IUserInfo)identityInfo;
        }

        public static IHtmlString GetUserIdentityDisplayName(this ViewDataDictionary viewData)
        {
            var result = viewData.GetUserIdentityInfo();
            return new HtmlString(result != null ? "'" + result.DisplayName + "'": String.Empty);
        }
    }
}