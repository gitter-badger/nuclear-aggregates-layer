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
    using System.Linq;
    using System.Web;

    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
    using DoubleGis.Erm.Platform.Common;
#line 1 "..\..\Views\Shared\SearchList.cshtml"
    using Platform.UI.Web.Mvc.Security;
    
    #line default
    #line hidden
    
    #line 2 "..\..\Views\Shared\SearchList.cshtml"
    using Settings;
    
    #line default
    #line hidden
    
    #line 3 "..\..\Views\Shared\SearchList.cshtml"
    using UserProfiles;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Shared/SearchList.cshtml")]
    public partial class SearchList : System.Web.Mvc.WebViewPage<Settings.ConfigurationDto.EntityViewSet>
    {
        public SearchList()
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

WriteAttribute("content", Tuple.Create(" content=\"", 272), Tuple.Create("\"", 321)
            
            #line 10 "..\..\Views\Shared\SearchList.cshtml"
, Tuple.Create(Tuple.Create("", 282), Tuple.Create<System.Object, System.Int32>(HttpContext.Current.IsDebuggingEnabled
            
            #line default
            #line hidden
, 282), false)
);

WriteLiteral(" />\r\n    <meta");

WriteLiteral(" id=\"meta_Revision\"");

WriteLiteral(" name=\"meta_Revision\"");

WriteAttribute("content", Tuple.Create(" content=\"", 376), Tuple.Create("\"", 420)
            
            #line 11 "..\..\Views\Shared\SearchList.cshtml"
, Tuple.Create(Tuple.Create("", 386), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 386), false)
);

WriteLiteral(" />\r\n    \r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 474), Tuple.Create("\"", 536)
, Tuple.Create(Tuple.Create("", 481), Tuple.Create("/Content/ext-all.css?", 481), true)
            
            #line 13 "..\..\Views\Shared\SearchList.cshtml"
, Tuple.Create(Tuple.Create("", 502), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 502), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 584), Tuple.Create("\"", 643)
, Tuple.Create(Tuple.Create("", 591), Tuple.Create("/Content/CRM4.css?", 591), true)
            
            #line 14 "..\..\Views\Shared\SearchList.cshtml"
, Tuple.Create(Tuple.Create("", 609), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 609), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 691), Tuple.Create("\"", 754)
, Tuple.Create(Tuple.Create("", 698), Tuple.Create("/Content/MainPage.css?", 698), true)
            
            #line 15 "..\..\Views\Shared\SearchList.cshtml"
, Tuple.Create(Tuple.Create("", 720), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 720), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 802), Tuple.Create("\"", 865)
, Tuple.Create(Tuple.Create("", 809), Tuple.Create("/Content/ext-mask.css?", 809), true)
            
            #line 16 "..\..\Views\Shared\SearchList.cshtml"
, Tuple.Create(Tuple.Create("", 831), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 831), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 913), Tuple.Create("\"", 979)
, Tuple.Create(Tuple.Create("", 920), Tuple.Create("/Content/LookupStyle.css?", 920), true)
            
            #line 17 "..\..\Views\Shared\SearchList.cshtml"
, Tuple.Create(Tuple.Create("", 945), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 945), false)
);

WriteLiteral(" />\r\n    \r\n");

            
            #line 19 "..\..\Views\Shared\SearchList.cshtml"
    
            
            #line default
            #line hidden
            
            #line 19 "..\..\Views\Shared\SearchList.cshtml"
     if (HttpContext.Current.IsDebuggingEnabled)
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1063), Tuple.Create("\"", 1130)
, Tuple.Create(Tuple.Create("", 1069), Tuple.Create("/Scripts/ext-base-debug.js?", 1069), true)
            
            #line 21 "..\..\Views\Shared\SearchList.cshtml"
, Tuple.Create(Tuple.Create("", 1096), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1096), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1181), Tuple.Create("\"", 1258)
, Tuple.Create(Tuple.Create("", 1187), Tuple.Create("/Scripts/ext-all-debug-w-comments.js?", 1187), true)
            
            #line 22 "..\..\Views\Shared\SearchList.cshtml"
, Tuple.Create(Tuple.Create("", 1224), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1224), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 23 "..\..\Views\Shared\SearchList.cshtml"
    }
    else
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1333), Tuple.Create("\"", 1394)
, Tuple.Create(Tuple.Create("", 1339), Tuple.Create("/Scripts/ext-base.js?", 1339), true)
            
            #line 26 "..\..\Views\Shared\SearchList.cshtml"
, Tuple.Create(Tuple.Create("", 1360), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1360), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1445), Tuple.Create("\"", 1505)
, Tuple.Create(Tuple.Create("", 1451), Tuple.Create("/Scripts/ext-all.js?", 1451), true)
            
            #line 27 "..\..\Views\Shared\SearchList.cshtml"
, Tuple.Create(Tuple.Create("", 1471), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1471), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 28 "..\..\Views\Shared\SearchList.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1561), Tuple.Create("\"", 1703)
, Tuple.Create(Tuple.Create("", 1567), Tuple.Create("/Scripts/", 1567), true)
            
            #line 30 "..\..\Views\Shared\SearchList.cshtml"
, Tuple.Create(Tuple.Create("", 1576), Tuple.Create<System.Object, System.Int32>("Ext.LocalizedResources." + ViewData.GetUserLocaleInfo().TwoLetterISOLanguageName + ".js"
            
            #line default
            #line hidden
, 1576), false)
, Tuple.Create(Tuple.Create("", 1668), Tuple.Create("?", 1668), true)
            
            #line 30 "..\..\Views\Shared\SearchList.cshtml"
                                       , Tuple.Create(Tuple.Create("", 1669), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1669), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.USER_ID = ");

            
            #line 33 "..\..\Views\Shared\SearchList.cshtml"
                 Write(ViewData.GetUserIdentityInfo().Code);

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.USER_NAME = ");

            
            #line 34 "..\..\Views\Shared\SearchList.cshtml"
                   Write(ViewData.GetUserIdentityDisplayName());

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo = ");

            
            #line 35 "..\..\Views\Shared\SearchList.cshtml"
                     Write(Html.WriteJson(ViewData.GetUserLocaleInfo()));

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo.NumberFormatInfo.CurrencySymbol = \'");

            
            #line 36 "..\..\Views\Shared\SearchList.cshtml"
                                                      Write(ViewData.GetErmBaseCurrencySymbol());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.CRM_URL = \'");

            
            #line 37 "..\..\Views\Shared\SearchList.cshtml"
                  Write(ViewData.GetMsCrmSettingsUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.BasicOperationsServiceRestUrl = \'");

            
            #line 38 "..\..\Views\Shared\SearchList.cshtml"
                                        Write(ViewData.GetBasicOperationsServiceRestUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n    </script>\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2260), Tuple.Create("\"", 2338)
, Tuple.Create(Tuple.Create("", 2266), Tuple.Create("/Scripts/DoubleGis.GlobalVariables.js?", 2266), true)
            
            #line 41 "..\..\Views\Shared\SearchList.cshtml"
, Tuple.Create(Tuple.Create("", 2304), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2304), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2385), Tuple.Create("\"", 2457)
, Tuple.Create(Tuple.Create("", 2391), Tuple.Create("/Scripts/Common.ErrorHandler.js?", 2391), true)
            
            #line 42 "..\..\Views\Shared\SearchList.cshtml"
, Tuple.Create(Tuple.Create("", 2423), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2423), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2504), Tuple.Create("\"", 2575)
, Tuple.Create(Tuple.Create("", 2510), Tuple.Create("/Scripts/Ext.ux.FitToParent.js?", 2510), true)
            
            #line 43 "..\..\Views\Shared\SearchList.cshtml"
, Tuple.Create(Tuple.Create("", 2541), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2541), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2622), Tuple.Create("\"", 2695)
, Tuple.Create(Tuple.Create("", 2628), Tuple.Create("/Scripts/Ext.ux.SearchControl.js?", 2628), true)
            
            #line 44 "..\..\Views\Shared\SearchList.cshtml"
, Tuple.Create(Tuple.Create("", 2661), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2661), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2742), Tuple.Create("\"", 2815)
, Tuple.Create(Tuple.Create("", 2748), Tuple.Create("/Scripts/Ext.ux.TabularButton.js?", 2748), true)
            
            #line 45 "..\..\Views\Shared\SearchList.cshtml"
, Tuple.Create(Tuple.Create("", 2781), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2781), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2862), Tuple.Create("\"", 2932)
, Tuple.Create(Tuple.Create("", 2868), Tuple.Create("/Scripts/Ext.ux.SearchForm.js?", 2868), true)
            
            #line 46 "..\..\Views\Shared\SearchList.cshtml"
, Tuple.Create(Tuple.Create("", 2898), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2898), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n");

            
            #line 48 "..\..\Views\Shared\SearchList.cshtml"
    
            
            #line default
            #line hidden
            
            #line 48 "..\..\Views\Shared\SearchList.cshtml"
     foreach (var script in Model.DataViews.First().Scripts)
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3054), Tuple.Create("\"", 3120)
, Tuple.Create(Tuple.Create("", 3060), Tuple.Create("/Scripts/", 3060), true)
            
            #line 50 "..\..\Views\Shared\SearchList.cshtml"
, Tuple.Create(Tuple.Create("", 3069), Tuple.Create<System.Object, System.Int32>(script.FileName
            
            #line default
            #line hidden
, 3069), false)
, Tuple.Create(Tuple.Create("", 3085), Tuple.Create("?", 3085), true)
            
            #line 50 "..\..\Views\Shared\SearchList.cshtml"
, Tuple.Create(Tuple.Create("", 3086), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 3086), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 51 "..\..\Views\Shared\SearchList.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.onReady(function ()\r\n            {\r\n            var settings = ");

            
            #line 56 "..\..\Views\Shared\SearchList.cshtml"
                      Write(Html.WriteJson(Model));

            
            #line default
            #line hidden
WriteLiteral(@";
            var qstring = Ext.urlDecode(location.search.substring(1));
            window.Entity = new Ext.ux.SearchForm(
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
