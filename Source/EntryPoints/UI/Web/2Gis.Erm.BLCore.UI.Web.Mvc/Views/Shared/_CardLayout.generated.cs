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

WriteAttribute("content", Tuple.Create(" content=\"", 286), Tuple.Create("\"", 330)
            
            #line 9 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 296), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
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

WriteAttribute("href", Tuple.Create(" href=\"", 528), Tuple.Create("\"", 590)
, Tuple.Create(Tuple.Create("", 535), Tuple.Create("/Content/ext-all.css?", 535), true)
            
            #line 14 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 556), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 556), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 638), Tuple.Create("\"", 697)
, Tuple.Create(Tuple.Create("", 645), Tuple.Create("/Content/CRM4.css?", 645), true)
            
            #line 15 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 663), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 663), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 745), Tuple.Create("\"", 808)
, Tuple.Create(Tuple.Create("", 752), Tuple.Create("/Content/MainPage.css?", 752), true)
            
            #line 16 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 774), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 774), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 856), Tuple.Create("\"", 919)
, Tuple.Create(Tuple.Create("", 863), Tuple.Create("/Content/ext-mask.css?", 863), true)
            
            #line 17 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 885), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 885), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 967), Tuple.Create("\"", 1033)
, Tuple.Create(Tuple.Create("", 974), Tuple.Create("/Content/LookupStyle.css?", 974), true)
            
            #line 18 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 999), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 999), false)
);

WriteLiteral(" />\r\n    \r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 1087), Tuple.Create("\"", 1146)
, Tuple.Create(Tuple.Create("", 1094), Tuple.Create("/Content/Card.css?", 1094), true)
            
            #line 20 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1112), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1112), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 1194), Tuple.Create("\"", 1259)
, Tuple.Create(Tuple.Create("", 1201), Tuple.Create("/Content/ext-extend.css?", 1201), true)
            
            #line 21 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1225), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1225), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 1307), Tuple.Create("\"", 1377)
, Tuple.Create(Tuple.Create("", 1314), Tuple.Create("/Content/AsyncFileUpload.css?", 1314), true)
            
            #line 22 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1343), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1343), false)
);

WriteLiteral(" />\r\n\r\n");

            
            #line 24 "..\..\Views\Shared\_CardLayout.cshtml"
    
            
            #line default
            #line hidden
            
            #line 24 "..\..\Views\Shared\_CardLayout.cshtml"
     if (HttpContext.Current.IsDebuggingEnabled)
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1457), Tuple.Create("\"", 1524)
, Tuple.Create(Tuple.Create("", 1463), Tuple.Create("/Scripts/ext-base-debug.js?", 1463), true)
            
            #line 26 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1490), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1490), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1575), Tuple.Create("\"", 1652)
, Tuple.Create(Tuple.Create("", 1581), Tuple.Create("/Scripts/ext-all-debug-w-comments.js?", 1581), true)
            
            #line 27 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1618), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1618), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 28 "..\..\Views\Shared\_CardLayout.cshtml"
    }
    else
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1727), Tuple.Create("\"", 1788)
, Tuple.Create(Tuple.Create("", 1733), Tuple.Create("/Scripts/ext-base.js?", 1733), true)
            
            #line 31 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1754), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1754), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1839), Tuple.Create("\"", 1899)
, Tuple.Create(Tuple.Create("", 1845), Tuple.Create("/Scripts/ext-all.js?", 1845), true)
            
            #line 32 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1865), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1865), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 33 "..\..\Views\Shared\_CardLayout.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1959), Tuple.Create("\"", 2101)
, Tuple.Create(Tuple.Create("", 1965), Tuple.Create("/Scripts/", 1965), true)
            
            #line 35 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 1974), Tuple.Create<System.Object, System.Int32>("Ext.LocalizedResources." + ViewData.GetUserLocaleInfo().TwoLetterISOLanguageName + ".js"
            
            #line default
            #line hidden
, 1974), false)
, Tuple.Create(Tuple.Create("", 2066), Tuple.Create("?", 2066), true)
            
            #line 35 "..\..\Views\Shared\_CardLayout.cshtml"
                                       , Tuple.Create(Tuple.Create("", 2067), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2067), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.USER_ID = ");

            
            #line 38 "..\..\Views\Shared\_CardLayout.cshtml"
                 Write(ViewData.GetUserIdentityInfo().Code);

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.USER_NAME = ");

            
            #line 39 "..\..\Views\Shared\_CardLayout.cshtml"
                   Write(ViewData.GetUserIdentityDisplayName());

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo = ");

            
            #line 40 "..\..\Views\Shared\_CardLayout.cshtml"
                     Write(Html.WriteJson(ViewData.GetUserLocaleInfo()));

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo.NumberFormatInfo.CurrencySymbol = \'");

            
            #line 41 "..\..\Views\Shared\_CardLayout.cshtml"
                                                      Write(ViewData.GetErmBaseCurrencySymbol());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.CRM_URL = \'");

            
            #line 42 "..\..\Views\Shared\_CardLayout.cshtml"
                  Write(ViewData.GetMsCrmSettingsUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.BasicOperationsServiceRestUrl = \'");

            
            #line 43 "..\..\Views\Shared\_CardLayout.cshtml"
                                        Write(ViewData.GetBasicOperationsServiceRestUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.SpecialOperationsServiceRestUrl = \'");

            
            #line 44 "..\..\Views\Shared\_CardLayout.cshtml"
                                          Write(ViewData.GetSpecialOperationsServiceRestUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n    </script>\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2755), Tuple.Create("\"", 2833)
, Tuple.Create(Tuple.Create("", 2761), Tuple.Create("/Scripts/DoubleGis.GlobalVariables.js?", 2761), true)
            
            #line 47 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 2799), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2799), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2880), Tuple.Create("\"", 2952)
, Tuple.Create(Tuple.Create("", 2886), Tuple.Create("/Scripts/Common.ErrorHandler.js?", 2886), true)
            
            #line 48 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 2918), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2918), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2999), Tuple.Create("\"", 3070)
, Tuple.Create(Tuple.Create("", 3005), Tuple.Create("/Scripts/Ext.ux.FitToParent.js?", 3005), true)
            
            #line 49 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3036), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 3036), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3117), Tuple.Create("\"", 3196)
, Tuple.Create(Tuple.Create("", 3123), Tuple.Create("/Scripts/DoubleGis.MvcFormValidator.js?", 3123), true)
            
            #line 50 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3162), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 3162), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3243), Tuple.Create("\"", 3323)
, Tuple.Create(Tuple.Create("", 3249), Tuple.Create("/Scripts/DoubleGis.DependencyHandler.js?", 3249), true)
            
            #line 51 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3289), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 3289), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3370), Tuple.Create("\"", 3443)
, Tuple.Create(Tuple.Create("", 3376), Tuple.Create("/Scripts/Ext.Ajax.syncRequest.js?", 3376), true)
            
            #line 52 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3409), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 3409), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3490), Tuple.Create("\"", 3561)
, Tuple.Create(Tuple.Create("", 3496), Tuple.Create("/Scripts/Ext.ux.LookupField.js?", 3496), true)
            
            #line 53 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3527), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 3527), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3608), Tuple.Create("\"", 3683)
, Tuple.Create(Tuple.Create("", 3614), Tuple.Create("/Scripts/Ext.ux.AsyncFileUpload.js?", 3614), true)
            
            #line 54 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3649), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 3649), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3730), Tuple.Create("\"", 3799)
, Tuple.Create(Tuple.Create("", 3736), Tuple.Create("/Scripts/Ext.ux.LinkField.js?", 3736), true)
            
            #line 55 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3765), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 3765), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3846), Tuple.Create("\"", 3914)
, Tuple.Create(Tuple.Create("", 3852), Tuple.Create("/Scripts/Ext.ux.Calendar.js?", 3852), true)
            
            #line 56 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 3880), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 3880), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 3961), Tuple.Create("\"", 4037)
, Tuple.Create(Tuple.Create("", 3967), Tuple.Create("/Scripts/Ext.ux.LookupFieldOwner.js?", 3967), true)
            
            #line 57 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4003), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 4003), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4084), Tuple.Create("\"", 4153)
, Tuple.Create(Tuple.Create("", 4090), Tuple.Create("/Scripts/Ext.ux.NotePanel.js?", 4090), true)
            
            #line 58 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4119), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 4119), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4200), Tuple.Create("\"", 4277)
, Tuple.Create(Tuple.Create("", 4206), Tuple.Create("/Scripts/Ext.ux.ActionsHistoryTab.js?", 4206), true)
            
            #line 59 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4243), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 4243), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4324), Tuple.Create("\"", 4394)
, Tuple.Create(Tuple.Create("", 4330), Tuple.Create("/Scripts/DoubleGis.UI.Card.js?", 4330), true)
            
            #line 60 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4360), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 4360), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4441), Tuple.Create("\"", 4508)
, Tuple.Create(Tuple.Create("", 4447), Tuple.Create("/Scripts/Ext.ux.IdField.js?", 4447), true)
            
            #line 61 "..\..\Views\Shared\_CardLayout.cshtml"
, Tuple.Create(Tuple.Create("", 4474), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 4474), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n");

WriteLiteral("    ");

            
            #line 63 "..\..\Views\Shared\_CardLayout.cshtml"
Write(RenderSection("CardScripts"));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n</head>\r\n    <body>\r\n");

            
            #line 67 "..\..\Views\Shared\_CardLayout.cshtml"
        
            
            #line default
            #line hidden
            
            #line 67 "..\..\Views\Shared\_CardLayout.cshtml"
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

            
            #line 75 "..\..\Views\Shared\_CardLayout.cshtml"
                   Write(RenderSection("CardBody"));

            
            #line default
            #line hidden
WriteLiteral("\r\n                        \r\n");

            
            #line 77 "..\..\Views\Shared\_CardLayout.cshtml"
                        
            
            #line default
            #line hidden
            
            #line 77 "..\..\Views\Shared\_CardLayout.cshtml"
                         if (Model is IEntityViewModelBase && Model.ViewConfig.CardSettings.HasAdminTab)
                        {
                            Html.RenderPartial("AdministrationTab");
                        }

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n\r\n");

WriteLiteral("                    ");

            
            #line 84 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.CheckBoxFor(x => x.ViewConfig.ReadOnly));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 85 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.ViewConfig.EntityName));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 86 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.ViewConfig.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 87 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.ViewConfig.PId));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 88 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.ViewConfig.PType));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 89 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.ViewConfig.ExtendedInfo));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 90 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.IsNew));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 91 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.Hidden("EntityStatus", ViewData["EntityStatus"]));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 92 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.Hidden("ViewConfig_DependencyList", Model.ViewConfig.DependencyList));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 93 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.EntityStateToken));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 94 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.IsDeleted));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 95 "..\..\Views\Shared\_CardLayout.cshtml"
               Write(Html.HiddenFor(m => m.IsActive));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n                    <div");

WriteLiteral(" id=\"MessageType\"");

WriteLiteral(">");

            
            #line 97 "..\..\Views\Shared\_CardLayout.cshtml"
                                     Write(Model.MessageType);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                    <div");

WriteLiteral(" id=\"Message\"");

WriteLiteral(">");

            
            #line 98 "..\..\Views\Shared\_CardLayout.cshtml"
                                 Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                </div>\r\n            </div>            \r\n");

            
            #line 101 "..\..\Views\Shared\_CardLayout.cshtml"
        }   

            
            #line default
            #line hidden
WriteLiteral("        \r\n");

            
            #line 103 "..\..\Views\Shared\_CardLayout.cshtml"
        
            
            #line default
            #line hidden
            
            #line 103 "..\..\Views\Shared\_CardLayout.cshtml"
         if (IsSectionDefined("CustomInit"))
        {
            
            
            #line default
            #line hidden
            
            #line 105 "..\..\Views\Shared\_CardLayout.cshtml"
       Write(RenderSection("CustomInit"));

            
            #line default
            #line hidden
            
            #line 105 "..\..\Views\Shared\_CardLayout.cshtml"
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

            
            #line 113 "..\..\Views\Shared\_CardLayout.cshtml"
                                  Write(Html.WriteJson(Model.ViewConfig.CardSettings));

            
            #line default
            #line hidden
WriteLiteral(";\r\n                    window.Card = new window.Ext.DoubleGis.UI.SharableCard(car" +
"dSettings);\r\n                    window.Card.Build();\r\n                    \r\n   " +
"             });\r\n            </script>\r\n");

            
            #line 119 "..\..\Views\Shared\_CardLayout.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </body>\r\n</html>\r\n");

        }
    }
}
#pragma warning restore 1591
