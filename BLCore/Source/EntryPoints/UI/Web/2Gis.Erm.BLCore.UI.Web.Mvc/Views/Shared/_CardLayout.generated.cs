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

WriteAttribute("content", Tuple.Create(" content=\"", 183), Tuple.Create("\"", 232)
            
            #line 8 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 193), Tuple.Create<System.Object, System.Int32>(HttpContext.Current.IsDebuggingEnabled
            
            #line default
            #line hidden
, 193), false)
);

WriteLiteral(" />\r\n    <meta");

WriteLiteral(" id=\"meta_Revision\"");

WriteLiteral(" name=\"meta_Revision\"");

WriteAttribute("content", Tuple.Create(" content=\"", 287), Tuple.Create("\"", 316)
            
            #line 9 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 297), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 297), false)
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

WriteAttribute("href", Tuple.Create(" href=\"", 514), Tuple.Create("\"", 561)
, Tuple.Create(Tuple.Create("", 521), Tuple.Create("/Content/ext-all.css?", 521), true)
            
            #line 14 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 542), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 542), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 609), Tuple.Create("\"", 653)
, Tuple.Create(Tuple.Create("", 616), Tuple.Create("/Content/CRM4.css?", 616), true)
            
            #line 15 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 634), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 634), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 701), Tuple.Create("\"", 749)
, Tuple.Create(Tuple.Create("", 708), Tuple.Create("/Content/MainPage.css?", 708), true)
            
            #line 16 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 730), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 730), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 797), Tuple.Create("\"", 845)
, Tuple.Create(Tuple.Create("", 804), Tuple.Create("/Content/ext-mask.css?", 804), true)
            
            #line 17 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 826), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 826), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 893), Tuple.Create("\"", 944)
, Tuple.Create(Tuple.Create("", 900), Tuple.Create("/Content/LookupStyle.css?", 900), true)
            
            #line 18 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 925), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 925), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 992), Tuple.Create("\"", 1048)
, Tuple.Create(Tuple.Create("", 999), Tuple.Create("/Content/ext-ux-calendar2.css?", 999), true)
            
            #line 19 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1029), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1029), false)
);

WriteLiteral(" />\r\n    \r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 1102), Tuple.Create("\"", 1146)
, Tuple.Create(Tuple.Create("", 1109), Tuple.Create("/Content/Card.css?", 1109), true)
            
            #line 21 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1127), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1127), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 1194), Tuple.Create("\"", 1244)
, Tuple.Create(Tuple.Create("", 1201), Tuple.Create("/Content/ext-extend.css?", 1201), true)
            
            #line 22 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1225), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1225), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 1292), Tuple.Create("\"", 1347)
, Tuple.Create(Tuple.Create("", 1299), Tuple.Create("/Content/AsyncFileUpload.css?", 1299), true)
            
            #line 23 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1328), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1328), false)
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

WriteAttribute("src", Tuple.Create(" src=\"", 1431), Tuple.Create("\"", 1483)
, Tuple.Create(Tuple.Create("", 1437), Tuple.Create("/Scripts/ext-base-debug.js?", 1437), true)
            
            #line 27 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1464), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1464), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1534), Tuple.Create("\"", 1596)
, Tuple.Create(Tuple.Create("", 1540), Tuple.Create("/Scripts/ext-all-debug-w-comments.js?", 1540), true)
            
            #line 28 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1577), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1577), false)
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

WriteAttribute("src", Tuple.Create(" src=\"", 1872), Tuple.Create("\"", 1918)
, Tuple.Create(Tuple.Create("", 1878), Tuple.Create("/Scripts/ext-base.js?", 1878), true)
            
            #line 35 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1899), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1899), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1969), Tuple.Create("\"", 2014)
, Tuple.Create(Tuple.Create("", 1975), Tuple.Create("/Scripts/ext-all.js?", 1975), true)
            
            #line 36 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1995), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1995), false)
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

WriteAttribute("src", Tuple.Create(" src=\"", 2283), Tuple.Create("\"", 2410)
, Tuple.Create(Tuple.Create("", 2289), Tuple.Create("/Scripts/", 2289), true)
            
            #line 42 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 2298), Tuple.Create<System.Object, System.Int32>("Ext.LocalizedResources." + ViewData.GetUserLocaleInfo().TwoLetterISOLanguageName + ".js"
            
            #line default
            #line hidden
, 2298), false)
, Tuple.Create(Tuple.Create("", 2390), Tuple.Create("?", 2390), true)
            
            #line 42 "..\..\Views\Shared\_CardLayout.cshtml"
                                       , Tuple.Create(Tuple.Create("", 2391), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 2391), false)
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

WriteAttribute("src", Tuple.Create(" src=\"", 3154), Tuple.Create("\"", 3217)
, Tuple.Create(Tuple.Create("", 3160), Tuple.Create("/Scripts/DoubleGis.GlobalVariables.js?", 3160), true)
            
            #line 55 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3198), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3198), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3264), Tuple.Create("\"", 3323)
, Tuple.Create(Tuple.Create("", 3270), Tuple.Create("/Scripts/DoubleGis.TimeZoneMap.js?", 3270), true)
            
            #line 56 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3304), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3304), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3370), Tuple.Create("\"", 3427)
, Tuple.Create(Tuple.Create("", 3376), Tuple.Create("/Scripts/Common.ErrorHandler.js?", 3376), true)
            
            #line 57 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3408), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3408), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3474), Tuple.Create("\"", 3530)
, Tuple.Create(Tuple.Create("", 3480), Tuple.Create("/Scripts/Ext.ux.FitToParent.js?", 3480), true)
            
            #line 58 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3511), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3511), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3577), Tuple.Create("\"", 3641)
, Tuple.Create(Tuple.Create("", 3583), Tuple.Create("/Scripts/DoubleGis.MvcFormValidator.js?", 3583), true)
            
            #line 59 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3622), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3622), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3688), Tuple.Create("\"", 3753)
, Tuple.Create(Tuple.Create("", 3694), Tuple.Create("/Scripts/DoubleGis.DependencyHandler.js?", 3694), true)
            
            #line 60 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3734), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3734), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3800), Tuple.Create("\"", 3858)
, Tuple.Create(Tuple.Create("", 3806), Tuple.Create("/Scripts/Ext.Ajax.syncRequest.js?", 3806), true)
            
            #line 61 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3839), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3839), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3905), Tuple.Create("\"", 3961)
, Tuple.Create(Tuple.Create("", 3911), Tuple.Create("/Scripts/Ext.ux.LookupField.js?", 3911), true)
            
            #line 62 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3942), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 3942), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4008), Tuple.Create("\"", 4068)
, Tuple.Create(Tuple.Create("", 4014), Tuple.Create("/Scripts/Ext.ux.AsyncFileUpload.js?", 4014), true)
            
            #line 63 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4049), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4049), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4115), Tuple.Create("\"", 4169)
, Tuple.Create(Tuple.Create("", 4121), Tuple.Create("/Scripts/Ext.ux.LinkField.js?", 4121), true)
            
            #line 64 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4150), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4150), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4216), Tuple.Create("\"", 4275)
, Tuple.Create(Tuple.Create("", 4222), Tuple.Create("/Scripts/Ext.ux.PhonecallField.js?", 4222), true)
            
            #line 65 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4256), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4256), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4322), Tuple.Create("\"", 4375)
, Tuple.Create(Tuple.Create("", 4328), Tuple.Create("/Scripts/Ext.ux.Calendar.js?", 4328), true)
            
            #line 66 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4356), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4356), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4422), Tuple.Create("\"", 4483)
, Tuple.Create(Tuple.Create("", 4428), Tuple.Create("/Scripts/Ext.ux.LookupFieldOwner.js?", 4428), true)
            
            #line 67 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4464), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4464), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4530), Tuple.Create("\"", 4584)
, Tuple.Create(Tuple.Create("", 4536), Tuple.Create("/Scripts/Ext.ux.NotePanel.js?", 4536), true)
            
            #line 68 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4565), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4565), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4631), Tuple.Create("\"", 4693)
, Tuple.Create(Tuple.Create("", 4637), Tuple.Create("/Scripts/Ext.ux.ActionsHistoryTab.js?", 4637), true)
            
            #line 69 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4674), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4674), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4740), Tuple.Create("\"", 4795)
, Tuple.Create(Tuple.Create("", 4746), Tuple.Create("/Scripts/DoubleGis.UI.Card.js?", 4746), true)
            
            #line 70 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4776), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4776), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4842), Tuple.Create("\"", 4894)
, Tuple.Create(Tuple.Create("", 4848), Tuple.Create("/Scripts/Ext.ux.IdField.js?", 4848), true)
            
            #line 71 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4875), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4875), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4943), Tuple.Create("\"", 4999)
, Tuple.Create(Tuple.Create("", 4949), Tuple.Create("/Scripts/Ext.ux.MonthPicker.js?", 4949), true)
            
            #line 73 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4980), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4980), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 5046), Tuple.Create("\"", 5100)
, Tuple.Create(Tuple.Create("", 5052), Tuple.Create("/Scripts/Ext.ux.MonthMenu.js?", 5052), true)
            
            #line 74 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 5081), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 5081), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 5147), Tuple.Create("\"", 5204)
, Tuple.Create(Tuple.Create("", 5153), Tuple.Create("/Scripts/Ext.ux.TimeComboBox.js?", 5153), true)
            
            #line 75 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 5185), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 5185), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 5251), Tuple.Create("\"", 5305)
, Tuple.Create(Tuple.Create("", 5257), Tuple.Create("/Scripts/Ext.ux.Calendar2.js?", 5257), true)
            
            #line 76 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 5286), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 5286), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n");

WriteLiteral("    ");

            
            #line 78 "..\..\Views\Shared\_CardLayout.cshtml"
Write(RenderSection("CardScripts"));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n</head>\r\n    <body>\r\n");

            
            #line 82 "..\..\Views\Shared\_CardLayout.cshtml"
        
            
            #line default
            #line hidden
            
            #line 82 "..\..\Views\Shared\_CardLayout.cshtml"
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

            
            #line 90 "..\..\Views\Shared\_CardLayout.cshtml"
                   Write(RenderSection("CardBody"));

            
            #line default
            #line hidden
WriteLiteral("\r\n                        \r\n");

            
            #line 92 "..\..\Views\Shared\_CardLayout.cshtml"
                        
            
            #line default
            #line hidden
            
            #line 92 "..\..\Views\Shared\_CardLayout.cshtml"
                         if (Model is IEntityViewModelBase && Model.ViewConfig.CardSettings.HasAdminTab)
                        {
                            Html.RenderPartial("AdministrationTab");
                        }

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n\r\n");

WriteLiteral("                    ");

            
            #line 99 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.CheckBoxFor(x => x.ViewConfig.ReadOnly));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 100 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.ViewConfig.EntityName));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 101 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.ViewConfig.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 102 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.ViewConfig.PId));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 103 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.ViewConfig.PType));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 104 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.ViewConfig.ExtendedInfo));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 105 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.IsNew));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 106 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.Hidden("EntityStatus", ViewData["EntityStatus"]));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 107 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.Hidden("ViewConfig_DependencyList", Model.ViewConfig.DependencyList));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 108 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.EntityStateToken));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 109 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.IsDeleted));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 110 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.IsActive));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n                    <div");

WriteLiteral(" id=\"MessageType\"");

WriteLiteral(">");

            
            #line 112 "..\..\Views\Shared\_CardLayout.cshtml"
                                     Write(Model.MessageType);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                    <div");

WriteLiteral(" id=\"Message\"");

WriteLiteral(">");

            
            #line 113 "..\..\Views\Shared\_CardLayout.cshtml"
                                 Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                </div>\r\n            </div>            \r\n");

            
            #line 116 "..\..\Views\Shared\_CardLayout.cshtml"
        }   

            
            #line default
            #line hidden
WriteLiteral("        \r\n");

            
            #line 118 "..\..\Views\Shared\_CardLayout.cshtml"
        
            
            #line default
            #line hidden
            
            #line 118 "..\..\Views\Shared\_CardLayout.cshtml"
         if (IsSectionDefined("CustomInit"))
        {
            
            
            #line default
            #line hidden
            
            #line 120 "..\..\Views\Shared\_CardLayout.cshtml"
       Write(RenderSection("CustomInit"));

            
            #line default
            #line hidden
            
            #line 120 "..\..\Views\Shared\_CardLayout.cshtml"
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

            
            #line 128 "..\..\Views\Shared\_CardLayout.cshtml"
                                  Write(Html.WriteJson(Model.ViewConfig.CardSettings));

            
            #line default
            #line hidden
WriteLiteral(";\r\n                    window.Card = new window.Ext.DoubleGis.UI.SharableCard(car" +
"dSettings);\r\n                    window.Card.Build();\r\n                    \r\n   " +
"             });\r\n            </script>\r\n");

            
            #line 134 "..\..\Views\Shared\_CardLayout.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </body>\r\n</html>\r\n");

        }
    }
}
#pragma warning restore 1591
