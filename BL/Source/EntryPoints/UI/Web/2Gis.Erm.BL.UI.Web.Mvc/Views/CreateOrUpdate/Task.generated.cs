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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Task.cshtml")]
    public partial class Task : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BL.UI.Web.Mvc.Models.Activity.TaskViewModel>
    {
        public Task()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\CreateOrUpdate\Task.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 159), Tuple.Create("\"", 216)
, Tuple.Create(Tuple.Create("", 165), Tuple.Create("/Scripts/Ext.ux.TimeComboBox.js?", 165), true)
            
            #line 9 "..\..\Views\CreateOrUpdate\Task.cshtml"
, Tuple.Create(Tuple.Create("", 197), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 197), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 263), Tuple.Create("\"", 352)
, Tuple.Create(Tuple.Create("", 269), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.RegardingObjectController.js?", 269), true)
            
            #line 10 "..\..\Views\CreateOrUpdate\Task.cshtml"
  , Tuple.Create(Tuple.Create("", 333), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 333), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 399), Tuple.Create("\"", 475)
, Tuple.Create(Tuple.Create("", 405), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.ActivityBase.js?", 405), true)
            
            #line 11 "..\..\Views\CreateOrUpdate\Task.cshtml"
, Tuple.Create(Tuple.Create("", 456), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 456), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 522), Tuple.Create("\"", 590)
, Tuple.Create(Tuple.Create("", 528), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.Task.js?", 528), true)
            
            #line 12 "..\..\Views\CreateOrUpdate\Task.cshtml"
, Tuple.Create(Tuple.Create("", 571), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 571), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CustomInit", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.onReady(function()\r\n        {\r\n            var cardSettings = ");

            
            #line 20 "..\..\Views\CreateOrUpdate\Task.cshtml"
                          Write(Html.WriteJson(Model.ViewConfig.CardSettings));

            
            #line default
            #line hidden
WriteLiteral(";\r\n            window.Card = new window.Ext.DoubleGis.UI.Task(cardSettings);\r\n   " +
"         window.Card.Build();\r\n        });\r\n    </script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 1012), Tuple.Create("\"", 1048)
            
            #line 29 "..\..\Views\CreateOrUpdate\Task.cshtml"
, Tuple.Create(Tuple.Create("", 1020), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 1020), false)
);

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 30 "..\..\Views\CreateOrUpdate\Task.cshtml"
   Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 31 "..\..\Views\CreateOrUpdate\Task.cshtml"
   Write(Html.HiddenFor(m => m.Status));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 33 "..\..\Views\CreateOrUpdate\Task.cshtml"
   Write(Html.SectionHead("regardingObjectHeader", BLResources.TitleRegarding));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 34 "..\..\Views\CreateOrUpdate\Task.cshtml"
   Write(Html.SectionRow(
            @Html.TemplateField(m => m.Client, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Client }),
            @Html.TemplateField(m => m.Firm, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Firm, ExtendedInfo = "filterToParent=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId" })));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 37 "..\..\Views\CreateOrUpdate\Task.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Deal, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Deal, ExtendedInfo = "filterToParent=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId" })));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 39 "..\..\Views\CreateOrUpdate\Task.cshtml"
   Write(Html.SectionHead("planHeader", BLResources.TitlePlan));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 40 "..\..\Views\CreateOrUpdate\Task.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.TaskType, FieldFlex.lone, null, EnumResources.ResourceManager)));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 41 "..\..\Views\CreateOrUpdate\Task.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Title, FieldFlex.lone)));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 42 "..\..\Views\CreateOrUpdate\Task.cshtml"
   Write(Html.SectionRow(
            @Html.TemplateField(m => m.ScheduledStart, FieldFlex.twins,
                new CalendarSettings { Store = CalendarSettings.StoreMode.Absolute, Time = new CalendarSettings.TimeSettings() }),
            @Html.TemplateField(m => m.Priority, FieldFlex.twins, null, EnumResources.ResourceManager)));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 47 "..\..\Views\CreateOrUpdate\Task.cshtml"
   Write(Html.SectionHead("resultHeader", BLResources.TitleResult));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 48 "..\..\Views\CreateOrUpdate\Task.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Description, FieldFlex.lone, new Dictionary<string, object> { { "rows", "10" } })));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
