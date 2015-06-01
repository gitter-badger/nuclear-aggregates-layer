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

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Views.LocalMessage
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    using DoubleGis.Erm.BLCore.Resources.Server.Properties;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    using NuClear.Model.Common.Entities;

    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/LocalMessage/ImportFromFile.cshtml")]
    public partial class ImportFromFile : System.Web.Mvc.WebViewPage<LocalMessageImportFromFileViewModel>
    {
        public ImportFromFile()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 4 "..\..\Views\LocalMessage\ImportFromFile.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 8 "..\..\Views\LocalMessage\ImportFromFile.cshtml"
            Write(BLResources.LoadLocalMessageFromFileDialogTitle);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 9 "..\..\Views\LocalMessage\ImportFromFile.cshtml"
                  Write(BLResources.LoadLocalMessageFromFileDialogTitle);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 10 "..\..\Views\LocalMessage\ImportFromFile.cshtml"
                    Write(BLResources.LoadLocalMessageFromFileDialogTitle);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

WriteLiteral("\r\n");

DefineSection("PageContent", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n\r\n        Ext.onReady(function ()\r\n        {\r\n\r\n            //Show error messa" +
"ges\r\n            if (Ext.getDom(\"Notifications\").innerHTML.trim() == \"OK\")\r\n    " +
"        {\r\n                window.close();\r\n                return;\r\n           " +
" }\r\n            else if (Ext.getDom(\"Notifications\").innerHTML.trim() != \"\")\r\n  " +
"          {\r\n                Ext.getDom(\"Notifications\").style.display = \"block\"" +
";\r\n            }\r\n\r\n            var depList = window.Ext.getDom(\"ViewConfig_Depe" +
"ndencyList\");\r\n            if (depList.value)\r\n            {\r\n                th" +
"is.DependencyHandler = new window.Ext.DoubleGis.DependencyHandler();\r\n          " +
"      this.DependencyHandler.register(window.Ext.decode(depList.value), window.E" +
"ntityForm);\r\n            }\r\n\r\n            Ext.get(\"Cancel\").on(\"click\", function" +
" () { window.close(); });\r\n            Ext.get(\"OK\").on(\"click\", function ()\r\n  " +
"          {\r\n                if (Ext.DoubleGis.FormValidator.validate(window.Ent" +
"ityForm))\r\n                {\r\n                    Ext.getDom(\"OK\").disabled = \"d" +
"isabled\";\r\n                    Ext.getDom(\"Cancel\").disabled = \"disabled\";\r\n    " +
"                window.Ext.each(window.Ext.query(\"input.x-calendar\", window.Enti" +
"tyForm), function (node)\r\n                    {\r\n                        node.va" +
"lue = window.Ext.getCmp(node.id).getValue() ? new Date(window.Ext.getCmp(node.id" +
").getValue()).format(Ext.CultureInfo.DateTimeFormatInfo.PhpInvariantDateTimePatt" +
"ern) : \"\";\r\n                    });\r\n                    window.EntityForm.submi" +
"t();\r\n                }\r\n            });\r\n        });\r\n    </script>\r\n");

            
            #line 53 "..\..\Views\LocalMessage\ImportFromFile.cshtml"
    
            
            #line default
            #line hidden
            
            #line 53 "..\..\Views\LocalMessage\ImportFromFile.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" }, { "enctype", "multipart/form-data" } }))
    {

            
            #line default
            #line hidden
WriteLiteral("        <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 56 "..\..\Views\LocalMessage\ImportFromFile.cshtml"
       Write(Html.HiddenFor(m=>m.ViewConfig.DependencyList));

            
            #line default
            #line hidden
WriteLiteral("\r\n            <div");

WriteLiteral(" style=\"display: none; height: 15px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(" class=\"Notifications\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 58 "..\..\Views\LocalMessage\ImportFromFile.cshtml"
           Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 61 "..\..\Views\LocalMessage\ImportFromFile.cshtml"
           Write(Html.TemplateField(m => m.IntegrationType, FieldFlex.lone, null, LocalMessageController.IntegrationTypeImportResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 64 "..\..\Views\LocalMessage\ImportFromFile.cshtml"
           Write(Html.TemplateField(m => m.BranchOfficeOrganizationUnit, FieldFlex.lone, new LookupSettings { EntityName = EntityType.Instance.BranchOfficeOrganizationUnit() }));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"display-wrapper field-wrapper\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" class=\"label-wrapper\"");

WriteLiteral(">\r\n                        <label");

WriteLiteral(" for=\"file1\"");

WriteLiteral(">");

            
            #line 69 "..\..\Views\LocalMessage\ImportFromFile.cshtml"
                                      Write(BLResources.File);

            
            #line default
            #line hidden
WriteLiteral("</label>:\r\n                    </div>\r\n                    <div");

WriteLiteral(" class=\"input-wrapper\"");

WriteLiteral(">\r\n                        <input");

WriteLiteral(" type=\"file\"");

WriteLiteral(" id=\"file1\"");

WriteLiteral(" name=\"file\"");

WriteLiteral(" class=\"inputfields\"");

WriteLiteral(" />\r\n                    </div>\r\n                </div>\r\n            </div>\r\n    " +
"    </div>\r\n");

            
            #line 77 "..\..\Views\LocalMessage\ImportFromFile.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("    \r\n    ");

WriteLiteral("\r\n");

            
            #line 80 "..\..\Views\LocalMessage\ImportFromFile.cshtml"
        
            
            #line default
            #line hidden
            
            #line 80 "..\..\Views\LocalMessage\ImportFromFile.cshtml"
         if(Model != null && Model.MessageErrors != null && Model.MessageErrors.Length > 0)
        {
            using (Html.BeginForm("GetValidationResults", "LocalMessage", FormMethod.Post, new Dictionary<string, object> { { "target", "_blank" } }))
            {

            
            #line default
            #line hidden
WriteLiteral("               <input");

WriteLiteral(" type=\"hidden\"");

WriteLiteral(" name=\"errors\"");

WriteAttribute("value", Tuple.Create(" value=\"", 3698), Tuple.Create("\"", 3726)
            
            #line 84 "..\..\Views\LocalMessage\ImportFromFile.cshtml"
, Tuple.Create(Tuple.Create("", 3706), Tuple.Create<System.Object, System.Int32>(Model.MessageErrors
            
            #line default
            #line hidden
, 3706), false)
);

WriteLiteral(" />\r\n");

WriteLiteral("               <input");

WriteLiteral(" type=\"submit\"");

WriteLiteral(" name=\"button\"");

WriteAttribute("value", Tuple.Create(" value=\"", 3781), Tuple.Create("\"", 3808)
            
            #line 85 "..\..\Views\LocalMessage\ImportFromFile.cshtml"
, Tuple.Create(Tuple.Create("", 3789), Tuple.Create<System.Object, System.Int32>(BLResources.Errors
            
            #line default
            #line hidden
, 3789), false)
);

WriteLiteral(" />\r\n");

            
            #line 86 "..\..\Views\LocalMessage\ImportFromFile.cshtml"
            }
        }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
