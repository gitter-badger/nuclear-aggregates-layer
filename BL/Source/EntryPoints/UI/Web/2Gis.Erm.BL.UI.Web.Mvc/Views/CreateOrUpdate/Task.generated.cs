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
    
    #line 1 "..\..\Views\CreateOrUpdate\Task.cshtml"
    using System.ComponentModel;
    
    #line default
    #line hidden
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
            
            #line 4 "..\..\Views\CreateOrUpdate\Task.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 189), Tuple.Create("\"", 246)
, Tuple.Create(Tuple.Create("", 195), Tuple.Create("/Scripts/Ext.ux.TimeComboBox.js?", 195), true)
            
            #line 10 "..\..\Views\CreateOrUpdate\Task.cshtml"
, Tuple.Create(Tuple.Create("", 227), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 227), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 293), Tuple.Create("\"", 382)
, Tuple.Create(Tuple.Create("", 299), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.RegardingObjectController.js?", 299), true)
            
            #line 11 "..\..\Views\CreateOrUpdate\Task.cshtml"
  , Tuple.Create(Tuple.Create("", 363), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 363), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 429), Tuple.Create("\"", 505)
, Tuple.Create(Tuple.Create("", 435), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.ActivityBase.js?", 435), true)
            
            #line 12 "..\..\Views\CreateOrUpdate\Task.cshtml"
, Tuple.Create(Tuple.Create("", 486), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 486), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 552), Tuple.Create("\"", 620)
, Tuple.Create(Tuple.Create("", 558), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.Task.js?", 558), true)
            
            #line 13 "..\..\Views\CreateOrUpdate\Task.cshtml"
, Tuple.Create(Tuple.Create("", 601), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 601), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CustomInit", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.onReady(function()\r\n        {\r\n            var cardSettings = ");

            
            #line 21 "..\..\Views\CreateOrUpdate\Task.cshtml"
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

WriteAttribute("title", Tuple.Create(" title=\"", 1042), Tuple.Create("\"", 1078)
            
            #line 30 "..\..\Views\CreateOrUpdate\Task.cshtml"
, Tuple.Create(Tuple.Create("", 1050), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 1050), false)
);

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 31 "..\..\Views\CreateOrUpdate\Task.cshtml"
   Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 32 "..\..\Views\CreateOrUpdate\Task.cshtml"
   Write(Html.HiddenFor(m => m.Status));

            
            #line default
            #line hidden
WriteLiteral("\r\n        \r\n");

            
            #line 34 "..\..\Views\CreateOrUpdate\Task.cshtml"
        
            
            #line default
            #line hidden
            
            #line 34 "..\..\Views\CreateOrUpdate\Task.cshtml"
          
            var isOwnerAndNameSortFields = new[] { new LookupSortInfo { Direction = ListSortDirection.Descending, Field = "IsOwner" }, new LookupSortInfo { Direction = ListSortDirection.Ascending, Field = "Name" } };            
        
            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 38 "..\..\Views\CreateOrUpdate\Task.cshtml"
   Write(Html.SectionHead("regardingObjectHeader", BLResources.TitleRegarding));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 39 "..\..\Views\CreateOrUpdate\Task.cshtml"
        
            
            #line default
            #line hidden
            
            #line 39 "..\..\Views\CreateOrUpdate\Task.cshtml"
          
            var firmDataFields = new[]
                                     {
                                         new LookupDataField { Name = "id", Mapping = "Id" }, 
                                         new LookupDataField { Name = "name", Mapping = "Name" }, 
                                         new LookupDataField { Name = "city", Mapping = "OrganizationUnitName" }
                                     };
            const string HeaderTextTemplate = "'<span class=\"x-lookup-thumb\">{name}</span>&nbsp;<span class=\"x-lookup-thumb\" style=\"color:gray\">{city}</span>&nbsp;'";
        
            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 48 "..\..\Views\CreateOrUpdate\Task.cshtml"
   Write(Html.SectionRow(
            @Html.TemplateField(m => m.Client, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Client, DefaultSortFields = isOwnerAndNameSortFields }),
            @Html.TemplateField(m => m.Firm, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Firm, ExtendedInfo = "ForClientAndLinkedChild=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId", DefaultSortFields = isOwnerAndNameSortFields, DataFields = firmDataFields, HeaderTextTemplate = HeaderTextTemplate })));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 51 "..\..\Views\CreateOrUpdate\Task.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Deal, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Deal, ExtendedInfo = "ForClientAndLinkedChild=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId", DefaultSortFields = isOwnerAndNameSortFields })));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 53 "..\..\Views\CreateOrUpdate\Task.cshtml"
   Write(Html.SectionHead("planHeader", BLResources.TitlePlan));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 54 "..\..\Views\CreateOrUpdate\Task.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.TaskType, FieldFlex.lone, null, EnumResources.ResourceManager)));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 55 "..\..\Views\CreateOrUpdate\Task.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Title, FieldFlex.lone)));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 56 "..\..\Views\CreateOrUpdate\Task.cshtml"
   Write(Html.SectionRow(
            @Html.TemplateField(m => m.ScheduledStart, FieldFlex.twins,
                new CalendarSettings { Store = CalendarSettings.StoreMode.Absolute, Time = new CalendarSettings.TimeSettings() }),
            @Html.TemplateField(m => m.Priority, FieldFlex.twins, null, EnumResources.ResourceManager)));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 61 "..\..\Views\CreateOrUpdate\Task.cshtml"
   Write(Html.SectionHead("resultHeader", BLResources.TitleResult));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 62 "..\..\Views\CreateOrUpdate\Task.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Description, FieldFlex.lone, new Dictionary<string, object> { { "rows", "10" } })));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
