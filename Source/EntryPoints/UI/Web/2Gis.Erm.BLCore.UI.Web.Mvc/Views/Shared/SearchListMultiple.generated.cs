﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Views.Shared
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
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
    using DoubleGis.Erm.Platform.Common;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.Model.Entities.Enums;
    using DoubleGis.Erm.Platform.Model.Metadata.Enums;
    using DoubleGis.Erm.Platform.UI.Web.Mvc;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Security;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Shared/SearchListMultiple.cshtml")]
    public partial class SearchListMultiple : System.Web.Mvc.WebViewPage<Settings.ConfigurationDto.EntityViewSet>
    {
        public SearchListMultiple()
        {
        }
        public override void Execute()
        {
WriteLiteral("<html");

WriteLiteral(" xmlns=\"http://www.w3.org/1999/xhtml\"");

WriteLiteral(">\r\n<head>\r\n    <title>Поиск записей</title>\r\n    <meta");

WriteLiteral(" id=\"meta_IsDebug\"");

WriteLiteral(" name=\"meta_IsDebug\"");

WriteAttribute("content", Tuple.Create(" content=\"", 184), Tuple.Create("\"", 233)
            
            #line 6 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 194), Tuple.Create<System.Object, System.Int32>(HttpContext.Current.IsDebuggingEnabled
            
            #line default
            #line hidden
, 194), false)
);

WriteLiteral(" />\r\n    <meta");

WriteLiteral(" id=\"meta_Revision\"");

WriteLiteral(" name=\"meta_Revision\"");

WriteAttribute("content", Tuple.Create(" content=\"", 288), Tuple.Create("\"", 332)
            
            #line 7 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 298), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 298), false)
);

WriteLiteral(" />\r\n    \r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 386), Tuple.Create("\"", 448)
, Tuple.Create(Tuple.Create("", 393), Tuple.Create("/Content/ext-all.css?", 393), true)
            
            #line 9 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 414), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 414), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 496), Tuple.Create("\"", 555)
, Tuple.Create(Tuple.Create("", 503), Tuple.Create("/Content/CRM4.css?", 503), true)
            
            #line 10 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 521), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 521), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 603), Tuple.Create("\"", 666)
, Tuple.Create(Tuple.Create("", 610), Tuple.Create("/Content/MainPage.css?", 610), true)
            
            #line 11 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 632), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 632), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 714), Tuple.Create("\"", 777)
, Tuple.Create(Tuple.Create("", 721), Tuple.Create("/Content/ext-mask.css?", 721), true)
            
            #line 12 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 743), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 743), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 825), Tuple.Create("\"", 891)
, Tuple.Create(Tuple.Create("", 832), Tuple.Create("/Content/LookupStyle.css?", 832), true)
            
            #line 13 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 857), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 857), false)
);

WriteLiteral(" />\r\n\r\n");

            
            #line 15 "..\..\Views\Shared\SearchListMultiple.cshtml"
    
            
            #line default
            #line hidden
            
            #line 15 "..\..\Views\Shared\SearchListMultiple.cshtml"
     if (HttpContext.Current.IsDebuggingEnabled)
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 971), Tuple.Create("\"", 1038)
, Tuple.Create(Tuple.Create("", 977), Tuple.Create("/Scripts/ext-base-debug.js?", 977), true)
            
            #line 17 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 1004), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1004), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1089), Tuple.Create("\"", 1166)
, Tuple.Create(Tuple.Create("", 1095), Tuple.Create("/Scripts/ext-all-debug-w-comments.js?", 1095), true)
            
            #line 18 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 1132), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1132), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 19 "..\..\Views\Shared\SearchListMultiple.cshtml"
    }
    else
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1241), Tuple.Create("\"", 1302)
, Tuple.Create(Tuple.Create("", 1247), Tuple.Create("/Scripts/ext-base.js?", 1247), true)
            
            #line 22 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 1268), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1268), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1353), Tuple.Create("\"", 1413)
, Tuple.Create(Tuple.Create("", 1359), Tuple.Create("/Scripts/ext-all.js?", 1359), true)
            
            #line 23 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 1379), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1379), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 24 "..\..\Views\Shared\SearchListMultiple.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1469), Tuple.Create("\"", 1611)
, Tuple.Create(Tuple.Create("", 1475), Tuple.Create("/Scripts/", 1475), true)
            
            #line 26 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 1484), Tuple.Create<System.Object, System.Int32>("Ext.LocalizedResources." + ViewData.GetUserLocaleInfo().TwoLetterISOLanguageName + ".js"
            
            #line default
            #line hidden
, 1484), false)
, Tuple.Create(Tuple.Create("", 1576), Tuple.Create("?", 1576), true)
            
            #line 26 "..\..\Views\Shared\SearchListMultiple.cshtml"
                                       , Tuple.Create(Tuple.Create("", 1577), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1577), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.USER_ID = ");

            
            #line 29 "..\..\Views\Shared\SearchListMultiple.cshtml"
                 Write(ViewData.GetUserIdentityInfo().Code);

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.USER_NAME = ");

            
            #line 30 "..\..\Views\Shared\SearchListMultiple.cshtml"
                   Write(ViewData.GetUserIdentityDisplayName());

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo = ");

            
            #line 31 "..\..\Views\Shared\SearchListMultiple.cshtml"
                     Write(Html.WriteJson(ViewData.GetUserLocaleInfo()));

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo.NumberFormatInfo.CurrencySymbol = \'");

            
            #line 32 "..\..\Views\Shared\SearchListMultiple.cshtml"
                                                      Write(ViewData.GetErmBaseCurrencySymbol());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.CRM_URL = \'");

            
            #line 33 "..\..\Views\Shared\SearchListMultiple.cshtml"
                  Write(ViewData.GetMsCrmSettingsUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.BasicOperationsServiceRestUrl = \'");

            
            #line 34 "..\..\Views\Shared\SearchListMultiple.cshtml"
                                        Write(ViewData.GetBasicOperationsServiceRestUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.SpecialOperationsServiceRestUrl = \'");

            
            #line 35 "..\..\Views\Shared\SearchListMultiple.cshtml"
                                          Write(ViewData.GetSpecialOperationsServiceRestUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n    </script>\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2265), Tuple.Create("\"", 2343)
, Tuple.Create(Tuple.Create("", 2271), Tuple.Create("/Scripts/DoubleGis.GlobalVariables.js?", 2271), true)
            
            #line 38 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2309), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2309), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2390), Tuple.Create("\"", 2462)
, Tuple.Create(Tuple.Create("", 2396), Tuple.Create("/Scripts/Common.ErrorHandler.js?", 2396), true)
            
            #line 39 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2428), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2428), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2509), Tuple.Create("\"", 2580)
, Tuple.Create(Tuple.Create("", 2515), Tuple.Create("/Scripts/Ext.ux.FitToParent.js?", 2515), true)
            
            #line 40 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2546), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2546), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2627), Tuple.Create("\"", 2700)
, Tuple.Create(Tuple.Create("", 2633), Tuple.Create("/Scripts/Ext.ux.SearchControl.js?", 2633), true)
            
            #line 41 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2666), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2666), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2747), Tuple.Create("\"", 2820)
, Tuple.Create(Tuple.Create("", 2753), Tuple.Create("/Scripts/Ext.ux.TabularButton.js?", 2753), true)
            
            #line 42 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2786), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2786), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2867), Tuple.Create("\"", 2945)
, Tuple.Create(Tuple.Create("", 2873), Tuple.Create("/Scripts/Ext.ux.MultiSelectionList.js?", 2873), true)
            
            #line 43 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2911), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2911), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2992), Tuple.Create("\"", 3070)
, Tuple.Create(Tuple.Create("", 2998), Tuple.Create("/Scripts/Ext.ux.SearchFormMultiple.js?", 2998), true)
            
            #line 44 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 3036), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 3036), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    \r\n");

            
            #line 46 "..\..\Views\Shared\SearchListMultiple.cshtml"
    
            
            #line default
            #line hidden
            
            #line 46 "..\..\Views\Shared\SearchListMultiple.cshtml"
     foreach (var script in Model.DataViews.First().Scripts)
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3196), Tuple.Create("\"", 3262)
, Tuple.Create(Tuple.Create("", 3202), Tuple.Create("/Scripts/", 3202), true)
            
            #line 48 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 3211), Tuple.Create<System.Object, System.Int32>(script.FileName
            
            #line default
            #line hidden
, 3211), false)
, Tuple.Create(Tuple.Create("", 3227), Tuple.Create("?", 3227), true)
            
            #line 48 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 3228), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 3228), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 49 "..\..\Views\Shared\SearchListMultiple.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n\r\n        Ext.onReady(function ()\r\n            {\r\n            var settings = ");

            
            #line 55 "..\..\Views\Shared\SearchListMultiple.cshtml"
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
