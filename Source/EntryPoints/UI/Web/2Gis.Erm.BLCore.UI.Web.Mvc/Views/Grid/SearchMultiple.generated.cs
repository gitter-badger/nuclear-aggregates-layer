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
    
    #line 1 "..\..\Views\Grid\SearchMultiple.cshtml"
    using Platform.UI.Web.Mvc.Security;
    
    #line default
    #line hidden
    
    #line 2 "..\..\Views\Grid\SearchMultiple.cshtml"
    using Settings;
    
    #line default
    #line hidden
    
    #line 3 "..\..\Views\Grid\SearchMultiple.cshtml"
    using UserProfiles;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Grid/SearchMultiple.cshtml")]
    public partial class SearchMultiple : System.Web.Mvc.WebViewPage<Settings.ConfigurationDto.EntityViewSet>
    {
        public SearchMultiple()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

WriteLiteral("<html");

WriteLiteral(" xmlns=\"http://www.w3.org/1999/xhtml\"");

WriteLiteral(">\r\n<head>\r\n    <title>");

            
            #line 9 "..\..\Views\Grid\SearchMultiple.cshtml"
      Write(BLResources.RecordsSearch);

            
            #line default
            #line hidden
WriteLiteral("</title>\r\n    <meta");

WriteLiteral(" id=\"meta_IsDebug\"");

WriteLiteral(" name=\"meta_IsDebug\"");

WriteAttribute("content", Tuple.Create(" content=\"", 274), Tuple.Create("\"", 323)
            
            #line 10 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 284), Tuple.Create<System.Object, System.Int32>(HttpContext.Current.IsDebuggingEnabled
            
            #line default
            #line hidden
, 284), false)
);

WriteLiteral(" />\r\n    <meta");

WriteLiteral(" id=\"meta_Revision\"");

WriteLiteral(" name=\"meta_Revision\"");

WriteAttribute("content", Tuple.Create(" content=\"", 378), Tuple.Create("\"", 422)
            
            #line 11 "..\..\Views\Grid\SearchMultiple.cshtml"
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
            
            #line 13 "..\..\Views\Grid\SearchMultiple.cshtml"
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
            
            #line 14 "..\..\Views\Grid\SearchMultiple.cshtml"
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
            
            #line 15 "..\..\Views\Grid\SearchMultiple.cshtml"
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
            
            #line 16 "..\..\Views\Grid\SearchMultiple.cshtml"
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
            
            #line 17 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 947), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 947), false)
);

WriteLiteral(" />\r\n\r\n");

            
            #line 19 "..\..\Views\Grid\SearchMultiple.cshtml"
    
            
            #line default
            #line hidden
            
            #line 19 "..\..\Views\Grid\SearchMultiple.cshtml"
     if (HttpContext.Current.IsDebuggingEnabled)
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1061), Tuple.Create("\"", 1128)
, Tuple.Create(Tuple.Create("", 1067), Tuple.Create("/Scripts/ext-base-debug.js?", 1067), true)
            
            #line 21 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 1094), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1094), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1179), Tuple.Create("\"", 1256)
, Tuple.Create(Tuple.Create("", 1185), Tuple.Create("/Scripts/ext-all-debug-w-comments.js?", 1185), true)
            
            #line 22 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 1222), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1222), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 23 "..\..\Views\Grid\SearchMultiple.cshtml"
    }
    else
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1331), Tuple.Create("\"", 1392)
, Tuple.Create(Tuple.Create("", 1337), Tuple.Create("/Scripts/ext-base.js?", 1337), true)
            
            #line 26 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 1358), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1358), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1443), Tuple.Create("\"", 1503)
, Tuple.Create(Tuple.Create("", 1449), Tuple.Create("/Scripts/ext-all.js?", 1449), true)
            
            #line 27 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 1469), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1469), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 28 "..\..\Views\Grid\SearchMultiple.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1559), Tuple.Create("\"", 1701)
, Tuple.Create(Tuple.Create("", 1565), Tuple.Create("/Scripts/", 1565), true)
            
            #line 30 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 1574), Tuple.Create<System.Object, System.Int32>("Ext.LocalizedResources." + ViewData.GetUserLocaleInfo().TwoLetterISOLanguageName + ".js"
            
            #line default
            #line hidden
, 1574), false)
, Tuple.Create(Tuple.Create("", 1666), Tuple.Create("?", 1666), true)
            
            #line 30 "..\..\Views\Grid\SearchMultiple.cshtml"
                                       , Tuple.Create(Tuple.Create("", 1667), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1667), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.USER_ID = ");

            
            #line 33 "..\..\Views\Grid\SearchMultiple.cshtml"
                 Write(ViewData.GetUserIdentityInfo().Code);

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.USER_NAME = ");

            
            #line 34 "..\..\Views\Grid\SearchMultiple.cshtml"
                   Write(ViewData.GetUserIdentityDisplayName());

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo = ");

            
            #line 35 "..\..\Views\Grid\SearchMultiple.cshtml"
                     Write(Html.WriteJson(ViewData.GetUserLocaleInfo()));

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo.NumberFormatInfo.CurrencySymbol = \'");

            
            #line 36 "..\..\Views\Grid\SearchMultiple.cshtml"
                                                      Write(ViewData.GetErmBaseCurrencySymbol());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.CRM_URL = \'");

            
            #line 37 "..\..\Views\Grid\SearchMultiple.cshtml"
                  Write(ViewData.GetMsCrmSettingsUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.BasicOperationsServiceRestUrl = \'");

            
            #line 38 "..\..\Views\Grid\SearchMultiple.cshtml"
                                        Write(ViewData.GetBasicOperationsServiceRestUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n    </script>\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2258), Tuple.Create("\"", 2336)
, Tuple.Create(Tuple.Create("", 2264), Tuple.Create("/Scripts/DoubleGis.GlobalVariables.js?", 2264), true)
            
            #line 41 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2302), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2302), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2383), Tuple.Create("\"", 2455)
, Tuple.Create(Tuple.Create("", 2389), Tuple.Create("/Scripts/Common.ErrorHandler.js?", 2389), true)
            
            #line 42 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2421), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2421), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2502), Tuple.Create("\"", 2573)
, Tuple.Create(Tuple.Create("", 2508), Tuple.Create("/Scripts/Ext.ux.FitToParent.js?", 2508), true)
            
            #line 43 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2539), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2539), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2620), Tuple.Create("\"", 2693)
, Tuple.Create(Tuple.Create("", 2626), Tuple.Create("/Scripts/Ext.ux.SearchControl.js?", 2626), true)
            
            #line 44 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2659), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2659), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2740), Tuple.Create("\"", 2813)
, Tuple.Create(Tuple.Create("", 2746), Tuple.Create("/Scripts/Ext.ux.TabularButton.js?", 2746), true)
            
            #line 45 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2779), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2779), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2860), Tuple.Create("\"", 2938)
, Tuple.Create(Tuple.Create("", 2866), Tuple.Create("/Scripts/Ext.ux.MultiSelectionList.js?", 2866), true)
            
            #line 46 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2904), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2904), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2985), Tuple.Create("\"", 3063)
, Tuple.Create(Tuple.Create("", 2991), Tuple.Create("/Scripts/Ext.ux.SearchFormMultiple.js?", 2991), true)
            
            #line 47 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 3029), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 3029), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    \r\n");

            
            #line 49 "..\..\Views\Grid\SearchMultiple.cshtml"
    
            
            #line default
            #line hidden
            
            #line 49 "..\..\Views\Grid\SearchMultiple.cshtml"
     foreach (var script in Model.DataViews.First().Scripts)
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3189), Tuple.Create("\"", 3255)
, Tuple.Create(Tuple.Create("", 3195), Tuple.Create("/Scripts/", 3195), true)
            
            #line 51 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 3204), Tuple.Create<System.Object, System.Int32>(script.FileName
            
            #line default
            #line hidden
, 3204), false)
, Tuple.Create(Tuple.Create("", 3220), Tuple.Create("?", 3220), true)
            
            #line 51 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 3221), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 3221), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 52 "..\..\Views\Grid\SearchMultiple.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n\r\n        Ext.onReady(function ()\r\n            {\r\n            var settings = ");

            
            #line 57 "..\..\Views\Grid\SearchMultiple.cshtml"
                      Write(Html.WriteJson(Model));

            
            #line default
            #line hidden
WriteLiteral(@";
            var qstring = Ext.urlDecode(location.search.substring(1));
            window.Entity = new Ext.ux.SearchFormMultiple(
                {
                    applyTo: Ext.getBody(),
                    plugins: [new window.Ext.ux.FitToParent(document.body)],
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
</html>
");

        }
    }
}
#pragma warning restore 1591
