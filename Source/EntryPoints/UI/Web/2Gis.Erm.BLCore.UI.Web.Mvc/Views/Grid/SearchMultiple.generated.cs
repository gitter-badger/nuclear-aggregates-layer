﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.0
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Grid/SearchMultiple.cshtml")]
    public partial class SearchMultiple : System.Web.Mvc.WebViewPage<Settings.ConfigurationDto.EntityViewSet>
    {
        public SearchMultiple()
        {
        }
        public override void Execute()
        {
WriteLiteral("<html");

WriteLiteral(" xmlns=\"http://www.w3.org/1999/xhtml\"");

WriteLiteral(">\r\n<head>\r\n    <title>");

            
            #line 5 "..\..\Views\Grid\SearchMultiple.cshtml"
      Write(BLResources.RecordsSearch);

            
            #line default
            #line hidden
WriteLiteral("</title>\r\n    <meta");

WriteLiteral(" id=\"meta_IsDebug\"");

WriteLiteral(" name=\"meta_IsDebug\"");

WriteAttribute("content", Tuple.Create(" content=\"", 197), Tuple.Create("\"", 246)
            
            #line 6 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 207), Tuple.Create<System.Object, System.Int32>(HttpContext.Current.IsDebuggingEnabled
            
            #line default
            #line hidden
, 207), false)
);

WriteLiteral(" />\r\n    <meta");

WriteLiteral(" id=\"meta_Revision\"");

WriteLiteral(" name=\"meta_Revision\"");

WriteAttribute("content", Tuple.Create(" content=\"", 301), Tuple.Create("\"", 330)
            
            #line 7 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 311), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 311), false)
);

WriteLiteral(" />\r\n    \r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 384), Tuple.Create("\"", 431)
, Tuple.Create(Tuple.Create("", 391), Tuple.Create("/Content/ext-all.css?", 391), true)
            
            #line 9 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 412), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 412), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 479), Tuple.Create("\"", 523)
, Tuple.Create(Tuple.Create("", 486), Tuple.Create("/Content/CRM4.css?", 486), true)
            
            #line 10 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 504), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 504), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 571), Tuple.Create("\"", 619)
, Tuple.Create(Tuple.Create("", 578), Tuple.Create("/Content/MainPage.css?", 578), true)
            
            #line 11 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 600), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 600), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 667), Tuple.Create("\"", 715)
, Tuple.Create(Tuple.Create("", 674), Tuple.Create("/Content/ext-mask.css?", 674), true)
            
            #line 12 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 696), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 696), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 763), Tuple.Create("\"", 814)
, Tuple.Create(Tuple.Create("", 770), Tuple.Create("/Content/LookupStyle.css?", 770), true)
            
            #line 13 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 795), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 795), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 862), Tuple.Create("\"", 910)
, Tuple.Create(Tuple.Create("", 869), Tuple.Create("/Content/DataList.css?", 869), true)
            
            #line 14 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 891), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 891), false)
);

WriteLiteral(" />\r\n\r\n");

            
            #line 16 "..\..\Views\Grid\SearchMultiple.cshtml"
    
            
            #line default
            #line hidden
            
            #line 16 "..\..\Views\Grid\SearchMultiple.cshtml"
     if (HttpContext.Current.IsDebuggingEnabled)
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 990), Tuple.Create("\"", 1042)
, Tuple.Create(Tuple.Create("", 996), Tuple.Create("/Scripts/ext-base-debug.js?", 996), true)
            
            #line 18 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 1023), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1023), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1093), Tuple.Create("\"", 1155)
, Tuple.Create(Tuple.Create("", 1099), Tuple.Create("/Scripts/ext-all-debug-w-comments.js?", 1099), true)
            
            #line 19 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 1136), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1136), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 20 "..\..\Views\Grid\SearchMultiple.cshtml"
    }
    else
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1230), Tuple.Create("\"", 1276)
, Tuple.Create(Tuple.Create("", 1236), Tuple.Create("/Scripts/ext-base.js?", 1236), true)
            
            #line 23 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 1257), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1257), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1327), Tuple.Create("\"", 1372)
, Tuple.Create(Tuple.Create("", 1333), Tuple.Create("/Scripts/ext-all.js?", 1333), true)
            
            #line 24 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 1353), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1353), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 25 "..\..\Views\Grid\SearchMultiple.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1428), Tuple.Create("\"", 1555)
, Tuple.Create(Tuple.Create("", 1434), Tuple.Create("/Scripts/", 1434), true)
            
            #line 27 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 1443), Tuple.Create<System.Object, System.Int32>("Ext.LocalizedResources." + ViewData.GetUserLocaleInfo().TwoLetterISOLanguageName + ".js"
            
            #line default
            #line hidden
, 1443), false)
, Tuple.Create(Tuple.Create("", 1535), Tuple.Create("?", 1535), true)
            
            #line 27 "..\..\Views\Grid\SearchMultiple.cshtml"
                                       , Tuple.Create(Tuple.Create("", 1536), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1536), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.USER_ID = ");

            
            #line 30 "..\..\Views\Grid\SearchMultiple.cshtml"
                 Write(ViewData.GetUserIdentityInfo().Code);

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.USER_NAME = ");

            
            #line 31 "..\..\Views\Grid\SearchMultiple.cshtml"
                   Write(ViewData.GetUserIdentityDisplayName());

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo = ");

            
            #line 32 "..\..\Views\Grid\SearchMultiple.cshtml"
                     Write(Html.WriteJson(ViewData.GetUserLocaleInfo()));

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo.NumberFormatInfo.CurrencySymbol = \'");

            
            #line 33 "..\..\Views\Grid\SearchMultiple.cshtml"
                                                      Write(ViewData.GetErmBaseCurrencySymbol());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.CRM_URL = \'");

            
            #line 34 "..\..\Views\Grid\SearchMultiple.cshtml"
                  Write(ViewData.GetMsCrmSettingsUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.BasicOperationsServiceRestUrl = \'");

            
            #line 35 "..\..\Views\Grid\SearchMultiple.cshtml"
                                        Write(ViewData.GetBasicOperationsServiceRestUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.SpecialOperationsServiceRestUrl = \'");

            
            #line 36 "..\..\Views\Grid\SearchMultiple.cshtml"
                                          Write(ViewData.GetSpecialOperationsServiceRestUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n    </script>\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2209), Tuple.Create("\"", 2272)
, Tuple.Create(Tuple.Create("", 2215), Tuple.Create("/Scripts/DoubleGis.GlobalVariables.js?", 2215), true)
            
            #line 39 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2253), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 2253), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2319), Tuple.Create("\"", 2376)
, Tuple.Create(Tuple.Create("", 2325), Tuple.Create("/Scripts/Common.ErrorHandler.js?", 2325), true)
            
            #line 40 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2357), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 2357), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2423), Tuple.Create("\"", 2479)
, Tuple.Create(Tuple.Create("", 2429), Tuple.Create("/Scripts/Ext.ux.FitToParent.js?", 2429), true)
            
            #line 41 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2460), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 2460), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2526), Tuple.Create("\"", 2584)
, Tuple.Create(Tuple.Create("", 2532), Tuple.Create("/Scripts/Ext.ux.SearchControl.js?", 2532), true)
            
            #line 42 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2565), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 2565), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2631), Tuple.Create("\"", 2689)
, Tuple.Create(Tuple.Create("", 2637), Tuple.Create("/Scripts/Ext.ux.TabularButton.js?", 2637), true)
            
            #line 43 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2670), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 2670), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2736), Tuple.Create("\"", 2799)
, Tuple.Create(Tuple.Create("", 2742), Tuple.Create("/Scripts/Ext.ux.MultiSelectionList.js?", 2742), true)
            
            #line 44 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2780), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 2780), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2846), Tuple.Create("\"", 2909)
, Tuple.Create(Tuple.Create("", 2852), Tuple.Create("/Scripts/Ext.ux.SearchFormMultiple.js?", 2852), true)
            
            #line 45 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 2890), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 2890), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    \r\n");

            
            #line 47 "..\..\Views\Grid\SearchMultiple.cshtml"
    
            
            #line default
            #line hidden
            
            #line 47 "..\..\Views\Grid\SearchMultiple.cshtml"
     foreach (var script in Model.DataViews.First().Scripts)
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3035), Tuple.Create("\"", 3086)
, Tuple.Create(Tuple.Create("", 3041), Tuple.Create("/Scripts/", 3041), true)
            
            #line 49 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 3050), Tuple.Create<System.Object, System.Int32>(script.FileName
            
            #line default
            #line hidden
, 3050), false)
, Tuple.Create(Tuple.Create("", 3066), Tuple.Create("?", 3066), true)
            
            #line 49 "..\..\Views\Grid\SearchMultiple.cshtml"
, Tuple.Create(Tuple.Create("", 3067), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3067), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 50 "..\..\Views\Grid\SearchMultiple.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n\r\n        Ext.onReady(function ()\r\n            {\r\n            var settings = ");

            
            #line 55 "..\..\Views\Grid\SearchMultiple.cshtml"
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
                    readOnly: qstring.ReadOnly === ""true"",
                    nameLocaleResourceId: qstring.NameLocaleResourceId,
                    honourDataListFields: qstring.HonourDataListFields === ""true""
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
