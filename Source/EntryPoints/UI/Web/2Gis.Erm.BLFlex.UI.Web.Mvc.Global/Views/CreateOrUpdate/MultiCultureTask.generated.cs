﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Views.CreateOrUpdate
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc.Html;
#line 1 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
    using BLCore.UI.Web.Mvc.Utils;
    
    #line default
    #line hidden
    using DoubleGis.Erm.BLCore.Resources.Server.Properties;
    using DoubleGis.Erm.Platform.Common;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/MultiCultureTask.cshtml")]
    public partial class MultiCultureTask : System.Web.Mvc.WebViewPage<Models.MultiCultureTaskViewModel>
    {
        public MultiCultureTask()
        {
        }
        public override void Execute()
        {
            
            #line 4 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 166), Tuple.Create("\"", 238)
, Tuple.Create(Tuple.Create("", 172), Tuple.Create("/Scripts/Ext.ux.TimeComboBox.js?", 172), true)
            
            #line 10 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
, Tuple.Create(Tuple.Create("", 204), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 204), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 285), Tuple.Create("\"", 367)
, Tuple.Create(Tuple.Create("", 291), Tuple.Create("/Scripts/Ext.DoubleGis.UI.ActivityBase.js?", 291), true)
            
            #line 11 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
, Tuple.Create(Tuple.Create("", 333), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 333), false)
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

WriteAttribute("title", Tuple.Create(" title=\"", 584), Tuple.Create("\"", 620)
            
            #line 21 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
, Tuple.Create(Tuple.Create("", 592), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 592), false)
);

WriteLiteral(">\r\n        <br />\r\n        <div");

WriteLiteral(" style=\"display: none\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 24 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
       Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n");

            
            #line 27 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
        
            
            #line default
            #line hidden
            
            #line 27 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
          
            var readonlyFieldHtmlAttributes = new Dictionary<string, object> { { "class", "readonly inputfields" }, { "readonly", "readonly" } };
        
            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 31 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
   Write(Html.SectionHead("planHeader", BLResources.TitlePlan));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 33 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
       Write(Html.TemplateField(m => m.Type, FieldFlex.twins, readonlyFieldHtmlAttributes, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 34 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
       Write(Html.TemplateField(m => m.TaskType, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 37 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
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

            
            #line 43 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
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

            
            #line 51 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
                               Write(Html.DateFor(m => m.ScheduledStart, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                    ");

            
            #line 52 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
                               Write(Html.ValidationMessageFor(m => m.ScheduledStart, null, new Dictionary<string, object> { { "class", "error" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                                <td");

WriteLiteral(" style=\"width: 5px\"");

WriteLiteral("></td>\r\n                                <td");

WriteLiteral(" style=\"width: 60px\"");

WriteLiteral(">\r\n");

WriteLiteral("                                    ");

            
            #line 56 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
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

            
            #line 69 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
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

            
            #line 77 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
                               Write(Html.DateFor(m => m.ScheduledEnd, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                    ");

            
            #line 78 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
                               Write(Html.ValidationMessageFor(m => m.ScheduledEnd, null, new Dictionary<string, object> { { "class", "error" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                                <td");

WriteLiteral(" style=\"width: 5px\"");

WriteLiteral("></td>\r\n                                <td");

WriteLiteral(" style=\"width: 60px\"");

WriteLiteral(">\r\n");

WriteLiteral("                                    ");

            
            #line 82 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
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

            
            #line 95 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
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

            
            #line 103 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
                               Write(Html.DateFor(m => m.ActualEnd, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                    ");

            
            #line 104 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
                               Write(Html.ValidationMessageFor(m => m.ActualEnd, null, new Dictionary<string, object> { { "class", "error" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                                <td");

WriteLiteral(" style=\"width: 5px\"");

WriteLiteral("></td>\r\n                                <td");

WriteLiteral(" style=\"width: 60px\"");

WriteLiteral(">\r\n");

WriteLiteral("                                    ");

            
            #line 108 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
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

            
            #line 118 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
       Write(Html.TemplateField(m => m.Duration, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 121 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
       Write(Html.TemplateField(m => m.Priority, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 122 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
       Write(Html.TemplateField(m => m.Status, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 125 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
       Write(Html.TemplateField(m => m.Description, FieldFlex.lone, new Dictionary<string, object> { { "rows", "5" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n");

WriteLiteral("        ");

            
            #line 127 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
   Write(Html.SectionHead("planHeader", BLResources.TitleRegarding));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 129 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
       Write(Html.TemplateField(m => m.Client, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Client }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 130 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
       Write(Html.TemplateField(m => m.Firm, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Firm, ExtendedInfo = "filterToParent=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 133 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
       Write(Html.TemplateField(m => m.Contact, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Contact, ExtendedInfo = "filterToParent=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

WriteLiteral("\r\n");

            
            #line 138 "..\..\Views\CreateOrUpdate\MultiCultureTask.cshtml"
Write(RenderBody());

            
            #line default
            #line hidden
WriteLiteral("\r\n");

        }
    }
}
#pragma warning restore 1591
