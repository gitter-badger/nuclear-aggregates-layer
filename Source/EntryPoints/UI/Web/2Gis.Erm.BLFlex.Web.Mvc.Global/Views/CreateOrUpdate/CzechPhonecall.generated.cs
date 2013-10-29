﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18331
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.BLFlex.Web.Mvc.Global.Views.CreateOrUpdate
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
    using DoubleGis.Erm.BL.Resources.Server.Properties;
    using DoubleGis.Erm.Core;
    using DoubleGis.Erm.Core.Enums;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.UI.Web.Mvc;
    using DoubleGis.Erm.UI.Web.Mvc.Controllers;
    using DoubleGis.Erm.UI.Web.Mvc.Models;
    
    #line 1 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
    using DoubleGis.Erm.UI.Web.Mvc.Utils;
    
    #line default
    #line hidden
    
    #line 2 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
    using Platform.Common;
    
    #line default
    #line hidden
    
    #line 3 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
    using Platform.Web.Mvc.Utils;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/CzechPhonecall.cshtml")]
    public partial class CzechPhonecall : System.Web.Mvc.WebViewPage<CzechPhonecallViewModel>
    {
        public CzechPhonecall()
        {
        }
        public override void Execute()
        {
            
            #line 6 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 219), Tuple.Create("\"", 291)
, Tuple.Create(Tuple.Create("", 225), Tuple.Create("/Scripts/Ext.ux.TimeComboBox.js?", 225), true)
            
            #line 12 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
, Tuple.Create(Tuple.Create("", 257), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 257), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 338), Tuple.Create("\"", 431)
, Tuple.Create(Tuple.Create("", 344), Tuple.Create("/Scripts/Czech/Ext.DoubleGis.UI.CzechActivityBase.js?", 344), true)
            
            #line 13 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
, Tuple.Create(Tuple.Create("", 397), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 397), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(">\r\n        div.label-wrapper\r\n        {\r\n            width: 180px !important;\r\n  " +
"      }\r\n    </style>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 657), Tuple.Create("\"", 693)
            
            #line 24 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
, Tuple.Create(Tuple.Create("", 665), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 665), false)
);

WriteLiteral(">\r\n        <br />\r\n        <div");

WriteLiteral(" style=\"display: none\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 27 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
       Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n");

            
            #line 29 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
        
            
            #line default
            #line hidden
            
            #line 29 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
          
            var disabledFieldHtmlAttributes = new Dictionary<string, object> { { "class", "readonly inputfields" }, { "disabled", "true" } };
        
            
            #line default
            #line hidden
WriteLiteral("\r\n    \r\n");

WriteLiteral("        ");

            
            #line 33 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
   Write(Html.SectionHead("planHeader", BLResources.TitlePlan));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 35 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
       Write(Html.TemplateField(m => m.Type, FieldFlex.twins, disabledFieldHtmlAttributes, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 38 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
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

            
            #line 44 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
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

            
            #line 52 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
                               Write(Html.DateFor(m => m.ScheduledStart, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                    ");

            
            #line 53 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
                               Write(Html.ValidationMessageFor(m => m.ScheduledStart, null, new Dictionary<string, object> { { "class", "error" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                                <td");

WriteLiteral(" style=\"width: 5px\"");

WriteLiteral(">\r\n                                </td>\r\n                                <td");

WriteLiteral(" style=\"width: 60px\"");

WriteLiteral(">\r\n");

WriteLiteral("                                    ");

            
            #line 58 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
                               Write(Html.TextBoxFor(m => m.ScheduledStartTime, new Dictionary<string, object>{{ "class", "timepicker inputfields"}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                                <td");

WriteLiteral(" style=\"width: auto\"");

WriteLiteral(">\r\n                                </td>\r\n                            </tr>\r\n    " +
"                    </tbody>\r\n                    </table>\r\n                </di" +
"v>\r\n            </div>\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"display-wrapper field-wrapper twins\"");

WriteLiteral(" id=\"ScheduledEnd-wrapper\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"label-wrapper\"");

WriteLiteral(">\r\n                    <span>\r\n");

WriteLiteral("                        ");

            
            #line 72 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
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

            
            #line 80 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
                               Write(Html.DateFor(m => m.ScheduledEnd, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                    ");

            
            #line 81 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
                               Write(Html.ValidationMessageFor(m => m.ScheduledEnd, null, new Dictionary<string, object> { { "class", "error" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                                <td");

WriteLiteral(" style=\"width: 5px\"");

WriteLiteral(">\r\n                                </td>\r\n                                <td");

WriteLiteral(" style=\"width: 60px\"");

WriteLiteral(">\r\n");

WriteLiteral("                                    ");

            
            #line 86 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
                               Write(Html.TextBoxFor(m => m.ScheduledEndTime, new Dictionary<string, object>{{ "class", "timepicker inputfields"}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                                <td");

WriteLiteral(" style=\"width: auto\"");

WriteLiteral(">\r\n                                </td>\r\n                            </tr>\r\n    " +
"                    </tbody>\r\n                    </table>\r\n                </di" +
"v>\r\n            </div>\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"display-wrapper field-wrapper twins\"");

WriteLiteral(" id=\"ActualEnd-wrapper\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"label-wrapper\"");

WriteLiteral(">\r\n                    <span>\r\n");

WriteLiteral("                        ");

            
            #line 100 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
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

            
            #line 108 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
                               Write(Html.DateFor(m => m.ActualEnd, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                    ");

            
            #line 109 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
                               Write(Html.ValidationMessageFor(m => m.ActualEnd, null, new Dictionary<string, object> { { "class", "error" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                                <td");

WriteLiteral(" style=\"width: 5px\"");

WriteLiteral(">\r\n                                </td>\r\n                                <td");

WriteLiteral(" style=\"width: 60px\"");

WriteLiteral(">\r\n");

WriteLiteral("                                    ");

            
            #line 114 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
                               Write(Html.TextBoxFor(m => m.ActualEndTime, new Dictionary<string, object>{{ "class", "timepicker inputfields"}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                                <td");

WriteLiteral(" style=\"width: auto\"");

WriteLiteral(">\r\n                                </td>\r\n                            </tr>\r\n    " +
"                    </tbody>\r\n                    </table>\r\n                </di" +
"v>\r\n            </div>\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 125 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
       Write(Html.TemplateField(m => m.Duration, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 128 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
       Write(Html.TemplateField(m => m.Priority, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 129 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
       Write(Html.TemplateField(m => m.Status, FieldFlex.twins, disabledFieldHtmlAttributes, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n");

WriteLiteral("        ");

            
            #line 131 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
   Write(Html.SectionHead("planHeader", BLResources.TitlePurpose));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 133 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
       Write(Html.TemplateField(m => m.Purpose, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 136 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
       Write(Html.TemplateField(m => m.Description, FieldFlex.lone, new Dictionary<string, object> { { "rows", "5" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n");

WriteLiteral("        ");

            
            #line 138 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
   Write(Html.SectionHead("planHeader", BLResources.TitleRegarding));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 140 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
       Write(Html.TemplateField(m => m.Client, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Client }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 141 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
       Write(Html.TemplateField(m => m.Firm, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Firm }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 144 "..\..\Views\CreateOrUpdate\CzechPhonecall.cshtml"
       Write(Html.TemplateField(m => m.Contact, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Contact }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
