﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Views.ReleaseInfo
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    using DoubleGis.Erm.BLCore.Resources.Server.Properties;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.Model.Metadata.Enums;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/ReleaseInfo/ReleaseRevertDialog.cshtml")]
    public partial class ReleaseRevertDialog : System.Web.Mvc.WebViewPage<ReleaseRevertDialogViewModel>
    {
        public ReleaseRevertDialog()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\ReleaseInfo\ReleaseRevertDialog.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 7 "..\..\Views\ReleaseInfo\ReleaseRevertDialog.cshtml"
            Write(BLResources.RevertAssembly);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 8 "..\..\Views\ReleaseInfo\ReleaseRevertDialog.cshtml"
                  Write(BLResources.RevertAssembly);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 9 "..\..\Views\ReleaseInfo\ReleaseRevertDialog.cshtml"
                    Write(BLResources.SpecifyAssemblyPeriodAndOrganizationUnit);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

WriteLiteral("\r\n");

DefineSection("PageContent", () => {

WriteLiteral("\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(">\r\n        td.itemCaption\r\n        {\r\n            vertical-align: top;\r\n         " +
"   padding-top: 5px;\r\n        }\r\n        \r\n        td.itemValue\r\n        {\r\n    " +
"        vertical-align: top;\r\n        }\r\n    </style>\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.onReady(function ()\r\n        {\r\n            var isSuccess = \'");

            
            #line 28 "..\..\Views\ReleaseInfo\ReleaseRevertDialog.cshtml"
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
"ction ()\r\n            {\r\n                window.close();\r\n            });\r\n\r\n   " +
"         Ext.get(\"OK\").on(\"click\", function ()\r\n            {\r\n                E" +
"xt.get(\"Notifications\").removeClass(\"Notifications\");\r\n                if (Ext.D" +
"oubleGis.FormValidator.validate(EntityForm))\r\n                {\r\n               " +
"     Ext.getDom(\"OK\").disabled = \"disabled\";\r\n                    Ext.getDom(\"Ca" +
"ncel\").disabled = \"disabled\";\r\n                    window.Ext.each(window.Ext.qu" +
"ery(\"input.x-calendar\", window.EntityForm), function (node)\r\n                   " +
" {\r\n                        node.value = window.Ext.getCmp(node.id).getValue() ?" +
" new Date(window.Ext.getCmp(node.id).getValue()).format(Ext.CultureInfo.DateTime" +
"FormatInfo.PhpInvariantDateTimePattern) : \"\";\r\n                    });\r\n        " +
"            EntityForm.submit();\r\n                }\r\n            });\r\n        })" +
";\r\n    </script>\r\n");

            
            #line 69 "..\..\Views\ReleaseInfo\ReleaseRevertDialog.cshtml"
    
            
            #line default
            #line hidden
            
            #line 69 "..\..\Views\ReleaseInfo\ReleaseRevertDialog.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" style=\"display: none; height: 15px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(" class=\"Notifications\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 73 "..\..\Views\ReleaseInfo\ReleaseRevertDialog.cshtml"
       Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 76 "..\..\Views\ReleaseInfo\ReleaseRevertDialog.cshtml"
       Write(Html.TemplateField(m => m.OrganizationUnit, FieldFlex.lone, new LookupSettings {EntityName = EntityName.OrganizationUnit}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 79 "..\..\Views\ReleaseInfo\ReleaseRevertDialog.cshtml"
       Write(Html.TemplateField(m => m.PeriodStart, FieldFlex.lone, new DateTimeSettings {ShiftOffset = false, PeriodType = PeriodType.MonthlyLowerBound}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 82 "..\..\Views\ReleaseInfo\ReleaseRevertDialog.cshtml"
       Write(Html.TemplateField(m => m.Comment, FieldFlex.lone, new Dictionary<string, object> { { "rows", "5" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

            
            #line 85 "..\..\Views\ReleaseInfo\ReleaseRevertDialog.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
