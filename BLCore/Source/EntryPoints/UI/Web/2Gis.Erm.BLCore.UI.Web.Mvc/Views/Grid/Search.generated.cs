﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Views.Grid
{
    using System;
    using System.Linq;
    using System.Web;

    using DoubleGis.Erm.BLCore.Resources.Server.Properties;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Security;

    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Grid/Search.cshtml")]
    public partial class Search : System.Web.Mvc.WebViewPage<Settings.ConfigurationDto.EntityViewSet>
    {
        public Search()
        {
        }
        public override void Execute()
        {
WriteLiteral("<html");

WriteLiteral(" xmlns=\"http://www.w3.org/1999/xhtml\"");

WriteLiteral(">\r\n<head>\r\n    <title>");

            
            #line 5 "..\..\Views\Grid\Search.cshtml"
      Write(BLResources.RecordsSearch);

            
            #line default
            #line hidden
WriteLiteral("</title>\r\n    <meta");

WriteLiteral(" id=\"meta_IsDebug\"");

WriteLiteral(" name=\"meta_IsDebug\"");

WriteAttribute("content", Tuple.Create(" content=\"", 197), Tuple.Create("\"", 246)
            
            #line 6 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 207), Tuple.Create<System.Object, System.Int32>(HttpContext.Current.IsDebuggingEnabled
            
            #line default
            #line hidden
, 207), false)
);

WriteLiteral(" />\r\n    <meta");

WriteLiteral(" id=\"meta_Revision\"");

WriteLiteral(" name=\"meta_Revision\"");

WriteAttribute("content", Tuple.Create(" content=\"", 301), Tuple.Create("\"", 330)
            
            #line 7 "..\..\Views\Grid\Search.cshtml"
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
            
            #line 9 "..\..\Views\Grid\Search.cshtml"
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
            
            #line 10 "..\..\Views\Grid\Search.cshtml"
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
            
            #line 11 "..\..\Views\Grid\Search.cshtml"
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
            
            #line 12 "..\..\Views\Grid\Search.cshtml"
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
            
            #line 13 "..\..\Views\Grid\Search.cshtml"
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
            
            #line 14 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 891), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 891), false)
);

WriteLiteral(" />\r\n\r\n");

            
            #line 16 "..\..\Views\Grid\Search.cshtml"
    
            
            #line default
            #line hidden
            
            #line 16 "..\..\Views\Grid\Search.cshtml"
     if (HttpContext.Current.IsDebuggingEnabled)
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 990), Tuple.Create("\"", 1042)
, Tuple.Create(Tuple.Create("", 996), Tuple.Create("/Scripts/ext-base-debug.js?", 996), true)
            
            #line 18 "..\..\Views\Grid\Search.cshtml"
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
            
            #line 19 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 1136), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1136), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 20 "..\..\Views\Grid\Search.cshtml"
    }
    else
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1230), Tuple.Create("\"", 1276)
, Tuple.Create(Tuple.Create("", 1236), Tuple.Create("/Scripts/ext-base.js?", 1236), true)
            
            #line 23 "..\..\Views\Grid\Search.cshtml"
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
            
            #line 24 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 1353), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1353), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 25 "..\..\Views\Grid\Search.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1428), Tuple.Create("\"", 1555)
, Tuple.Create(Tuple.Create("", 1434), Tuple.Create("/Scripts/", 1434), true)
            
            #line 27 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 1443), Tuple.Create<System.Object, System.Int32>("Ext.LocalizedResources." + ViewData.GetUserLocaleInfo().TwoLetterISOLanguageName + ".js"
            
            #line default
            #line hidden
, 1443), false)
, Tuple.Create(Tuple.Create("", 1535), Tuple.Create("?", 1535), true)
            
            #line 27 "..\..\Views\Grid\Search.cshtml"
                                       , Tuple.Create(Tuple.Create("", 1536), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1536), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.USER_ID = ");

            
            #line 30 "..\..\Views\Grid\Search.cshtml"
                 Write(ViewData.GetUserIdentityInfo().Code);

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.USER_NAME = ");

            
            #line 31 "..\..\Views\Grid\Search.cshtml"
                   Write(ViewData.GetUserIdentityDisplayName());

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo = ");

            
            #line 32 "..\..\Views\Grid\Search.cshtml"
                     Write(Html.WriteJson(ViewData.GetUserLocaleInfo()));

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo.NumberFormatInfo.CurrencySymbol = \'");

            
            #line 33 "..\..\Views\Grid\Search.cshtml"
                                                      Write(ViewData.GetErmBaseCurrencySymbol());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.BasicOperationsServiceRestUrl = \'");

            
            #line 34 "..\..\Views\Grid\Search.cshtml"
                                        Write(ViewData.GetBasicOperationsServiceRestUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.SpecialOperationsServiceRestUrl = \'");

            
            #line 35 "..\..\Views\Grid\Search.cshtml"
                                          Write(ViewData.GetSpecialOperationsServiceRestUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n    </script>\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2151), Tuple.Create("\"", 2214)
, Tuple.Create(Tuple.Create("", 2157), Tuple.Create("/Scripts/DoubleGis.GlobalVariables.js?", 2157), true)
            
            #line 38 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 2195), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 2195), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2261), Tuple.Create("\"", 2318)
, Tuple.Create(Tuple.Create("", 2267), Tuple.Create("/Scripts/Common.ErrorHandler.js?", 2267), true)
            
            #line 39 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 2299), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 2299), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2365), Tuple.Create("\"", 2421)
, Tuple.Create(Tuple.Create("", 2371), Tuple.Create("/Scripts/Ext.ux.FitToParent.js?", 2371), true)
            
            #line 40 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 2402), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 2402), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2468), Tuple.Create("\"", 2526)
, Tuple.Create(Tuple.Create("", 2474), Tuple.Create("/Scripts/Ext.ux.SearchControl.js?", 2474), true)
            
            #line 41 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 2507), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 2507), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2573), Tuple.Create("\"", 2631)
, Tuple.Create(Tuple.Create("", 2579), Tuple.Create("/Scripts/Ext.ux.TabularButton.js?", 2579), true)
            
            #line 42 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 2612), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 2612), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2678), Tuple.Create("\"", 2735)
, Tuple.Create(Tuple.Create("", 2684), Tuple.Create("/Scripts/Ext.DoubleGis.Store.js?", 2684), true)
            
            #line 43 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 2716), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 2716), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2782), Tuple.Create("\"", 2837)
, Tuple.Create(Tuple.Create("", 2788), Tuple.Create("/Scripts/Ext.ux.SearchForm.js?", 2788), true)
            
            #line 44 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 2818), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 2818), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    \r\n");

            
            #line 46 "..\..\Views\Grid\Search.cshtml"
    
            
            #line default
            #line hidden
            
            #line 46 "..\..\Views\Grid\Search.cshtml"
     foreach (var script in Model.DataViews.First().Scripts)
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2963), Tuple.Create("\"", 3014)
, Tuple.Create(Tuple.Create("", 2969), Tuple.Create("/Scripts/", 2969), true)
            
            #line 48 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 2978), Tuple.Create<System.Object, System.Int32>(script.FileName
            
            #line default
            #line hidden
, 2978), false)
, Tuple.Create(Tuple.Create("", 2994), Tuple.Create("?", 2994), true)
            
            #line 48 "..\..\Views\Grid\Search.cshtml"
, Tuple.Create(Tuple.Create("", 2995), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 2995), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 49 "..\..\Views\Grid\Search.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.onReady(function ()\r\n            {\r\n            var settings = ");

            
            #line 53 "..\..\Views\Grid\Search.cshtml"
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
                    readOnly: qstring.ReadOnly === ""true"",
                    nameLocaleResourceId: qstring.NameLocaleResourceId
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
