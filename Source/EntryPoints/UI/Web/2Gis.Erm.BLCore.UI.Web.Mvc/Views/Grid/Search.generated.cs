﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Views.Grid
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Optimization;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    using DoubleGis.Erm.BLCore.Resources.Server.Properties;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
    using DoubleGis.Erm.Platform.Common;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.Model.Entities.Enums;
    using DoubleGis.Erm.Platform.Model.Metadata.Enums;
    using DoubleGis.Erm.Platform.UI.Web.Mvc;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    
    #line 1 "..\..\Views\Grid\Search.cshtml"
    using Platform.UI.Web.Mvc.Security;
    
    #line default
    #line hidden
    
    #line 2 "..\..\Views\Grid\Search.cshtml"
    using Settings;
    
    #line default
    #line hidden
    
    #line 3 "..\..\Views\Grid\Search.cshtml"
    using UserProfiles;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Grid/Search.cshtml")]
    public partial class Search : System.Web.Mvc.WebViewPage<Settings.ConfigurationDto.EntityViewSet>
    {
        public Search()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

WriteLiteral("<html");

WriteLiteral(" xmlns=\"http://www.w3.org/1999/xhtml\"");

WriteLiteral(">\r\n<head>\r\n    <title>");

            
            #line 9 "..\..\Views\Grid\Search.cshtml"
      Write(BLResources.RecordsSearch);

            
            #line default
            #line hidden
WriteLiteral("</title>\r\n    <meta");

WriteLiteral(" id=\"meta_IsDebug\"");

WriteLiteral(" name=\"meta_IsDebug\"");

WriteAttribute("content", Tuple.Create(" content=\"", 274), Tuple.Create("\"", 323)
            
            #line 10 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 284), Tuple.Create<System.Object, System.Int32>(HttpContext.Current.IsDebuggingEnabled
            
            #line default
            #line hidden
, 284), false)
);

WriteLiteral(" />\r\n    <meta");

WriteLiteral(" id=\"meta_Revision\"");

WriteLiteral(" name=\"meta_Revision\"");

WriteAttribute("content", Tuple.Create(" content=\"", 378), Tuple.Create("\"", 422)
            
            #line 11 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 388), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 388), false)
);

WriteLiteral(" />\r\n    \r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 476), Tuple.Create("\"", 538)
, Tuple.Create(Tuple.Create("", 483), Tuple.Create("/Content/ext-all.css?", 483), true)
            
            #line 13 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 504), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 504), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 586), Tuple.Create("\"", 645)
, Tuple.Create(Tuple.Create("", 593), Tuple.Create("/Content/CRM4.css?", 593), true)
            
            #line 14 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 611), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 611), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 693), Tuple.Create("\"", 756)
, Tuple.Create(Tuple.Create("", 700), Tuple.Create("/Content/MainPage.css?", 700), true)
            
            #line 15 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 722), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 722), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 804), Tuple.Create("\"", 867)
, Tuple.Create(Tuple.Create("", 811), Tuple.Create("/Content/ext-mask.css?", 811), true)
            
            #line 16 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 833), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 833), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 915), Tuple.Create("\"", 981)
, Tuple.Create(Tuple.Create("", 922), Tuple.Create("/Content/LookupStyle.css?", 922), true)
            
            #line 17 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 947), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 947), false)
);

WriteLiteral(" />\r\n    \r\n");

            
            #line 19 "..\..\Views\Grid\Search.cshtml"
    
            
            #line default
            #line hidden
            
            #line 19 "..\..\Views\Grid\Search.cshtml"
     if (HttpContext.Current.IsDebuggingEnabled)
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1065), Tuple.Create("\"", 1132)
, Tuple.Create(Tuple.Create("", 1071), Tuple.Create("/Scripts/ext-base-debug.js?", 1071), true)
            
            #line 21 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 1098), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1098), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1183), Tuple.Create("\"", 1260)
, Tuple.Create(Tuple.Create("", 1189), Tuple.Create("/Scripts/ext-all-debug-w-comments.js?", 1189), true)
            
            #line 22 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 1226), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1226), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 23 "..\..\Views\Grid\Search.cshtml"
    }
    else
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1335), Tuple.Create("\"", 1396)
, Tuple.Create(Tuple.Create("", 1341), Tuple.Create("/Scripts/ext-base.js?", 1341), true)
            
            #line 26 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 1362), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1362), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1447), Tuple.Create("\"", 1507)
, Tuple.Create(Tuple.Create("", 1453), Tuple.Create("/Scripts/ext-all.js?", 1453), true)
            
            #line 27 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 1473), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1473), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 28 "..\..\Views\Grid\Search.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1563), Tuple.Create("\"", 1705)
, Tuple.Create(Tuple.Create("", 1569), Tuple.Create("/Scripts/", 1569), true)
            
            #line 30 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 1578), Tuple.Create<System.Object, System.Int32>("Ext.LocalizedResources." + ViewData.GetUserLocaleInfo().TwoLetterISOLanguageName + ".js"
            
            #line default
            #line hidden
, 1578), false)
, Tuple.Create(Tuple.Create("", 1670), Tuple.Create("?", 1670), true)
            
            #line 30 "..\..\Views\Grid\Search.cshtml"
                                       , Tuple.Create(Tuple.Create("", 1671), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1671), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.USER_ID = ");

            
            #line 33 "..\..\Views\Grid\Search.cshtml"
                 Write(ViewData.GetUserIdentityInfo().Code);

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.USER_NAME = ");

            
            #line 34 "..\..\Views\Grid\Search.cshtml"
                   Write(ViewData.GetUserIdentityDisplayName());

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo = ");

            
            #line 35 "..\..\Views\Grid\Search.cshtml"
                     Write(Html.WriteJson(ViewData.GetUserLocaleInfo()));

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo.NumberFormatInfo.CurrencySymbol = \'");

            
            #line 36 "..\..\Views\Grid\Search.cshtml"
                                                      Write(ViewData.GetErmBaseCurrencySymbol());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.CRM_URL = \'");

            
            #line 37 "..\..\Views\Grid\Search.cshtml"
                  Write(ViewData.GetMsCrmSettingsUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.BasicOperationsServiceRestUrl = \'");

            
            #line 38 "..\..\Views\Grid\Search.cshtml"
                                        Write(ViewData.GetBasicOperationsServiceRestUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n    </script>\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2262), Tuple.Create("\"", 2340)
, Tuple.Create(Tuple.Create("", 2268), Tuple.Create("/Scripts/DoubleGis.GlobalVariables.js?", 2268), true)
            
            #line 41 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 2306), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2306), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2387), Tuple.Create("\"", 2459)
, Tuple.Create(Tuple.Create("", 2393), Tuple.Create("/Scripts/Common.ErrorHandler.js?", 2393), true)
            
            #line 42 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 2425), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2425), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2506), Tuple.Create("\"", 2577)
, Tuple.Create(Tuple.Create("", 2512), Tuple.Create("/Scripts/Ext.ux.FitToParent.js?", 2512), true)
            
            #line 43 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 2543), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2543), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2624), Tuple.Create("\"", 2697)
, Tuple.Create(Tuple.Create("", 2630), Tuple.Create("/Scripts/Ext.ux.SearchControl.js?", 2630), true)
            
            #line 44 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 2663), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2663), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2744), Tuple.Create("\"", 2817)
, Tuple.Create(Tuple.Create("", 2750), Tuple.Create("/Scripts/Ext.ux.TabularButton.js?", 2750), true)
            
            #line 45 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 2783), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2783), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2864), Tuple.Create("\"", 2934)
, Tuple.Create(Tuple.Create("", 2870), Tuple.Create("/Scripts/Ext.ux.SearchForm.js?", 2870), true)
            
            #line 46 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 2900), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2900), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    \r\n");

            
            #line 48 "..\..\Views\Grid\Search.cshtml"
    
            
            #line default
            #line hidden
            
            #line 48 "..\..\Views\Grid\Search.cshtml"
     foreach (var script in Model.DataViews.First().Scripts)
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3060), Tuple.Create("\"", 3126)
, Tuple.Create(Tuple.Create("", 3066), Tuple.Create("/Scripts/", 3066), true)
            
            #line 50 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 3075), Tuple.Create<System.Object, System.Int32>(script.FileName
            
            #line default
            #line hidden
, 3075), false)
, Tuple.Create(Tuple.Create("", 3091), Tuple.Create("?", 3091), true)
            
            #line 50 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 3092), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 3092), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 51 "..\..\Views\Grid\Search.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.onReady(function ()\r\n            {\r\n            var settings = ");

            
            #line 55 "..\..\Views\Grid\Search.cshtml"
                      Write(Html.WriteJson(Model));

            
            #line default
            #line hidden
WriteLiteral(@";
            var qstring = Ext.urlDecode(location.search.substring(1));

            var fitToParentPlugin = new window.Ext.ux.FitToParent(document.body);
            if (Ext.isChrome) {
                fitToParentPlugin.offsets[0] = 10;
                fitToParentPlugin.offsets[1] = 10;
            }
            
            window.Entity = new Ext.ux.SearchForm(
                {
                    applyTo: Ext.getBody(),
                    plugins: [fitToParentPlugin],
                    searchFormSettings: settings,
                    extendedInfo: qstring.extendedInfo,
                    existingItem: window.dialogArguments,
                    readOnly: qstring.ReadOnly === ""true""
                });
        });
    </script>
</head>
<body>
</body>
</html>");

        }
    }
}
#pragma warning restore 1591
