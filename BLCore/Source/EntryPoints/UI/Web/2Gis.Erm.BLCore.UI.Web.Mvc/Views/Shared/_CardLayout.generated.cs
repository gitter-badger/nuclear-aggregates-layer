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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Shared/_CardLayout.cshtml")]
    public partial class CardLayout : System.Web.Mvc.WebViewPage<EntityViewModelBase>
    {
        public CardLayout()
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

WriteLiteral(" src=\"/Scripts/moment-timezone-with-data.js\"");

WriteLiteral("></script>\r\n");

            
            #line 31 "..\..\Views\Shared\_CardLayout.cshtml"
    }
    else
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1806), Tuple.Create("\"", 1852)
, Tuple.Create(Tuple.Create("", 1812), Tuple.Create("/Scripts/ext-base.js?", 1812), true)
            
            #line 34 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1833), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1833), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1903), Tuple.Create("\"", 1948)
, Tuple.Create(Tuple.Create("", 1909), Tuple.Create("/Scripts/ext-all.js?", 1909), true)
            
            #line 35 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1929), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1929), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteLiteral(" src=\"/Scripts/moment-with-locales.min.js\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteLiteral(" src=\"/Scripts/moment-timezone-with-data.min.js\"");

WriteLiteral("></script>\r\n");

            
            #line 38 "..\..\Views\Shared\_CardLayout.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2152), Tuple.Create("\"", 2279)
, Tuple.Create(Tuple.Create("", 2158), Tuple.Create("/Scripts/", 2158), true)
            
            #line 40 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 2167), Tuple.Create<System.Object, System.Int32>("Ext.LocalizedResources." + ViewData.GetUserLocaleInfo().TwoLetterISOLanguageName + ".js"
            
            #line default
            #line hidden
, 2167), false)
, Tuple.Create(Tuple.Create("", 2259), Tuple.Create("?", 2259), true)
            
            #line 40 "..\..\Views\Shared\_CardLayout.cshtml"
                                       , Tuple.Create(Tuple.Create("", 2260), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 2260), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.USER_ID = ");

            
            #line 43 "..\..\Views\Shared\_CardLayout.cshtml"
                 Write(ViewData.GetUserIdentityInfo().Code);

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.USER_NAME = ");

            
            #line 44 "..\..\Views\Shared\_CardLayout.cshtml"
                   Write(ViewData.GetUserIdentityDisplayName());

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo = ");

            
            #line 45 "..\..\Views\Shared\_CardLayout.cshtml"
                     Write(Html.WriteJson(ViewData.GetUserLocaleInfo()));

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo.NumberFormatInfo.CurrencySymbol = \'");

            
            #line 46 "..\..\Views\Shared\_CardLayout.cshtml"
                                                      Write(ViewData.GetErmBaseCurrencySymbol());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.CRM_URL = \'");

            
            #line 47 "..\..\Views\Shared\_CardLayout.cshtml"
                  Write(ViewData.GetMsCrmSettingsUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.BasicOperationsServiceRestUrl = \'");

            
            #line 48 "..\..\Views\Shared\_CardLayout.cshtml"
                                        Write(ViewData.GetBasicOperationsServiceRestUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.SpecialOperationsServiceRestUrl = \'");

            
            #line 49 "..\..\Views\Shared\_CardLayout.cshtml"
                                          Write(ViewData.GetSpecialOperationsServiceRestUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n    </script>\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2933), Tuple.Create("\"", 2996)
, Tuple.Create(Tuple.Create("", 2939), Tuple.Create("/Scripts/DoubleGis.GlobalVariables.js?", 2939), true)
            
            #line 52 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 2977), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 2977), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3043), Tuple.Create("\"", 3102)
, Tuple.Create(Tuple.Create("", 3049), Tuple.Create("/Scripts/DoubleGis.TimeZoneMap.js?", 3049), true)
            
            #line 53 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3083), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3083), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3149), Tuple.Create("\"", 3206)
, Tuple.Create(Tuple.Create("", 3155), Tuple.Create("/Scripts/Common.ErrorHandler.js?", 3155), true)
            
            #line 54 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3187), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3187), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3253), Tuple.Create("\"", 3309)
, Tuple.Create(Tuple.Create("", 3259), Tuple.Create("/Scripts/Ext.ux.FitToParent.js?", 3259), true)
            
            #line 55 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3290), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3290), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3356), Tuple.Create("\"", 3420)
, Tuple.Create(Tuple.Create("", 3362), Tuple.Create("/Scripts/DoubleGis.MvcFormValidator.js?", 3362), true)
            
            #line 56 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3401), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3401), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3467), Tuple.Create("\"", 3532)
, Tuple.Create(Tuple.Create("", 3473), Tuple.Create("/Scripts/DoubleGis.DependencyHandler.js?", 3473), true)
            
            #line 57 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3513), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3513), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3579), Tuple.Create("\"", 3637)
, Tuple.Create(Tuple.Create("", 3585), Tuple.Create("/Scripts/Ext.Ajax.syncRequest.js?", 3585), true)
            
            #line 58 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3618), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3618), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3684), Tuple.Create("\"", 3740)
, Tuple.Create(Tuple.Create("", 3690), Tuple.Create("/Scripts/Ext.ux.LookupField.js?", 3690), true)
            
            #line 59 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3721), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3721), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3787), Tuple.Create("\"", 3847)
, Tuple.Create(Tuple.Create("", 3793), Tuple.Create("/Scripts/Ext.ux.AsyncFileUpload.js?", 3793), true)
            
            #line 60 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3828), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3828), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3894), Tuple.Create("\"", 3948)
, Tuple.Create(Tuple.Create("", 3900), Tuple.Create("/Scripts/Ext.ux.LinkField.js?", 3900), true)
            
            #line 61 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3929), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3929), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3995), Tuple.Create("\"", 4048)
, Tuple.Create(Tuple.Create("", 4001), Tuple.Create("/Scripts/Ext.ux.Calendar.js?", 4001), true)
            
            #line 62 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4029), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4029), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4095), Tuple.Create("\"", 4156)
, Tuple.Create(Tuple.Create("", 4101), Tuple.Create("/Scripts/Ext.ux.LookupFieldOwner.js?", 4101), true)
            
            #line 63 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4137), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4137), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4203), Tuple.Create("\"", 4257)
, Tuple.Create(Tuple.Create("", 4209), Tuple.Create("/Scripts/Ext.ux.NotePanel.js?", 4209), true)
            
            #line 64 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4238), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4238), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4304), Tuple.Create("\"", 4366)
, Tuple.Create(Tuple.Create("", 4310), Tuple.Create("/Scripts/Ext.ux.ActionsHistoryTab.js?", 4310), true)
            
            #line 65 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4347), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4347), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4413), Tuple.Create("\"", 4468)
, Tuple.Create(Tuple.Create("", 4419), Tuple.Create("/Scripts/DoubleGis.UI.Card.js?", 4419), true)
            
            #line 66 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4449), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4449), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4515), Tuple.Create("\"", 4567)
, Tuple.Create(Tuple.Create("", 4521), Tuple.Create("/Scripts/Ext.ux.IdField.js?", 4521), true)
            
            #line 67 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4548), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4548), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4616), Tuple.Create("\"", 4672)
, Tuple.Create(Tuple.Create("", 4622), Tuple.Create("/Scripts/Ext.ux.MonthPicker.js?", 4622), true)
            
            #line 69 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4653), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4653), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4719), Tuple.Create("\"", 4773)
, Tuple.Create(Tuple.Create("", 4725), Tuple.Create("/Scripts/Ext.ux.MonthMenu.js?", 4725), true)
            
            #line 70 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4754), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4754), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4820), Tuple.Create("\"", 4874)
, Tuple.Create(Tuple.Create("", 4826), Tuple.Create("/Scripts/Ext.ux.Calendar2.js?", 4826), true)
            
            #line 71 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4855), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4855), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n");

WriteLiteral("    ");

            
            #line 73 "..\..\Views\Shared\_CardLayout.cshtml"
Write(RenderSection("CardScripts"));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n</head>\r\n    <body>\r\n");

            
            #line 77 "..\..\Views\Shared\_CardLayout.cshtml"
        
            
            #line default
            #line hidden
            
            #line 77 "..\..\Views\Shared\_CardLayout.cshtml"
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

            
            #line 85 "..\..\Views\Shared\_CardLayout.cshtml"
                   Write(RenderSection("CardBody"));

            
            #line default
            #line hidden
WriteLiteral("\r\n                        \r\n");

            
            #line 87 "..\..\Views\Shared\_CardLayout.cshtml"
                        
            
            #line default
            #line hidden
            
            #line 87 "..\..\Views\Shared\_CardLayout.cshtml"
                         if (Model is IEntityViewModelBase && Model.ViewConfig.CardSettings.HasAdminTab)
                        {
                            Html.RenderPartial("AdministrationTab");
                        }

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n\r\n");

WriteLiteral("                    ");

            
            #line 94 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.CheckBoxFor(x => x.ViewConfig.ReadOnly));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 95 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.ViewConfig.EntityName));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 96 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.ViewConfig.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 97 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.ViewConfig.PId));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 98 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.ViewConfig.PType));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 99 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.ViewConfig.ExtendedInfo));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 100 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.IsNew));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 101 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.Hidden("EntityStatus", ViewData["EntityStatus"]));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 102 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.Hidden("ViewConfig_DependencyList", Model.ViewConfig.DependencyList));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 103 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.EntityStateToken));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 104 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.IsDeleted));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 105 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.IsActive));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n                    <div");

WriteLiteral(" id=\"MessageType\"");

WriteLiteral(">");

            
            #line 107 "..\..\Views\Shared\_CardLayout.cshtml"
                                     Write(Model.MessageType);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                    <div");

WriteLiteral(" id=\"Message\"");

WriteLiteral(">");

            
            #line 108 "..\..\Views\Shared\_CardLayout.cshtml"
                                 Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                </div>\r\n            </div>            \r\n");

            
            #line 111 "..\..\Views\Shared\_CardLayout.cshtml"
        }   

            
            #line default
            #line hidden
WriteLiteral("        \r\n");

            
            #line 113 "..\..\Views\Shared\_CardLayout.cshtml"
        
            
            #line default
            #line hidden
            
            #line 113 "..\..\Views\Shared\_CardLayout.cshtml"
         if (IsSectionDefined("CustomInit"))
        {
            
            
            #line default
            #line hidden
            
            #line 115 "..\..\Views\Shared\_CardLayout.cshtml"
       Write(RenderSection("CustomInit"));

            
            #line default
            #line hidden
            
            #line 115 "..\..\Views\Shared\_CardLayout.cshtml"
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

            
            #line 123 "..\..\Views\Shared\_CardLayout.cshtml"
                                  Write(Html.WriteJson(Model.ViewConfig.CardSettings));

            
            #line default
            #line hidden
WriteLiteral(";\r\n                    window.Card = new window.Ext.DoubleGis.UI.SharableCard(car" +
"dSettings);\r\n                    window.Card.Build();\r\n                    \r\n   " +
"             });\r\n            </script>\r\n");

            
            #line 129 "..\..\Views\Shared\_CardLayout.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </body>\r\n</html>\r\n");

        }
    }
}
#pragma warning restore 1591
