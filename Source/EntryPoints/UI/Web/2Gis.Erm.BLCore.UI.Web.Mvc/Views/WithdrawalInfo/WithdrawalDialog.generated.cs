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

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Views.WithdrawalInfo
{
    using System.Collections.Generic;
    
    #line 1 "..\..\Views\WithdrawalInfo\WithdrawalDialog.cshtml"
#line default
    #line hidden
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    using DoubleGis.Erm.BLCore.Resources.Server.Properties;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.Model.Metadata.Enums;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    
    #line 2 "..\..\Views\WithdrawalInfo\WithdrawalDialog.cshtml"

#line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/WithdrawalInfo/WithdrawalDialog.cshtml")]
    public partial class WithdrawalDialog : System.Web.Mvc.WebViewPage<WithdrawalDialogViewModel>
    {
        public WithdrawalDialog()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 6 "..\..\Views\WithdrawalInfo\WithdrawalDialog.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 10 "..\..\Views\WithdrawalInfo\WithdrawalDialog.cshtml"
            Write(BLResources.Withdrawal);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 11 "..\..\Views\WithdrawalInfo\WithdrawalDialog.cshtml"
                  Write(BLResources.Withdrawal);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 12 "..\..\Views\WithdrawalInfo\WithdrawalDialog.cshtml"
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
"rtical-align: top;\r\n        }\r\n    </style>\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.onReady(function ()\r\n        {\r\n            Ext.each(Ext.CardLooku" +
"pSettings, function (item, i)\r\n            {\r\n                new window.Ext.ux." +
"LookupField(item);\r\n            }, this);\r\n            \r\n            var isSucce" +
"ss = \'");

            
            #line 35 "..\..\Views\WithdrawalInfo\WithdrawalDialog.cshtml"
                        Write(Model.IsSuccess);

            
            #line default
            #line hidden
WriteLiteral("\';\r\n            if (isSuccess == \'True\') {\r\n                alert(Ext.getDom(\"Not" +
"ifications\").innerHTML.trim());\r\n                window.close();\r\n              " +
"  return;\r\n            } else if (Ext.getDom(\"Notifications\").innerHTML.trim() !" +
"= \"\")\r\n            {\r\n                Ext.getDom(\"Notifications\").style.display " +
"= \"block\";\r\n            }\r\n\r\n            // show error messages\r\n            if " +
"(Ext.getDom(\"Notifications\").innerHTML.trim() != \"\")\r\n            {\r\n           " +
"     Ext.get(\"Notifications\").addClass(\"Notifications\");\r\n            }\r\n       " +
"     else\r\n            {\r\n                Ext.get(\"Notifications\").removeClass(\"" +
"Notifications\");\r\n            }\r\n\r\n            Ext.get(\"Cancel\").on(\"click\", fun" +
"ction () { window.close(); });\r\n            Ext.get(\"OK\").on(\"click\", function (" +
")\r\n            {\r\n                if (Ext.DoubleGis.FormValidator.validate(Entit" +
"yForm))\r\n                {\r\n                    Ext.getDom(\"OK\").disabled = \"dis" +
"abled\";\r\n                    Ext.getDom(\"Cancel\").disabled = \"disabled\";\r\n      " +
"              window.Ext.each(window.Ext.query(\"input.x-calendar\", window.Entity" +
"Form), function (node)\r\n                    {\r\n                        node.valu" +
"e = window.Ext.getCmp(node.id).getValue() ? new Date(window.Ext.getCmp(node.id)." +
"getValue()).format(Ext.CultureInfo.DateTimeFormatInfo.PhpInvariantDateTimePatter" +
"n) : \"\";\r\n                    });\r\n                    EntityForm.submit();\r\n   " +
"             }\r\n            });\r\n        });\r\n    </script>\r\n");

            
            #line 71 "..\..\Views\WithdrawalInfo\WithdrawalDialog.cshtml"
    
            
            #line default
            #line hidden
            
            #line 71 "..\..\Views\WithdrawalInfo\WithdrawalDialog.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
{

            
            #line default
            #line hidden
WriteLiteral("        <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" style=\"display: none; height: 30px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(" class=\"Notifications\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 75 "..\..\Views\WithdrawalInfo\WithdrawalDialog.cshtml"
           Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 78 "..\..\Views\WithdrawalInfo\WithdrawalDialog.cshtml"
           Write(Html.TemplateField(m => m.OrganizationUnit, FieldFlex.lone, new LookupSettings{EntityName = EntityName.OrganizationUnit, ExtendedInfo = "restrictByUser=true"}));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 81 "..\..\Views\WithdrawalInfo\WithdrawalDialog.cshtml"
           Write(Html.TemplateField(m => m.PeriodStart, FieldFlex.lone, new DateTimeSettings {ShiftOffset = false, PeriodType = PeriodType.MonthlyLowerBound}));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n        </div>\r\n");

            
            #line 84 "..\..\Views\WithdrawalInfo\WithdrawalDialog.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
