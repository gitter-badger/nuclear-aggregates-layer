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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Appointment.cshtml")]
    public partial class Appointment : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BL.UI.Web.Mvc.Models.Activity.AppointmentViewModel>
    {
        public Appointment()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 166), Tuple.Create("\"", 223)
, Tuple.Create(Tuple.Create("", 172), Tuple.Create("/Scripts/Ext.ux.TimeComboBox.js?", 172), true)
            
            #line 9 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
, Tuple.Create(Tuple.Create("", 204), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 204), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 270), Tuple.Create("\"", 359)
, Tuple.Create(Tuple.Create("", 276), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.RegardingObjectController.js?", 276), true)
            
            #line 10 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
  , Tuple.Create(Tuple.Create("", 340), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 340), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 406), Tuple.Create("\"", 495)
, Tuple.Create(Tuple.Create("", 412), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.ContactRelationController.js?", 412), true)
            
            #line 11 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
  , Tuple.Create(Tuple.Create("", 476), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 476), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 542), Tuple.Create("\"", 618)
, Tuple.Create(Tuple.Create("", 548), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.ActivityBase.js?", 548), true)
            
            #line 12 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
, Tuple.Create(Tuple.Create("", 599), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 599), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 665), Tuple.Create("\"", 740)
, Tuple.Create(Tuple.Create("", 671), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.Appointment.js?", 671), true)
            
            #line 13 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
, Tuple.Create(Tuple.Create("", 721), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 721), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CustomInit", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.onReady(function() {\r\n            var cardSettings = ");

            
            #line 20 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
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

WriteAttribute("title", Tuple.Create(" title=\"", 1260), Tuple.Create("\"", 1296)
            
            #line 30 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
, Tuple.Create(Tuple.Create("", 1268), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 1268), false)
);

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 31 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 32 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.HiddenFor(m => m.Status));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 33 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.HiddenFor(m => m.Priority));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 34 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.HiddenFor(m => m.IsNeedLookupInitialization));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 36 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.SectionHead("regardingObjectHeader", BLResources.TitleRegarding));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 37 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
        
            
            #line default
            #line hidden
            
            #line 37 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
          
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

            
            #line 46 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.SectionRow(
            @Html.TemplateField(m => m.Client, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Client }),
            @Html.TemplateField(m => m.Firm, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Firm, ExtendedInfo = "ForClientAndLinkedChild=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId", DataFields = firmDataFields, HeaderTextTemplate = HeaderTextTemplate })));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 49 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Deal, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Deal, ExtendedInfo = "ForClientAndLinkedChild=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId" })));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 51 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.SectionHead("planHeader", BLResources.TitlePlan));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 52 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Purpose, FieldFlex.lone, null, EnumResources.ResourceManager)));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 53 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Title, FieldFlex.lone)));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 55 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
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

            
            #line 61 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Attendee, FieldFlex.lone, new LookupSettings { EntityName = EntityName.Contact, ExtendedInfo = "ForClientAndLinkedChild=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId" })));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 62 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Location, FieldFlex.lone)));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 64 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.SectionHead("resultHeader", BLResources.TitleResult));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 65 "..\..\Views\CreateOrUpdate\Appointment.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Description, FieldFlex.lone, new Dictionary<string, object> { { "rows", "10" } })));

            
            #line default
            #line hidden
WriteLiteral("\r\n   </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
