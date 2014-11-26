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

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Views.CreateOrUpdate.Russia
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
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
    using DoubleGis.Erm.Platform.Common;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.Model.Entities.Enums;
    using DoubleGis.Erm.Platform.Model.Metadata.Enums;
    using DoubleGis.Erm.Platform.UI.Web.Mvc;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Russia/Appointment.cshtml")]
    public partial class Appointment : System.Web.Mvc.WebViewPage<Models.Russia.AppointmentViewModel>
    {
        public Appointment()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
  
    Layout = "../../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 139), Tuple.Create("\"", 196)
, Tuple.Create(Tuple.Create("", 145), Tuple.Create("/Scripts/Ext.ux.TimeComboBox.js?", 145), true)
            
            #line 9 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
, Tuple.Create(Tuple.Create("", 177), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 177), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 243), Tuple.Create("\"", 323)
, Tuple.Create(Tuple.Create("", 249), Tuple.Create("/Scripts/Russia/Ext.DoubleGis.UI.RussiaActivityBase.js?", 249), true)
            
            #line 10 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
, Tuple.Create(Tuple.Create("", 304), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 304), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(">\r\n        div.label-wrapper {\r\n            width: 180px !important;\r\n        }\r\n" +
"    </style>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 540), Tuple.Create("\"", 576)
            
            #line 20 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
, Tuple.Create(Tuple.Create("", 548), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 548), false)
);

WriteLiteral(">\r\n        <br />\r\n        <div");

WriteLiteral(" style=\"display: none\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 23 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
       Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n");

            
            #line 25 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
        
            
            #line default
            #line hidden
            
            #line 25 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
          
            var readonlyFieldHtmlAttributes = new Dictionary<string, object> { { "class", "readonly inputfields" }, { "readonly", "readonly" } };
        
            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 29 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
   Write(Html.SectionHead("planHeader", BLResources.TitlePlan));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 31 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
       Write(Html.TemplateField(m => m.Type, FieldFlex.twins, readonlyFieldHtmlAttributes, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 34 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
       Write(Html.TemplateField(m => m.Header, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"display-wrapper field-wrapper twins\"");

WriteLiteral(" id=\"ScheduledStart-wrapper\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"label-wrapper\"");

WriteLiteral(">\r\n                    <span>\r\n");

WriteLiteral("                        ");

            
            #line 40 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
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

            
            #line 48 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
                               Write(Html.DateFor(m => m.ScheduledStart, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                    ");

            
            #line 49 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
                               Write(Html.ValidationMessageFor(m => m.ScheduledStart, null, new Dictionary<string, object> { { "class", "error" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                                <td");

WriteLiteral(" style=\"width: 5px\"");

WriteLiteral("></td>\r\n                                <td");

WriteLiteral(" style=\"width: 60px\"");

WriteLiteral(">\r\n");

WriteLiteral("                                    ");

            
            #line 53 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
                               Write(Html.TextBoxFor(m => m.ScheduledStartTime, new Dictionary<string, object> { { "class", "timepicker inputfields" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                                <td");

WriteLiteral(" style=\"width: auto\"");

WriteLiteral("></td>\r\n                            </tr>\r\n                        </tbody>\r\n    " +
"                </table>\r\n                </div>\r\n            </div>\r\n        </" +
"div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"display-wrapper field-wrapper twins\"");

WriteLiteral(" id=\"ScheduledEnd-wrapper\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"label-wrapper\"");

WriteLiteral(">\r\n                    <span>\r\n");

WriteLiteral("                        ");

            
            #line 66 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
                   Write(Html.LabelFor(m => m.ScheduledEnd));

            
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

            
            #line 74 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
                               Write(Html.DateFor(m => m.ScheduledEnd, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                    ");

            
            #line 75 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
                               Write(Html.ValidationMessageFor(m => m.ScheduledEnd, null, new Dictionary<string, object> { { "class", "error" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                                <td");

WriteLiteral(" style=\"width: 5px\"");

WriteLiteral("></td>\r\n                                <td");

WriteLiteral(" style=\"width: 60px\"");

WriteLiteral(">\r\n");

WriteLiteral("                                    ");

            
            #line 79 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
                               Write(Html.TextBoxFor(m => m.ScheduledEndTime, new Dictionary<string, object> { { "class", "timepicker inputfields" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                                <td");

WriteLiteral(" style=\"width: auto\"");

WriteLiteral("></td>\r\n                            </tr>\r\n                        </tbody>\r\n    " +
"                </table>\r\n                </div>\r\n            </div>\r\n        </" +
"div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"display-wrapper field-wrapper twins\"");

WriteLiteral(" id=\"ActualEnd-wrapper\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"label-wrapper\"");

WriteLiteral(">\r\n                    <span>\r\n");

WriteLiteral("                        ");

            
            #line 92 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
                   Write(Html.LabelFor(m => m.ActualEnd));

            
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

            
            #line 100 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
                               Write(Html.DateFor(m => m.ActualEnd, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                    ");

            
            #line 101 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
                               Write(Html.ValidationMessageFor(m => m.ActualEnd, null, new Dictionary<string, object> { { "class", "error" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                                <td");

WriteLiteral(" style=\"width: 5px\"");

WriteLiteral("></td>\r\n                                <td");

WriteLiteral(" style=\"width: 60px\"");

WriteLiteral(">\r\n");

WriteLiteral("                                    ");

            
            #line 105 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
                               Write(Html.TextBoxFor(m => m.ActualEndTime, new Dictionary<string, object> { { "class", "timepicker inputfields" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                                <td");

WriteLiteral(" style=\"width: auto\"");

WriteLiteral("></td>\r\n                            </tr>\r\n                        </tbody>\r\n    " +
"                </table>\r\n                </div>\r\n            </div>\r\n        </" +
"div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 115 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
       Write(Html.TemplateField(m => m.Duration, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 118 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
       Write(Html.TemplateField(m => m.Priority, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 119 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
       Write(Html.TemplateField(m => m.Status, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n");

WriteLiteral("        ");

            
            #line 121 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
   Write(Html.SectionHead("planHeader", BLResources.TitlePurpose));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 123 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
       Write(Html.TemplateField(m => m.Purpose, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 126 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
       Write(Html.TemplateField(m => m.AfterSaleServiceType, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 129 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
       Write(Html.TemplateField(m => m.Description, FieldFlex.lone, new Dictionary<string, object> { { "rows", "5" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n");

WriteLiteral("        ");

            
            #line 131 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
   Write(Html.SectionHead("planHeader", BLResources.TitleRegarding));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 133 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
       Write(Html.TemplateField(m => m.Client, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Client }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 134 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
       Write(Html.TemplateField(m => m.Deal, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Deal, ExtendedInfo = "filterToParent=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 137 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
       Write(Html.TemplateField(m => m.Contact, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Contact, ExtendedInfo = "filterToParent=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 138 "..\..\Views\CreateOrUpdate\Russia\Appointment.cshtml"
       Write(Html.TemplateField(m => m.Firm, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Firm, ExtendedInfo = "filterToParent=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
