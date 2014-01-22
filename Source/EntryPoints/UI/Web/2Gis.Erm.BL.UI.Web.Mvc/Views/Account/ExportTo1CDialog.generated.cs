﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34003
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Views.Account
{
    using System;
    using System.Collections.Generic;
    
    #line 1 "..\..\Views\Account\ExportTo1CDialog.cshtml"
    using System.Globalization;
    
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
    
    #line 2 "..\..\Views\Account\ExportTo1CDialog.cshtml"
    using Platform.Model.Metadata.Enums;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Account/ExportTo1CDialog.cshtml")]
    public partial class ExportTo1CDialog : System.Web.Mvc.WebViewPage<Models.ExportAccountTo1CViewModel>
    {
        public ExportTo1CDialog()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 6 "..\..\Views\Account\ExportTo1CDialog.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 10 "..\..\Views\Account\ExportTo1CDialog.cshtml"
            Write(BLResources.Withdrawal);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 11 "..\..\Views\Account\ExportTo1CDialog.cshtml"
                  Write(BLResources.Withdrawal);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 12 "..\..\Views\Account\ExportTo1CDialog.cshtml"
                    Write(BLResources.SpecifyPeriodAndOrganizationUnit);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

WriteLiteral("\r\n");

DefineSection("PageContent", () => {

WriteLiteral("\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(">\r\n        td.itemCaption\r\n        {\r\n            vertical-align: top;\r\n         " +
"   padding-top: 5px;\r\n        }\r\n        td.itemValue\r\n        {\r\n            ve" +
"rtical-align: top;\r\n        }\r\n    </style>\r\n    \r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n            Ext.onReady(function () {\r\n                \r\n                if (E" +
"xt.getDom(\'ErrorsLink\')) {\r\n                    Ext.getDom(\'ErrorsLink\').onclick" +
" = function () {\r\n                        Ext.getDom(\'ResultForm\').submit();\r\n  " +
"                  };\r\n                }\r\n\r\n                Ext.each(Ext.CardLook" +
"upSettings, function (item, i) {\r\n                    new window.Ext.ux.LookupFi" +
"eld(item);\r\n                }, this);\r\n\r\n                if (Ext.getDom(\"Notific" +
"ations\").innerHTML.trim() == \"OK\") {\r\n \r\n            } else if (Ext.getDom(\"Noti" +
"fications\").innerHTML.trim() != \"\") {\r\n                Ext.getDom(\"Notifications" +
"\").style.display = \"block\";\r\n            }\r\n                \r\n\r\n            // s" +
"how error messages\r\n            if (Ext.getDom(\"Notifications\").innerHTML.trim()" +
" != \"\") {\r\n                Ext.get(\"Notifications\").addClass(\"Notifications\");\r\n" +
"            }\r\n            else {\r\n                Ext.get(\"Notifications\").remo" +
"veClass(\"Notifications\");\r\n            }\r\n\r\n            Ext.get(\"Cancel\").on(\"cl" +
"ick\", function () { window.close(); });\r\n            Ext.get(\"OK\").on(\"click\", f" +
"unction () {\r\n                if (Ext.DoubleGis.FormValidator.validate(EntityFor" +
"m)) {\r\n                    Ext.getDom(\"OK\").disabled = \"disabled\";\r\n            " +
"        Ext.getDom(\"Cancel\").disabled = \"disabled\";\r\n                    window." +
"Ext.each(window.Ext.query(\"input.x-calendar\", window.EntityForm), function (node" +
") {\r\n                        node.value = window.Ext.getCmp(node.id).getValue() " +
"? new Date(window.Ext.getCmp(node.id).getValue()).format(Ext.CultureInfo.DateTim" +
"eFormatInfo.PhpInvariantDateTimePattern) : \"\";\r\n                    });\r\n       " +
"             EntityForm.submit();\r\n                }\r\n            });\r\n        }" +
");\r\n    </script>\r\n    \r\n");

            
            #line 70 "..\..\Views\Account\ExportTo1CDialog.cshtml"
    
            
            #line default
            #line hidden
            
            #line 70 "..\..\Views\Account\ExportTo1CDialog.cshtml"
     if(Model.HasResult == true)
    {

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" style=\"height: 8px; padding-left: 5px;padding-top: 4px;position: fixed;\"");

WriteLiteral(" id=\"DivErrors\"");

WriteLiteral(">\r\n");

            
            #line 73 "..\..\Views\Account\ExportTo1CDialog.cshtml"
        
            
            #line default
            #line hidden
            
            #line 73 "..\..\Views\Account\ExportTo1CDialog.cshtml"
         using(Html.BeginForm("GetOperationLog", "Operation", FormMethod.Post, new Dictionary<string, object> { { "target", "_blank" }, {"id", "ResultForm"} }))
        {

            
            #line default
            #line hidden
WriteLiteral("            <input");

WriteLiteral(" type=\"hidden\"");

WriteLiteral(" name=\"operationId\"");

WriteAttribute("value", Tuple.Create(" value=\"", 2787), Tuple.Create("\"", 2808)
            
            #line 75 "..\..\Views\Account\ExportTo1CDialog.cshtml"
, Tuple.Create(Tuple.Create("", 2795), Tuple.Create<System.Object, System.Int32>(Model.FileId
            
            #line default
            #line hidden
, 2795), false)
);

WriteLiteral(" />\r\n");

            
            #line 76 "..\..\Views\Account\ExportTo1CDialog.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n");

            
            #line 78 "..\..\Views\Account\ExportTo1CDialog.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("\r\n    <div");

WriteLiteral(" style=\"height: 18px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(" class=\"Notifications\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 81 "..\..\Views\Account\ExportTo1CDialog.cshtml"
       Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 82 "..\..\Views\Account\ExportTo1CDialog.cshtml"
            
            
            #line default
            #line hidden
            
            #line 82 "..\..\Views\Account\ExportTo1CDialog.cshtml"
             if (Model.HasResult == true)
            {

            
            #line default
            #line hidden
WriteLiteral("                <a");

WriteLiteral(" href=\"#\"");

WriteLiteral(" id=\"ErrorsLink\"");

WriteLiteral("> Просмотреть результат...</a>\r\n");

            
            #line 85 "..\..\Views\Account\ExportTo1CDialog.cshtml"
            }

            
            #line default
            #line hidden
WriteLiteral("        </div>\r\n\r\n");

            
            #line 88 "..\..\Views\Account\ExportTo1CDialog.cshtml"
    
            
            #line default
            #line hidden
            
            #line 88 "..\..\Views\Account\ExportTo1CDialog.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {
        
            
            #line default
            #line hidden
            
            #line 90 "..\..\Views\Account\ExportTo1CDialog.cshtml"
   Write(Html.Hidden("now", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)));

            
            #line default
            #line hidden
            
            #line 90 "..\..\Views\Account\ExportTo1CDialog.cshtml"
                                                                                   

            
            #line default
            #line hidden
WriteLiteral("        <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" style=\"display: none; height: 15px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(" class=\"Notifications\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 93 "..\..\Views\Account\ExportTo1CDialog.cshtml"
       Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 96 "..\..\Views\Account\ExportTo1CDialog.cshtml"
       Write(Html.TemplateField(m => m.OrganizationUnit, FieldFlex.lone, new LookupSettings{EntityName = EntityName.OrganizationUnit, ExtendedInfo = "restrictByFranchisees=true"}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 99 "..\..\Views\Account\ExportTo1CDialog.cshtml"
       Write(Html.TemplateField(m => m.PeriodStart, FieldFlex.lone, new DateTimeSettings {ShiftOffset = false, PeriodType = PeriodType.MonthlyLowerBound, DisplayStyle = DisplayStyle.WithoutDayNumber}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

            
            #line 102 "..\..\Views\Account\ExportTo1CDialog.cshtml"
        

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" style=\"display: none\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" id=\"MessageType\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 105 "..\..\Views\Account\ExportTo1CDialog.cshtml"
       Write(Model.MessageType);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n        <div");

WriteLiteral(" id=\"Message\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 107 "..\..\Views\Account\ExportTo1CDialog.cshtml"
       Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n    </div>\r\n");

            
            #line 109 "..\..\Views\Account\ExportTo1CDialog.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
