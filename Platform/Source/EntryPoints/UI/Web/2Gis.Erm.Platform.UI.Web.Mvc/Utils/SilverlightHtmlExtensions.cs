using System;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DoubleGis.Erm.Platform.UI.Web.Mvc.Utils
{
    public static class SilverlightHtmlExtensions
    {
        private const string SilverlightControlsDefaultRelativePath = "ClientBin";

        private const string ParamTemplate = @"<param name=""{0}"" value=""{1}"" />";
        private const string SilverlightObjectTagTemplate = 
          @"<object data=""data:application/x-silverlight-2,"" type=""application/x-silverlight-2"" width=""100%"" height=""100%"">
	            {0}
	            <a href=""http://go.microsoft.com/fwlink/?LinkID=149156&v={1}"" style=""text-decoration:none"">
 		            <img src=""http://go.microsoft.com/fwlink/?LinkId=161376"" alt=""Get Microsoft Silverlight"" style=""border-style:none""/>
	            </a>
            </object>
            <iframe id=""Iframe1"" style=""visibility:hidden;height:0px;width:0px;border:0px""></iframe>";

        private static string GetAbsoluteVersionedSilverlightUrl(string xapFileName)
        {
            return VirtualPathUtility.ToAbsolute(string.Concat("~/", SilverlightControlsDefaultRelativePath, "/", xapFileName, "?", ThisAssembly.Build));
        }
        
        private static void AddParam<T>(
            this StringBuilder sb,
            string paramName,
            Expression<Func<T>> e,
            bool isRequiredParam,
            Func<SilverlightControlSettings, bool> canAddChecker,
            Func<T, string> valueConverter) 
        {
            var member = (MemberExpression)e.Body; 
            var hostTypeExpression = member.Expression;
            if (hostTypeExpression.Type != typeof(SilverlightControlSettings))
            {
                throw new InvalidOperationException("Only one of the " + hostTypeExpression.Type.Name + " type properties can be add");
            }

            var settings = Expression.Lambda<Func<SilverlightControlSettings>>(hostTypeExpression).Compile()();
            string propertyName = member.Member.Name;
            T propertyValue = e.Compile()();

            if (!canAddChecker(settings))
            {
                if (isRequiredParam)
                {
                    throw new ArgumentException("Required param " + propertyName + " member of type " + hostTypeExpression.Type.Name + " has invalid value");
                }

                return;
            }

            if (valueConverter != null)
            {
                sb.AppendFormat(ParamTemplate, paramName, valueConverter(propertyValue));
            }
            else
            {
                sb.AppendFormat(ParamTemplate, paramName, propertyValue);
            }
        }

        private static void AddParam<T>(this StringBuilder sb, string paramName, Expression<Func<T>> e, bool isRequiredParam, Func<SilverlightControlSettings, bool> canAddChecker)
        {
            sb.AddParam(paramName, e, isRequiredParam, canAddChecker, null);
        }

        private static string ToInitParamsString(NameValueCollection initParams)
        {
            const string ElementTemplate = "{0}={1}";

            if (initParams.Count == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            int lastElementIndex = initParams.Count - 1;
            for (int i = 0; i < lastElementIndex; i++)
            {
                sb.AppendFormat(ElementTemplate, initParams.GetKey(i), initParams.Get(i) + ",");
            }

            sb.AppendFormat(ElementTemplate, initParams.GetKey(lastElementIndex), initParams.Get(lastElementIndex));

            return sb.ToString();
        }

        public static MvcHtmlString VersionedSilverlightControl(this HtmlHelper helper, SilverlightControlSettings controlSettings)
        {
            if (!string.IsNullOrWhiteSpace(controlSettings.ControlCulture))
            {
                controlSettings.InitParams.Add("ControlCulture", controlSettings.ControlCulture);
            }

            var sb = new StringBuilder();
            sb.AddParam("source", () => controlSettings.ControlXapFileName, true, settings => !string.IsNullOrWhiteSpace(settings.ControlXapFileName), GetAbsoluteVersionedSilverlightUrl);
            sb.AddParam("onError", () => controlSettings.OnErrorJsHandlerName, false, settings => !string.IsNullOrWhiteSpace(settings.OnErrorJsHandlerName));
            sb.AddParam("onLoad", () => controlSettings.OnLoadedJsHandlerName, false, settings => !string.IsNullOrWhiteSpace(settings.OnLoadedJsHandlerName));
            sb.AddParam("background", () => controlSettings.BackgroundColor, false, settings => !string.IsNullOrWhiteSpace(settings.BackgroundColor));
            sb.AddParam("minRuntimeVersion", () => controlSettings.MinRuntimeVersion, true, settings => !string.IsNullOrWhiteSpace(settings.MinRuntimeVersion));
            sb.AddParam("autoUpgrade", () => controlSettings.AutoUpgrade, true, settings => true);
            sb.AddParam("initParams", () => controlSettings.InitParams, true, settings => true, ToInitParamsString);

            return MvcHtmlString.Create(string.Format(SilverlightObjectTagTemplate, sb, controlSettings.MinRuntimeVersion));
        }
    }
}
