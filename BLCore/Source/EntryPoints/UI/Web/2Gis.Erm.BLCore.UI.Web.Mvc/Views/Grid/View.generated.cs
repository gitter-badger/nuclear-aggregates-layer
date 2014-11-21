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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Grid/View.cshtml")]
    public partial class View : System.Web.Mvc.WebViewPage<Settings.ConfigurationDto.EntityViewSet>
    {
        public View()
        {
        }
        public override void Execute()
        {
WriteLiteral("<html");

WriteLiteral(" xmlns=\"http://www.w3.org/1999/xhtml\"");

WriteLiteral(">\r\n<head>\r\n    <title>");

            
            #line 5 "..\..\Views\Grid\View.cshtml"
      Write(BLResources.List);

            
            #line default
            #line hidden
WriteLiteral("</title>\r\n\r\n    <meta");

WriteLiteral(" id=\"meta_IsDebug\"");

WriteLiteral(" name=\"meta_IsDebug\"");

WriteAttribute("content", Tuple.Create(" content=\"", 201), Tuple.Create("\"", 250)
            
            #line 7 "..\..\Views\Grid\View.cshtml"
, Tuple.Create(Tuple.Create("", 211), Tuple.Create<System.Object, System.Int32>(HttpContext.Current.IsDebuggingEnabled
            
            #line default
            #line hidden
, 211), false)
);

WriteLiteral(" />\r\n    <meta");

WriteLiteral(" id=\"meta_Revision\"");

WriteLiteral(" name=\"meta_Revision\"");

WriteAttribute("content", Tuple.Create(" content=\"", 305), Tuple.Create("\"", 334)
            
            #line 8 "..\..\Views\Grid\View.cshtml"
, Tuple.Create(Tuple.Create("", 315), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 315), false)
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

WriteAttribute("href", Tuple.Create(" href=\"", 532), Tuple.Create("\"", 579)
, Tuple.Create(Tuple.Create("", 539), Tuple.Create("/Content/ext-all.css?", 539), true)
            
            #line 13 "..\..\Views\Grid\View.cshtml"
, Tuple.Create(Tuple.Create("", 560), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 560), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 627), Tuple.Create("\"", 677)
, Tuple.Create(Tuple.Create("", 634), Tuple.Create("/Content/ext-extend.css?", 634), true)
            
            #line 14 "..\..\Views\Grid\View.cshtml"
, Tuple.Create(Tuple.Create("", 658), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 658), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 725), Tuple.Create("\"", 773)
, Tuple.Create(Tuple.Create("", 732), Tuple.Create("/Content/DataList.css?", 732), true)
            
            #line 15 "..\..\Views\Grid\View.cshtml"
, Tuple.Create(Tuple.Create("", 754), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 754), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 821), Tuple.Create("\"", 875)
, Tuple.Create(Tuple.Create("", 828), Tuple.Create("/Content/QuickFindStyle.css?", 828), true)
            
            #line 16 "..\..\Views\Grid\View.cshtml"
, Tuple.Create(Tuple.Create("", 856), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 856), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 923), Tuple.Create("\"", 967)
, Tuple.Create(Tuple.Create("", 930), Tuple.Create("/Content/CRM4.css?", 930), true)
            
            #line 17 "..\..\Views\Grid\View.cshtml"
, Tuple.Create(Tuple.Create("", 948), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 948), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 1015), Tuple.Create("\"", 1063)
, Tuple.Create(Tuple.Create("", 1022), Tuple.Create("/Content/ext-mask.css?", 1022), true)
            
            #line 18 "..\..\Views\Grid\View.cshtml"
, Tuple.Create(Tuple.Create("", 1044), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1044), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 1111), Tuple.Create("\"", 1162)
, Tuple.Create(Tuple.Create("", 1118), Tuple.Create("/Content/LookupStyle.css?", 1118), true)
            
            #line 19 "..\..\Views\Grid\View.cshtml"
, Tuple.Create(Tuple.Create("", 1143), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1143), false)
);

WriteLiteral(" />\r\n    \r\n");

            
            #line 21 "..\..\Views\Grid\View.cshtml"
    
            
            #line default
            #line hidden
            
            #line 21 "..\..\Views\Grid\View.cshtml"
     if (HttpContext.Current.IsDebuggingEnabled)
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1246), Tuple.Create("\"", 1298)
, Tuple.Create(Tuple.Create("", 1252), Tuple.Create("/Scripts/ext-base-debug.js?", 1252), true)
            
            #line 23 "..\..\Views\Grid\View.cshtml"
, Tuple.Create(Tuple.Create("", 1279), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1279), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1349), Tuple.Create("\"", 1411)
, Tuple.Create(Tuple.Create("", 1355), Tuple.Create("/Scripts/ext-all-debug-w-comments.js?", 1355), true)
            
            #line 24 "..\..\Views\Grid\View.cshtml"
, Tuple.Create(Tuple.Create("", 1392), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1392), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 25 "..\..\Views\Grid\View.cshtml"
    }
    else
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1486), Tuple.Create("\"", 1532)
, Tuple.Create(Tuple.Create("", 1492), Tuple.Create("/Scripts/ext-base.js?", 1492), true)
            
            #line 28 "..\..\Views\Grid\View.cshtml"
, Tuple.Create(Tuple.Create("", 1513), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1513), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1583), Tuple.Create("\"", 1628)
, Tuple.Create(Tuple.Create("", 1589), Tuple.Create("/Scripts/ext-all.js?", 1589), true)
            
            #line 29 "..\..\Views\Grid\View.cshtml"
, Tuple.Create(Tuple.Create("", 1609), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1609), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 30 "..\..\Views\Grid\View.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1684), Tuple.Create("\"", 1811)
, Tuple.Create(Tuple.Create("", 1690), Tuple.Create("/Scripts/", 1690), true)
            
            #line 32 "..\..\Views\Grid\View.cshtml"
, Tuple.Create(Tuple.Create("", 1699), Tuple.Create<System.Object, System.Int32>("Ext.LocalizedResources." + ViewData.GetUserLocaleInfo().TwoLetterISOLanguageName + ".js"
            
            #line default
            #line hidden
, 1699), false)
, Tuple.Create(Tuple.Create("", 1791), Tuple.Create("?", 1791), true)
            
            #line 32 "..\..\Views\Grid\View.cshtml"
                                       , Tuple.Create(Tuple.Create("", 1792), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1792), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.USER_ID = ");

            
            #line 35 "..\..\Views\Grid\View.cshtml"
                 Write(ViewData.GetUserIdentityInfo().Code);

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.USER_NAME = ");

            
            #line 36 "..\..\Views\Grid\View.cshtml"
                   Write(ViewData.GetUserIdentityDisplayName());

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo = ");

            
            #line 37 "..\..\Views\Grid\View.cshtml"
                     Write(Html.WriteJson(ViewData.GetUserLocaleInfo()));

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo.NumberFormatInfo.CurrencySymbol = \'");

            
            #line 38 "..\..\Views\Grid\View.cshtml"
                                                      Write(ViewData.GetErmBaseCurrencySymbol());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.CRM_URL = \'");

            
            #line 39 "..\..\Views\Grid\View.cshtml"
                  Write(ViewData.GetMsCrmSettingsUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.BasicOperationsServiceRestUrl = \'");

            
            #line 40 "..\..\Views\Grid\View.cshtml"
                                        Write(ViewData.GetBasicOperationsServiceRestUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.SpecialOperationsServiceRestUrl = \'");

            
            #line 41 "..\..\Views\Grid\View.cshtml"
                                          Write(ViewData.GetSpecialOperationsServiceRestUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n    </script>\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2465), Tuple.Create("\"", 2528)
, Tuple.Create(Tuple.Create("", 2471), Tuple.Create("/Scripts/DoubleGis.GlobalVariables.js?", 2471), true)
            
            #line 44 "..\..\Views\Grid\View.cshtml"
, Tuple.Create(Tuple.Create("", 2509), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 2509), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2575), Tuple.Create("\"", 2633)
, Tuple.Create(Tuple.Create("", 2581), Tuple.Create("/Scripts/Ext.Ajax.syncRequest.js?", 2581), true)
            
            #line 45 "..\..\Views\Grid\View.cshtml"
, Tuple.Create(Tuple.Create("", 2614), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 2614), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2680), Tuple.Create("\"", 2737)
, Tuple.Create(Tuple.Create("", 2686), Tuple.Create("/Scripts/Common.ErrorHandler.js?", 2686), true)
            
            #line 46 "..\..\Views\Grid\View.cshtml"
, Tuple.Create(Tuple.Create("", 2718), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 2718), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2784), Tuple.Create("\"", 2843)
, Tuple.Create(Tuple.Create("", 2790), Tuple.Create("/Scripts/DoubleGis.UI.DataList.js?", 2790), true)
            
            #line 47 "..\..\Views\Grid\View.cshtml"
, Tuple.Create(Tuple.Create("", 2824), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 2824), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">
        var SearchRecords = function ()
        {
            Entity.Items.Store.setBaseParam(""filterInput"", this.getValue());
            Entity.Items.Grid.getBottomToolbar().changePage(1);
        };
    
        Ext.onReady(function ()
            {
            var settings = ");

            
            #line 58 "..\..\Views\Grid\View.cshtml"
                      Write(Html.WriteJson(Model));

            
            #line default
            #line hidden
WriteLiteral(@";
            settings.listeners = {
                afterbuild:function () 
                {
                    var viewBox = Ext.get(""availableViews"");
                    var defaultView = this.currentSettings;
                    Ext.each(this.EntityModel.DataViews, function (item) {
                        var isDefault = defaultView && item.NameLocaleResourceId == defaultView.NameLocaleResourceId;
                        viewBox.dom.options.add(new Option(item.LocalizedName, item.NameLocaleResourceId, isDefault, isDefault));
                    });
                    viewBox.on(""change"", this.RebuildPage, this);
                },
                afterrebuild: function () {
                    Ext.DoubleGis.UI.QuikFind[""RecordFilter""].setValue("""");
                    if(this.currentSettings.Title) {
                        window.document.title = this.currentSettings.Title;
                    }
                }
            };
            window.Entity = new Ext.DoubleGis.UI.DataList(settings);
        });
    </script>
");

            
            #line 80 "..\..\Views\Grid\View.cshtml"
    
            
            #line default
            #line hidden
            
            #line 80 "..\..\Views\Grid\View.cshtml"
     foreach (var script in Model.DataViews.First().Scripts)
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 4381), Tuple.Create("\"", 4432)
, Tuple.Create(Tuple.Create("", 4387), Tuple.Create("/Scripts/", 4387), true)
            
            #line 82 "..\..\Views\Grid\View.cshtml"
, Tuple.Create(Tuple.Create("", 4396), Tuple.Create<System.Object, System.Int32>(script.FileName
            
            #line default
            #line hidden
, 4396), false)
, Tuple.Create(Tuple.Create("", 4412), Tuple.Create("?", 4412), true)
            
            #line 82 "..\..\Views\Grid\View.cshtml"
, Tuple.Create(Tuple.Create("", 4413), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 4413), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 83 "..\..\Views\Grid\View.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("</head>\r\n<body");

WriteLiteral(" class=\"stage\"");

WriteLiteral(">\r\n    <div");

WriteLiteral(" id=\"DataListContent\"");

WriteLiteral(" style=\"display: none\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" id=\"FilterPanel\"");

WriteLiteral(">\r\n            <table");

WriteLiteral(" width=\"100%\"");

WriteLiteral(" border=\"0\"");

WriteLiteral(" style=\"background-color: #d6e8ff\"");

WriteLiteral(">\r\n                <tr>\r\n                    <td>\r\n                        <table" +
"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(" border=\"0\"");

WriteLiteral(" cellpadding=\"0\"");

WriteLiteral(" style=\"padding-left: 5px; padding-top: 7px;\r\n                            padding" +
"-bottom: 5px;\"");

WriteLiteral(">\r\n                            <colgroup>\r\n                                <col");

WriteLiteral(" width=\"60%\"");

WriteLiteral(" />\r\n                                <col");

WriteLiteral(" width=\"20\"");

WriteLiteral(" />\r\n                                <col />\r\n                                <co" +
"l");

WriteLiteral(" width=\"40%\"");

WriteLiteral(" />\r\n                            </colgroup>\r\n                            <tr>\r\n " +
"                               <td>\r\n");

            
            #line 101 "..\..\Views\Grid\View.cshtml"
                                    
            
            #line default
            #line hidden
            
            #line 101 "..\..\Views\Grid\View.cshtml"
                                      
                                        Html.RenderPartial("QuickFindControl", new QuickFindSettings
                                        {
                                            Name = "RecordFilter",
                                            OnSearchFunction = "SearchRecords"
                                        });
                                     
            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                                <td");

WriteLiteral(" style=\"width: 20\"");

WriteLiteral(">\r\n                                    <span />\r\n                                " +
"</td>\r\n                                <td");

WriteLiteral(" style=\"font-weight: bold;\"");

WriteLiteral(">\r\n");

WriteLiteral("                                    ");

            
            #line 113 "..\..\Views\Grid\View.cshtml"
                               Write(BLResources.View);

            
            #line default
            #line hidden
WriteLiteral(":\r\n                                </td>\r\n                                <td");

WriteLiteral(" class=\"QuickFind\"");

WriteLiteral(">\r\n                                    <select");

WriteLiteral(" id=\"availableViews\"");

WriteLiteral(" class=\"inputfields\"");

WriteLiteral(" style=\"width: 100%\"");

WriteLiteral(">\r\n                                        <optgroup");

WriteLiteral(" id=\"AppSystemViews\"");

WriteAttribute("label", Tuple.Create(" label=\"", 6228), Tuple.Create("\"", 6260)
            
            #line 117 "..\..\Views\Grid\View.cshtml"
, Tuple.Create(Tuple.Create("", 6236), Tuple.Create<System.Object, System.Int32>(BLResources.SystemViews
            
            #line default
            #line hidden
, 6236), false)
);

WriteLiteral(@" />
                                    </select>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</body>
</html>
");

        }
    }
}
#pragma warning restore 1591
