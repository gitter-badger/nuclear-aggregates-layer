﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASP
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
    using DoubleGis.Erm.BLCore.UI.Metadata.Confirmations;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models.GroupOperation;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
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
    using NuClear.Model.Common.Entities;
    using NuClear.Model.Common.Operations.Identity;
    using NuClear.Model.Common.Operations.Identity.Generic;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Shared/_CardLayout.cshtml")]
    public partial class _Views_Shared__CardLayout_cshtml : System.Web.Mvc.WebViewPage<EntityViewModelBase>
    {
        public _Views_Shared__CardLayout_cshtml()
        {
        }
        public override void Execute()
        {
WriteLiteral("<html");

WriteLiteral(" xmlns=\"http://www.w3.org/1999/xhtml\"");

WriteLiteral(">\r\n<head>\r\n    <title></title>\r\n    <base");

WriteLiteral(" target=\"_self\"");

WriteLiteral(" />\r\n\r\n    <meta");

WriteLiteral(" id=\"meta_IsDebug\"");

WriteLiteral(" name=\"meta_IsDebug\"");

WriteAttribute("content", Tuple.Create(" content=\"", 182), Tuple.Create("\"", 231)
            
            #line 8 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 192), Tuple.Create<System.Object, System.Int32>(HttpContext.Current.IsDebuggingEnabled
            
            #line default
            #line hidden
, 192), false)
);

WriteLiteral(" />\r\n    <meta");

WriteLiteral(" id=\"meta_Revision\"");

WriteLiteral(" name=\"meta_Revision\"");

WriteAttribute("content", Tuple.Create(" content=\"", 286), Tuple.Create("\"", 315)
            
            #line 9 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 296), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 296), false)
);

WriteLiteral(" />\r\n    \r\n    <link");

WriteLiteral(" rel=\"shortcut icon\"");

WriteLiteral(" type=\"image/x-icon\"");

WriteLiteral(" href=\"/favicon.ico\"");

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"icon\"");

WriteLiteral(" type=\"image/x-icon\"");

WriteLiteral(" href=\"/favicon.ico\"");

WriteLiteral("/>\r\n    \r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 513), Tuple.Create("\"", 560)
, Tuple.Create(Tuple.Create("", 520), Tuple.Create("/Content/ext-all.css?", 520), true)
            
            #line 14 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 541), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 541), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 608), Tuple.Create("\"", 652)
, Tuple.Create(Tuple.Create("", 615), Tuple.Create("/Content/CRM4.css?", 615), true)
            
            #line 15 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 633), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 633), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 700), Tuple.Create("\"", 748)
, Tuple.Create(Tuple.Create("", 707), Tuple.Create("/Content/MainPage.css?", 707), true)
            
            #line 16 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 729), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 729), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 796), Tuple.Create("\"", 844)
, Tuple.Create(Tuple.Create("", 803), Tuple.Create("/Content/ext-mask.css?", 803), true)
            
            #line 17 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 825), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 825), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 892), Tuple.Create("\"", 943)
, Tuple.Create(Tuple.Create("", 899), Tuple.Create("/Content/LookupStyle.css?", 899), true)
            
            #line 18 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 924), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 924), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 991), Tuple.Create("\"", 1047)
, Tuple.Create(Tuple.Create("", 998), Tuple.Create("/Content/ext-ux-calendar2.css?", 998), true)
            
            #line 19 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1028), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1028), false)
);

WriteLiteral(" />\r\n    \r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 1101), Tuple.Create("\"", 1145)
, Tuple.Create(Tuple.Create("", 1108), Tuple.Create("/Content/Card.css?", 1108), true)
            
            #line 21 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1126), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1126), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 1193), Tuple.Create("\"", 1243)
, Tuple.Create(Tuple.Create("", 1200), Tuple.Create("/Content/ext-extend.css?", 1200), true)
            
            #line 22 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1224), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1224), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 1291), Tuple.Create("\"", 1346)
, Tuple.Create(Tuple.Create("", 1298), Tuple.Create("/Content/AsyncFileUpload.css?", 1298), true)
            
            #line 23 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1327), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1327), false)
);

WriteLiteral(" />\r\n    \r\n");

            
            #line 25 "..\..\Views\Shared\_CardLayout.cshtml"
    
            
            #line default
            #line hidden
            
            #line 25 "..\..\Views\Shared\_CardLayout.cshtml"
     if (HttpContext.Current.IsDebuggingEnabled)
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1430), Tuple.Create("\"", 1482)
, Tuple.Create(Tuple.Create("", 1436), Tuple.Create("/Scripts/ext-base-debug.js?", 1436), true)
            
            #line 27 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1463), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1463), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1533), Tuple.Create("\"", 1595)
, Tuple.Create(Tuple.Create("", 1539), Tuple.Create("/Scripts/ext-all-debug-w-comments.js?", 1539), true)
            
            #line 28 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1576), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1576), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteLiteral(" src=\"/Scripts/moment-with-locales.js\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteLiteral(" src=\"/Scripts/moment.locale.kk-kz.js\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteLiteral(" src=\"/Scripts/moment-timezone-with-data.js\"");

WriteLiteral("></script>\r\n");

            
            #line 32 "..\..\Views\Shared\_CardLayout.cshtml"
    }
    else
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1871), Tuple.Create("\"", 1917)
, Tuple.Create(Tuple.Create("", 1877), Tuple.Create("/Scripts/ext-base.js?", 1877), true)
            
            #line 35 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1898), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1898), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1968), Tuple.Create("\"", 2013)
, Tuple.Create(Tuple.Create("", 1974), Tuple.Create("/Scripts/ext-all.js?", 1974), true)
            
            #line 36 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1994), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1994), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteLiteral(" src=\"/Scripts/moment-with-locales.min.js\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteLiteral(" src=\"/Scripts/moment.locale.kk-kz.js\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteLiteral(" src=\"/Scripts/moment-timezone-with-data.min.js\"");

WriteLiteral("></script>\r\n");

            
            #line 40 "..\..\Views\Shared\_CardLayout.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2282), Tuple.Create("\"", 2409)
, Tuple.Create(Tuple.Create("", 2288), Tuple.Create("/Scripts/", 2288), true)
            
            #line 42 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 2297), Tuple.Create<System.Object, System.Int32>("Ext.LocalizedResources." + ViewData.GetUserLocaleInfo().TwoLetterISOLanguageName + ".js"
            
            #line default
            #line hidden
, 2297), false)
, Tuple.Create(Tuple.Create("", 2389), Tuple.Create("?", 2389), true)
            
            #line 42 "..\..\Views\Shared\_CardLayout.cshtml"
                                       , Tuple.Create(Tuple.Create("", 2390), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 2390), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.USER_ID = ");

            
            #line 45 "..\..\Views\Shared\_CardLayout.cshtml"
                 Write(ViewData.GetUserIdentityInfo().Code);

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.USER_NAME = ");

            
            #line 46 "..\..\Views\Shared\_CardLayout.cshtml"
                   Write(ViewData.GetUserIdentityDisplayName());

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo = ");

            
            #line 47 "..\..\Views\Shared\_CardLayout.cshtml"
                     Write(Html.WriteJson(ViewData.GetUserLocaleInfo()));

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo.NumberFormatInfo.CurrencySymbol = \'");

            
            #line 48 "..\..\Views\Shared\_CardLayout.cshtml"
                                                      Write(ViewData.GetErmBaseCurrencySymbol());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.BasicOperationsServiceRestUrl = \'");

            
            #line 49 "..\..\Views\Shared\_CardLayout.cshtml"
                                        Write(ViewData.GetBasicOperationsServiceRestUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.SpecialOperationsServiceRestUrl = \'");

            
            #line 50 "..\..\Views\Shared\_CardLayout.cshtml"
                                          Write(ViewData.GetSpecialOperationsServiceRestUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.IdentityServiceRestUrl = \'");

            
            #line 51 "..\..\Views\Shared\_CardLayout.cshtml"
                                 Write(ViewData.GetIdentityServiceRestUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        moment.locale(\'");

            
            #line 52 "..\..\Views\Shared\_CardLayout.cshtml"
                  Write(ViewData.GetUserLocaleInfo().CultureName);

            
            #line default
            #line hidden
WriteLiteral("\');\r\n    </script>\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3153), Tuple.Create("\"", 3216)
, Tuple.Create(Tuple.Create("", 3159), Tuple.Create("/Scripts/DoubleGis.GlobalVariables.js?", 3159), true)
            
            #line 55 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3197), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3197), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3263), Tuple.Create("\"", 3322)
, Tuple.Create(Tuple.Create("", 3269), Tuple.Create("/Scripts/DoubleGis.TimeZoneMap.js?", 3269), true)
            
            #line 56 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3303), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3303), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3369), Tuple.Create("\"", 3426)
, Tuple.Create(Tuple.Create("", 3375), Tuple.Create("/Scripts/Common.ErrorHandler.js?", 3375), true)
            
            #line 57 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3407), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3407), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3473), Tuple.Create("\"", 3529)
, Tuple.Create(Tuple.Create("", 3479), Tuple.Create("/Scripts/Ext.ux.FitToParent.js?", 3479), true)
            
            #line 58 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3510), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3510), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3576), Tuple.Create("\"", 3640)
, Tuple.Create(Tuple.Create("", 3582), Tuple.Create("/Scripts/DoubleGis.MvcFormValidator.js?", 3582), true)
            
            #line 59 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3621), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3621), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3687), Tuple.Create("\"", 3752)
, Tuple.Create(Tuple.Create("", 3693), Tuple.Create("/Scripts/DoubleGis.DependencyHandler.js?", 3693), true)
            
            #line 60 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3733), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3733), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3799), Tuple.Create("\"", 3857)
, Tuple.Create(Tuple.Create("", 3805), Tuple.Create("/Scripts/Ext.Ajax.syncRequest.js?", 3805), true)
            
            #line 61 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3838), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3838), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3904), Tuple.Create("\"", 3960)
, Tuple.Create(Tuple.Create("", 3910), Tuple.Create("/Scripts/Ext.ux.LookupField.js?", 3910), true)
            
            #line 62 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3941), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3941), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4007), Tuple.Create("\"", 4067)
, Tuple.Create(Tuple.Create("", 4013), Tuple.Create("/Scripts/Ext.ux.AsyncFileUpload.js?", 4013), true)
            
            #line 63 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4048), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4048), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4114), Tuple.Create("\"", 4168)
, Tuple.Create(Tuple.Create("", 4120), Tuple.Create("/Scripts/Ext.ux.LinkField.js?", 4120), true)
            
            #line 64 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4149), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4149), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4215), Tuple.Create("\"", 4268)
, Tuple.Create(Tuple.Create("", 4221), Tuple.Create("/Scripts/Ext.ux.Calendar.js?", 4221), true)
            
            #line 65 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4249), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4249), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4315), Tuple.Create("\"", 4376)
, Tuple.Create(Tuple.Create("", 4321), Tuple.Create("/Scripts/Ext.ux.LookupFieldOwner.js?", 4321), true)
            
            #line 66 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4357), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4357), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4423), Tuple.Create("\"", 4477)
, Tuple.Create(Tuple.Create("", 4429), Tuple.Create("/Scripts/Ext.ux.NotePanel.js?", 4429), true)
            
            #line 67 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4458), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4458), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4524), Tuple.Create("\"", 4586)
, Tuple.Create(Tuple.Create("", 4530), Tuple.Create("/Scripts/Ext.ux.ActionsHistoryTab.js?", 4530), true)
            
            #line 68 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4567), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4567), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4633), Tuple.Create("\"", 4688)
, Tuple.Create(Tuple.Create("", 4639), Tuple.Create("/Scripts/DoubleGis.UI.Card.js?", 4639), true)
            
            #line 69 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4669), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4669), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4735), Tuple.Create("\"", 4787)
, Tuple.Create(Tuple.Create("", 4741), Tuple.Create("/Scripts/Ext.ux.IdField.js?", 4741), true)
            
            #line 70 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4768), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4768), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4836), Tuple.Create("\"", 4892)
, Tuple.Create(Tuple.Create("", 4842), Tuple.Create("/Scripts/Ext.ux.MonthPicker.js?", 4842), true)
            
            #line 72 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4873), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4873), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4939), Tuple.Create("\"", 4993)
, Tuple.Create(Tuple.Create("", 4945), Tuple.Create("/Scripts/Ext.ux.MonthMenu.js?", 4945), true)
            
            #line 73 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4974), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4974), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 5040), Tuple.Create("\"", 5094)
, Tuple.Create(Tuple.Create("", 5046), Tuple.Create("/Scripts/Ext.ux.Calendar2.js?", 5046), true)
            
            #line 74 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 5075), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 5075), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n");

WriteLiteral("    ");

            
            #line 76 "..\..\Views\Shared\_CardLayout.cshtml"
Write(RenderSection("CardScripts"));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n</head>\r\n    <body>\r\n");

            
            #line 80 "..\..\Views\Shared\_CardLayout.cshtml"
        
            
            #line default
            #line hidden
            
            #line 80 "..\..\Views\Shared\_CardLayout.cshtml"
         using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" }, { "autocomplete", "off" }, { "target", "_self" } }))
        {

            
            #line default
            #line hidden
WriteLiteral("            <div");

WriteLiteral(" style=\"display: none\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" id=\"ContentTab_holder\"");

WriteLiteral(" class=\"Holder\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" style=\"display: none; height: 24px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(" class=\"Notifications\"");

WriteLiteral(">\r\n                    </div>\r\n                    <div");

WriteLiteral(" id=\"MainTab_holder\"");

WriteLiteral(">\r\n                        \r\n");

WriteLiteral("                        ");

            
            #line 88 "..\..\Views\Shared\_CardLayout.cshtml"
                   Write(RenderSection("CardBody"));

            
            #line default
            #line hidden
WriteLiteral("\r\n                        \r\n");

            
            #line 90 "..\..\Views\Shared\_CardLayout.cshtml"
                        
            
            #line default
            #line hidden
            
            #line 90 "..\..\Views\Shared\_CardLayout.cshtml"
                         if (Model is IEntityViewModelBase && Model.ViewConfig.CardSettings.HasAdminTab)
                        {
                            Html.RenderPartial("AdministrationTab");
                        }

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n\r\n");

WriteLiteral("                    ");

            
            #line 97 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.CheckBoxFor(x => x.ViewConfig.ReadOnly));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 98 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.Hidden("ViewConfig.EntityName", Model.ViewConfig.EntityName.Description));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 99 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.ViewConfig.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 100 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.ViewConfig.PId));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 101 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.Hidden("ViewConfig.PType", Model.ViewConfig.PType.Description));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 102 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.ViewConfig.ExtendedInfo));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 103 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.IsNew));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 104 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.Hidden("EntityStatus", ViewData["EntityStatus"]));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 105 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.Hidden("ViewConfig.DependencyList", Model.ViewConfig.DependencyList));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 106 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.EntityStateToken));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 107 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.IsDeleted));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 108 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.IsActive));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n                    <div");

WriteLiteral(" id=\"MessageType\"");

WriteLiteral(">");

            
            #line 110 "..\..\Views\Shared\_CardLayout.cshtml"
                                     Write(Model.MessageType);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                    <div");

WriteLiteral(" id=\"Message\"");

WriteLiteral(">");

            
            #line 111 "..\..\Views\Shared\_CardLayout.cshtml"
                                 Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                </div>\r\n            </div>            \r\n");

            
            #line 114 "..\..\Views\Shared\_CardLayout.cshtml"
        }   

            
            #line default
            #line hidden
WriteLiteral("        \r\n");

            
            #line 116 "..\..\Views\Shared\_CardLayout.cshtml"
        
            
            #line default
            #line hidden
            
            #line 116 "..\..\Views\Shared\_CardLayout.cshtml"
         if (IsSectionDefined("CustomInit"))
        {
            
            
            #line default
            #line hidden
            
            #line 118 "..\..\Views\Shared\_CardLayout.cshtml"
       Write(RenderSection("CustomInit"));

            
            #line default
            #line hidden
            
            #line 118 "..\..\Views\Shared\_CardLayout.cshtml"
                                        ;
        }
        else
        {

            
            #line default
            #line hidden
WriteLiteral("            <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n                Ext.onReady(function()\r\n                {\r\n                   " +
" \r\n                    var cardSettings = ");

            
            #line 126 "..\..\Views\Shared\_CardLayout.cshtml"
                                  Write(Html.WriteJson(Model.ViewConfig.CardSettings));

            
            #line default
            #line hidden
WriteLiteral(";\r\n                    window.Card = new window.Ext.DoubleGis.UI.SharableCard(car" +
"dSettings);\r\n                    window.Card.Build();\r\n                    \r\n   " +
"             });\r\n            </script>\r\n");

            
            #line 132 "..\..\Views\Shared\_CardLayout.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </body>\r\n</html>\r\n");

        }
    }
}
#pragma warning restore 1591
