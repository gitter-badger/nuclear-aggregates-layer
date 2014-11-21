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

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas.Kazakhstan.Views.LegalPerson
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Areas/Kazakhstan/Views/LegalPerson/ChangeLegalPersonRequisites.cshtml")]
    public partial class ChangeLegalPersonRequisites : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Kazakhstan.KazakhstanChangeLegalPersonRequisitesViewModel>
    {
        public ChangeLegalPersonRequisites()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Areas\Kazakhstan\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
  
    Layout = "../../../../Views/Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 7 "..\..\Areas\Kazakhstan\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
            Write(BLResources.ControlChangeLegalPersonRequisites);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 8 "..\..\Areas\Kazakhstan\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
                  Write(BLResources.ControlChangeLegalPersonRequisites);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral("  ");

});

WriteLiteral("\r\n");

DefineSection("PageContent", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.onReady(function() {\r\n            if (Ext.getDom(\"Notifications\")." +
"innerHTML.trim() == \"OK\") {\r\n                window.returnValue = \'OK\';\r\n       " +
"         window.close();\r\n                return;\r\n            } else if (Ext.ge" +
"tDom(\"Notifications\").innerHTML.trim() != \"\") {\r\n                Ext.getDom(\"Not" +
"ifications\").style.display = \"block\";\r\n            }\r\n\r\n            var depList " +
"= window.Ext.getDom(\"ViewConfig_DependencyList\");\r\n            if (depList.value" +
") {\r\n                this.DependencyHandler = new window.Ext.DoubleGis.Dependenc" +
"yHandler();\r\n                this.DependencyHandler.register(window.Ext.decode(d" +
"epList.value), window.EntityForm);\r\n            }\r\n            depList.value = n" +
"ull;\r\n            Ext.get(\"Cancel\").on(\"click\", function() {\r\n                wi" +
"ndow.returnValue = \'CANCELED\';\r\n                window.close();\r\n            });" +
"\r\n            Ext.get(\"OK\").on(\"click\", function() {\r\n                if (Ext.Do" +
"ubleGis.FormValidator.validate(window.EntityForm)) {\r\n                    Ext.ge" +
"tDom(\"OK\").disabled = \"disabled\";\r\n                    Ext.getDom(\"Cancel\").disa" +
"bled = \"disabled\";\r\n\r\n                    window.Ext.each(window.Ext.query(\"inpu" +
"t.x-calendar\"), function (node) {\r\n                        var cmp = window.Ext." +
"getCmp(node.id);\r\n                        node.value = cmp.getValue() ? cmp.getV" +
"alue().format(Ext.CultureInfo.DateTimeFormatInfo.PhpInvariantDateTimePattern) : " +
"\"\";\r\n                    }, this);\r\n\r\n                    EntityForm.submit();\r\n" +
"                }\r\n            });\r\n        });\r\n    </script>\r\n");

            
            #line 48 "..\..\Areas\Kazakhstan\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
    
            
            #line default
            #line hidden
            
            #line 48 "..\..\Areas\Kazakhstan\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {

            
            #line default
            #line hidden
WriteLiteral("        <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 51 "..\..\Areas\Kazakhstan\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
       Write(Html.HiddenFor(m => m.LegalPersonP));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 52 "..\..\Areas\Kazakhstan\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
       Write(Html.HiddenFor(m => m.LegalPersonType));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 53 "..\..\Areas\Kazakhstan\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
       Write(Html.Hidden("ViewConfig_DependencyList", Model.ViewConfig.DependencyList));

            
            #line default
            #line hidden
WriteLiteral("\r\n            <div");

WriteLiteral(" style=\"display: none; height: 15px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(" class=\"Notifications\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 55 "..\..\Areas\Kazakhstan\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
           Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 58 "..\..\Areas\Kazakhstan\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
           Write(Html.TemplateField(m => m.LegalName, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 61 "..\..\Areas\Kazakhstan\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
           Write(Html.TemplateField(m => m.LegalAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 64 "..\..\Areas\Kazakhstan\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
           Write(Html.TemplateField(m => m.Bin, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                ");

            
            #line 65 "..\..\Areas\Kazakhstan\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
           Write(Html.TemplateField(m => m.BinIin, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                ");

            
            #line 66 "..\..\Areas\Kazakhstan\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
           Write(Html.TemplateField(m => m.Iin, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 69 "..\..\Areas\Kazakhstan\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
           Write(Html.TemplateField(m => m.IdentityCardNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                ");

            
            #line 70 "..\..\Areas\Kazakhstan\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
           Write(Html.TemplateField(m => m.IdentityCardIssuedOn, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 73 "..\..\Areas\Kazakhstan\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
           Write(Html.TemplateField(m => m.IdentityCardIssuedBy, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n        </div>\r\n");

            
            #line 76 "..\..\Areas\Kazakhstan\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
