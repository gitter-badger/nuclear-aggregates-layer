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
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
    using DoubleGis.Erm.Platform.Common;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.Model.Entities.Enums;
    using DoubleGis.Erm.Platform.Model.Metadata.Enums;
    using DoubleGis.Erm.Platform.UI.Web.Mvc;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    
    #line 1 "..\..\Views\Shared\SearchListMultiple.cshtml"
    using Platform.UI.Web.Mvc.Security;
    
    #line default
    #line hidden
    
    #line 2 "..\..\Views\Shared\SearchListMultiple.cshtml"
    using Settings;
    
    #line default
    #line hidden
    
    #line 3 "..\..\Views\Shared\SearchListMultiple.cshtml"
    using UserProfiles;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Shared/SearchListMultiple.cshtml")]
    public partial class SearchListMultiple : System.Web.Mvc.WebViewPage<Settings.ConfigurationDto.EntityViewSet>
    {
        public SearchListMultiple()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

WriteLiteral("<html");

WriteLiteral(" xmlns=\"http://www.w3.org/1999/xhtml\"");

WriteLiteral(">\r\n<head>\r\n    <title>Поиск записей</title>\r\n    <meta");

WriteLiteral(" id=\"meta_IsDebug\"");

WriteLiteral(" name=\"meta_IsDebug\"");

WriteAttribute("content", Tuple.Create(" content=\"", 261), Tuple.Create("\"", 310)
            
            #line 10 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 271), Tuple.Create<System.Object, System.Int32>(HttpContext.Current.IsDebuggingEnabled
            
            #line default
            #line hidden
, 271), false)
);

WriteLiteral(" />\r\n    <meta");

WriteLiteral(" id=\"meta_Revision\"");

WriteLiteral(" name=\"meta_Revision\"");

WriteAttribute("content", Tuple.Create(" content=\"", 365), Tuple.Create("\"", 409)
            
            #line 11 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 375), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 375), false)
);

WriteLiteral(" />\r\n    \r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 463), Tuple.Create("\"", 525)
, Tuple.Create(Tuple.Create("", 470), Tuple.Create("/Content/ext-all.css?", 470), true)
            
            #line 13 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 491), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 491), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 573), Tuple.Create("\"", 632)
, Tuple.Create(Tuple.Create("", 580), Tuple.Create("/Content/CRM4.css?", 580), true)
            
            #line 14 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 598), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 598), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 680), Tuple.Create("\"", 743)
, Tuple.Create(Tuple.Create("", 687), Tuple.Create("/Content/MainPage.css?", 687), true)
            
            #line 15 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 709), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 709), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 791), Tuple.Create("\"", 854)
, Tuple.Create(Tuple.Create("", 798), Tuple.Create("/Content/ext-mask.css?", 798), true)
            
            #line 16 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 820), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 820), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 902), Tuple.Create("\"", 968)
, Tuple.Create(Tuple.Create("", 909), Tuple.Create("/Content/LookupStyle.css?", 909), true)
            
            #line 17 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 934), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 934), false)
);

WriteLiteral(" />\r\n\r\n");

            
            #line 19 "..\..\Views\Shared\SearchListMultiple.cshtml"
    
            
            #line default
            #line hidden
            
            #line 19 "..\..\Views\Shared\SearchListMultiple.cshtml"
     if (HttpContext.Current.IsDebuggingEnabled)
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1048), Tuple.Create("\"", 1115)
, Tuple.Create(Tuple.Create("", 1054), Tuple.Create("/Scripts/ext-base-debug.js?", 1054), true)
            
            #line 21 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 1081), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1081), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1166), Tuple.Create("\"", 1243)
, Tuple.Create(Tuple.Create("", 1172), Tuple.Create("/Scripts/ext-all-debug-w-comments.js?", 1172), true)
            
            #line 22 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 1209), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1209), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 23 "..\..\Views\Shared\SearchListMultiple.cshtml"
    }
    else
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1318), Tuple.Create("\"", 1379)
, Tuple.Create(Tuple.Create("", 1324), Tuple.Create("/Scripts/ext-base.js?", 1324), true)
            
            #line 26 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 1345), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1345), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1430), Tuple.Create("\"", 1490)
, Tuple.Create(Tuple.Create("", 1436), Tuple.Create("/Scripts/ext-all.js?", 1436), true)
            
            #line 27 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 1456), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1456), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 28 "..\..\Views\Shared\SearchListMultiple.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1546), Tuple.Create("\"", 1688)
, Tuple.Create(Tuple.Create("", 1552), Tuple.Create("/Scripts/", 1552), true)
            
            #line 30 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 1561), Tuple.Create<System.Object, System.Int32>("Ext.LocalizedResources." + ViewData.GetUserLocaleInfo().TwoLetterISOLanguageName + ".js"
            
            #line default
            #line hidden
, 1561), false)
, Tuple.Create(Tuple.Create("", 1653), Tuple.Create("?", 1653), true)
            
            #line 30 "..\..\Views\Shared\SearchListMultiple.cshtml"
                                       , Tuple.Create(Tuple.Create("", 1654), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1654), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.USER_ID = ");

            
            #line 33 "..\..\Views\Shared\SearchListMultiple.cshtml"
                 Write(ViewData.GetUserIdentityInfo().Code);

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.USER_NAME = ");

            
            #line 34 "..\..\Views\Shared\SearchListMultiple.cshtml"
                   Write(ViewData.GetUserIdentityDisplayName());

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo = ");

            
            #line 35 "..\..\Views\Shared\SearchListMultiple.cshtml"
                     Write(Html.WriteJson(ViewData.GetUserLocaleInfo()));

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo.NumberFormatInfo.CurrencySymbol = \'");

            
            #line 36 "..\..\Views\Shared\SearchListMultiple.cshtml"
                                                      Write(ViewData.GetErmBaseCurrencySymbol());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.CRM_URL = \'");

            
            #line 37 "..\..\Views\Shared\SearchListMultiple.cshtml"
                  Write(ViewData.GetMsCrmSettingsUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.BasicOperationsServiceRestUrl = \'");

            
            #line 38 "..\..\Views\Shared\SearchListMultiple.cshtml"
                                        Write(ViewData.GetBasicOperationsServiceRestUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n    </script>\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2245), Tuple.Create("\"", 2323)
, Tuple.Create(Tuple.Create("", 2251), Tuple.Create("/Scripts/DoubleGis.GlobalVariables.js?", 2251), true)
            
            #line 41 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2289), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2289), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2370), Tuple.Create("\"", 2442)
, Tuple.Create(Tuple.Create("", 2376), Tuple.Create("/Scripts/Common.ErrorHandler.js?", 2376), true)
            
            #line 42 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2408), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2408), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2489), Tuple.Create("\"", 2560)
, Tuple.Create(Tuple.Create("", 2495), Tuple.Create("/Scripts/Ext.ux.FitToParent.js?", 2495), true)
            
            #line 43 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2526), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2526), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2607), Tuple.Create("\"", 2680)
, Tuple.Create(Tuple.Create("", 2613), Tuple.Create("/Scripts/Ext.ux.SearchControl.js?", 2613), true)
            
            #line 44 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2646), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2646), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2727), Tuple.Create("\"", 2800)
, Tuple.Create(Tuple.Create("", 2733), Tuple.Create("/Scripts/Ext.ux.TabularButton.js?", 2733), true)
            
            #line 45 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2766), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2766), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2847), Tuple.Create("\"", 2925)
, Tuple.Create(Tuple.Create("", 2853), Tuple.Create("/Scripts/Ext.ux.MultiSelectionList.js?", 2853), true)
            
            #line 46 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2891), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2891), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2972), Tuple.Create("\"", 3050)
, Tuple.Create(Tuple.Create("", 2978), Tuple.Create("/Scripts/Ext.ux.SearchFormMultiple.js?", 2978), true)
            
            #line 47 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 3016), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 3016), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    \r\n");

            
            #line 49 "..\..\Views\Shared\SearchListMultiple.cshtml"
    
            
            #line default
            #line hidden
            
            #line 49 "..\..\Views\Shared\SearchListMultiple.cshtml"
     foreach (var script in Model.DataViews.First().Scripts)
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3176), Tuple.Create("\"", 3242)
, Tuple.Create(Tuple.Create("", 3182), Tuple.Create("/Scripts/", 3182), true)
            
            #line 51 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 3191), Tuple.Create<System.Object, System.Int32>(script.FileName
            
            #line default
            #line hidden
, 3191), false)
, Tuple.Create(Tuple.Create("", 3207), Tuple.Create("?", 3207), true)
            
            #line 51 "..\..\Views\Shared\SearchListMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 3208), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 3208), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 52 "..\..\Views\Shared\SearchListMultiple.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n\r\n        Ext.onReady(function ()\r\n            {\r\n            var settings = ");

            
            #line 58 "..\..\Views\Shared\SearchListMultiple.cshtml"
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
