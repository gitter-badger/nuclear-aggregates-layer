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

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Views.CreateOrUpdate
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
    using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Phonecall.cshtml")]
    public partial class Phonecall : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BL.UI.Web.Mvc.Models.Activity.PhonecallViewModel>
    {
        public Phonecall()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 164), Tuple.Create("\"", 236)
, Tuple.Create(Tuple.Create("", 170), Tuple.Create("/Scripts/Ext.ux.TimeComboBox.js?", 170), true)
            
            #line 9 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
, Tuple.Create(Tuple.Create("", 202), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 202), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 283), Tuple.Create("\"", 387)
, Tuple.Create(Tuple.Create("", 289), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.RegardingObjectController.js?", 289), true)
            
            #line 10 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
  , Tuple.Create(Tuple.Create("", 353), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 353), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 434), Tuple.Create("\"", 525)
, Tuple.Create(Tuple.Create("", 440), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.ActivityBase.js?", 440), true)
            
            #line 11 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
, Tuple.Create(Tuple.Create("", 491), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 491), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 572), Tuple.Create("\"", 660)
, Tuple.Create(Tuple.Create("", 578), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.Phonecall.js?", 578), true)
            
            #line 12 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
, Tuple.Create(Tuple.Create("", 626), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 626), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CustomInit", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.onReady(function()\r\n        {\r\n            var cardSettings = ");

            
            #line 20 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
                          Write(Html.WriteJson(Model.ViewConfig.CardSettings));

            
            #line default
            #line hidden
WriteLiteral(";\r\n            window.Card = new window.Ext.DoubleGis.UI.Phonecall(cardSettings);" +
"\r\n            window.Card.Build();\r\n        });\r\n    </script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 1087), Tuple.Create("\"", 1123)
            
            #line 29 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
, Tuple.Create(Tuple.Create("", 1095), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 1095), false)
);

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 30 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 31 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.HiddenFor(m => m.Status));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 33 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.SectionHead("regardingObjectHeader", BLResources.TitleRegarding));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 34 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Client, FieldFlex.lone, new LookupSettings { EntityName = EntityName.Client })));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 35 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Firm, FieldFlex.lone, new LookupSettings { EntityName = EntityName.Firm, ExtendedInfo = "filterToParent=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId", SupressMatchesErrors = true })));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 36 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Deal, FieldFlex.lone, new LookupSettings { EntityName = EntityName.Deal, ExtendedInfo = "filterToParent=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId", SupressMatchesErrors = true })));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 38 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.SectionHead("planHeader", BLResources.TitlePlan));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 39 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.SectionRow(
            @Html.TemplateField(m => m.Purpose, FieldFlex.twins, null, EnumResources.ResourceManager),
            @Html.TemplateField(m => m.Priority, FieldFlex.twins, null, EnumResources.ResourceManager)));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 42 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Title, FieldFlex.lone)));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 43 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Contact, FieldFlex.lone, new LookupSettings { EntityName = EntityName.Contact, ExtendedInfo = "filterToParent=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId" })));

            
            #line default
            #line hidden
WriteLiteral("\r\n        \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"display-wrapper field-wrapper twins\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"label-wrapper\"");

WriteLiteral(">\r\n                    <span>\r\n");

WriteLiteral("                        ");

            
            #line 49 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
                   Write(Html.LabelFor(m => m.ScheduledStart));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </span>\r\n                </div>\r\n                <div");

WriteLiteral(" class=\"input-wrapper\"");

WriteLiteral(">\r\n                    <table");

WriteLiteral(" cellpadding=\"0\"");

WriteLiteral(" cellspacing=\"0\"");

WriteLiteral(">\r\n                        <tbody>\r\n                            <tr>\r\n           " +
"                     <td");

WriteLiteral(" style=\"width: 120px\"");

WriteLiteral(">\r\n");

WriteLiteral("                                    ");

            
            #line 57 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
                               Write(Html.DateFor(m => m.ScheduledStart, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                    ");

            
            #line 58 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
                               Write(Html.ValidationMessageFor(m => m.ScheduledStart, null, new Dictionary<string, object> { { "class", "error" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                                <td");

WriteLiteral(" style=\"width: 5px\"");

WriteLiteral("></td>\r\n                                <td");

WriteLiteral(" style=\"width: 60px\"");

WriteLiteral(">\r\n");

WriteLiteral("                                    ");

            
            #line 62 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
                               Write(Html.TextBoxFor(m => m.ScheduledStartTime));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                                <td");

WriteLiteral(" style=\"width: auto\"");

WriteLiteral("></td>\r\n                            </tr>\r\n                        </tbody>\r\n    " +
"                </table>\r\n                </div>\r\n            </div>\r\n        </" +
"div>\r\n");

WriteLiteral("        ");

            
            #line 71 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.SectionHead("resultHeader", BLResources.TitleResult));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 72 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Description, FieldFlex.lone, new Dictionary<string, object> { { "rows", "5" } })));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
