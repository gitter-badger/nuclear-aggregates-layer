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
    
    #line 1 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Appointment.cshtml")]
    public partial class Appointment : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BL.UI.Web.Mvc.Models.Activity.AppointmentViewModel>
    {
        public Appointment()
        {
        }
        public override void Execute()
        {
            
            #line 4 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 196), Tuple.Create("\"", 253)
, Tuple.Create(Tuple.Create("", 202), Tuple.Create("/Scripts/Ext.ux.TimeComboBox.js?", 202), true)
            
            #line 10 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
, Tuple.Create(Tuple.Create("", 234), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 234), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 300), Tuple.Create("\"", 389)
, Tuple.Create(Tuple.Create("", 306), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.RegardingObjectController.js?", 306), true)
            
            #line 11 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
  , Tuple.Create(Tuple.Create("", 370), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 370), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 436), Tuple.Create("\"", 525)
, Tuple.Create(Tuple.Create("", 442), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.ContactRelationController.js?", 442), true)
            
            #line 12 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
  , Tuple.Create(Tuple.Create("", 506), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 506), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 572), Tuple.Create("\"", 648)
, Tuple.Create(Tuple.Create("", 578), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.ActivityBase.js?", 578), true)
            
            #line 13 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
, Tuple.Create(Tuple.Create("", 629), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 629), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 695), Tuple.Create("\"", 770)
, Tuple.Create(Tuple.Create("", 701), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.Appointment.js?", 701), true)
            
            #line 14 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
, Tuple.Create(Tuple.Create("", 751), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 751), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CustomInit", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.onReady(function() {\r\n            var cardSettings = ");

            
            #line 21 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
                          Write(Html.WriteJson(Model.ViewConfig.CardSettings));

            
            #line default
            #line hidden
WriteLiteral(";\r\n            Ext.apply(cardSettings, { contactField: \"AttendeeId\", contactCompo" +
"nent: \"Attendee\" });\r\n            window.Card = new window.Ext.DoubleGis.UI.Appo" +
"intment(cardSettings);\r\n            window.Card.Build();\r\n        });\r\n    </scr" +
"ipt>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 1290), Tuple.Create("\"", 1326)
            
            #line 31 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
, Tuple.Create(Tuple.Create("", 1298), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 1298), false)
);

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 32 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 33 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.HiddenFor(m => m.Status));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 34 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.HiddenFor(m => m.Priority));

            
            #line default
            #line hidden
WriteLiteral("\r\n        \r\n");

            
            #line 36 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
        
            
            #line default
            #line hidden
            
            #line 36 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
          
            var isOwnerAndNameSortFields = new[] { new LookupSortInfo { Direction = ListSortDirection.Descending, Field = "IsOwner" }, new LookupSortInfo { Direction = ListSortDirection.Ascending, Field = "Name" } };
            var isOwnerAndFullNameSortFields = new[] { new LookupSortInfo { Direction = ListSortDirection.Descending, Field = "IsOwner" }, new LookupSortInfo { Direction = ListSortDirection.Ascending, Field = "FullName" } };
        
            
            #line default
            #line hidden
WriteLiteral("\r\n        \r\n");

WriteLiteral("        ");

            
            #line 41 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.SectionHead("regardingObjectHeader", BLResources.TitleRegarding));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 42 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
        
            
            #line default
            #line hidden
            
            #line 42 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
          
            var firmDataFields = new[]
                                     {
                                         new LookupDataField { Name = "id", Mapping = "Id" }, 
                                         new LookupDataField { Name = "name", Mapping = "Name" }, 
                                         new LookupDataField { Name = "city", Mapping = "OrganizationUnitName" }
                                     };
            const string HeaderTextTemplate = "'<span class=\"x-lookup-thumb\">{name}</span>&nbsp;<span class=\"x-lookup-thumb\" style=\"color:gray\">{city}</span>&nbsp;'";
        
            
            #line default
            #line hidden
WriteLiteral(" \r\n");

WriteLiteral("        ");

            
            #line 51 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.SectionRow(
            @Html.TemplateField(m => m.Client, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Client, DefaultSortFields = isOwnerAndNameSortFields }),
            @Html.TemplateField(m => m.Firm, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Firm, ExtendedInfo = "ForClientAndLinkedChild=true;needHelp=false", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId", DefaultSortFields = isOwnerAndNameSortFields, DataFields = firmDataFields, HeaderTextTemplate = HeaderTextTemplate })));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 54 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Deal, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Deal, ExtendedInfo = "ForClientAndLinkedChild=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId", DefaultSortFields = isOwnerAndNameSortFields })));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 56 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.SectionHead("planHeader", BLResources.TitlePlan));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 57 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Purpose, FieldFlex.lone, null, EnumResources.ResourceManager)));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 58 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Title, FieldFlex.lone)));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 60 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.SectionRow(
            @Html.TemplateField(m => m.ScheduledStart, FieldFlex.twins, 
                new CalendarSettings { Store = CalendarSettings.StoreMode.Absolute, Time = new CalendarSettings.TimeSettings() }),
            @Html.TemplateField(m => m.ScheduledEnd, FieldFlex.twins, 
                new CalendarSettings { Store = CalendarSettings.StoreMode.Absolute, Time = new CalendarSettings.TimeSettings() })
            ));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 66 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Attendee, FieldFlex.lone, new LookupSettings { EntityName = EntityName.Contact, ExtendedInfo = "ForClientAndLinkedChild=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId", DefaultSortFields = isOwnerAndFullNameSortFields })));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 67 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Location, FieldFlex.lone)));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 69 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.SectionHead("resultHeader", BLResources.TitleResult));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 70 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Description, FieldFlex.lone, new Dictionary<string, object> { { "rows", "10" } })));

            
            #line default
            #line hidden
WriteLiteral("\r\n   </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
