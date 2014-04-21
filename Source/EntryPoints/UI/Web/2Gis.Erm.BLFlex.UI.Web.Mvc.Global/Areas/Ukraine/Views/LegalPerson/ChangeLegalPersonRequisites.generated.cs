﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas.Ukraine.Views.LegalPerson
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Areas/Ukraine/Views/LegalPerson/ChangeLegalPersonRequisites.cshtml")]
    public partial class ChangeLegalPersonRequisites : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Ukraine.UkraineChangeLegalPersonRequisitesViewModel>
    {
        public ChangeLegalPersonRequisites()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Areas\Ukraine\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
  
    Layout = "../../../../Views/Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 7 "..\..\Areas\Ukraine\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
            Write(BLResources.ControlChangeLegalPersonRequisites);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 8 "..\..\Areas\Ukraine\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
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

WriteLiteral(@">
        Ext.onReady(function() {
            if (Ext.getDom(""Notifications"").innerHTML.trim() == ""OK"") {
                window.returnValue = 'OK';
                window.close();
                return;
            } else if (Ext.getDom(""Notifications"").innerHTML.trim() != """") {
                Ext.getDom(""Notifications"").style.display = ""block"";
            }

            var depList = window.Ext.getDom(""ViewConfig_DependencyList"");
            if (depList.value) {
                this.DependencyHandler = new window.Ext.DoubleGis.DependencyHandler();
                this.DependencyHandler.register(window.Ext.decode(depList.value), window.EntityForm);
            }
            depList.value = null;
            Ext.get(""Cancel"").on(""click"", function() {
                window.returnValue = 'CANCELED';
                window.close();
            });
            Ext.get(""OK"").on(""click"", function() {
                if (Ext.DoubleGis.FormValidator.validate(window.EntityForm)) {
                    Ext.getDom(""OK"").disabled = ""disabled"";
                    Ext.getDom(""Cancel"").disabled = ""disabled"";
                    EntityForm.submit();
                }
            });
        });
    </script>
");

            
            #line 42 "..\..\Areas\Ukraine\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
    
            
            #line default
            #line hidden
            
            #line 42 "..\..\Areas\Ukraine\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {

            
            #line default
            #line hidden
WriteLiteral("        <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 45 "..\..\Areas\Ukraine\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
       Write(Html.HiddenFor(m => m.LegalPersonP));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 46 "..\..\Areas\Ukraine\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
       Write(Html.HiddenFor(m => m.LegalPersonType));

            
            #line default
            #line hidden
WriteLiteral("\r\n        \r\n");

WriteLiteral("            ");

            
            #line 48 "..\..\Areas\Ukraine\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
       Write(Html.Hidden("ViewConfig_DependencyList", Model.ViewConfig.DependencyList));

            
            #line default
            #line hidden
WriteLiteral("\r\n            <div");

WriteLiteral(" style=\"display: none; height: 15px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(" class=\"Notifications\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 50 "..\..\Areas\Ukraine\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
           Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 53 "..\..\Areas\Ukraine\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
           Write(Html.TemplateField(m => m.LegalName, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 56 "..\..\Areas\Ukraine\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
           Write(Html.TemplateField(m => m.LegalAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 59 "..\..\Areas\Ukraine\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
           Write(Html.TemplateField(m => m.Ipn, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 62 "..\..\Areas\Ukraine\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
           Write(Html.TemplateField(m => m.Egrpou, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                ");

            
            #line 63 "..\..\Areas\Ukraine\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
           Write(Html.TemplateField(m => m.BusinessmanEgrpou, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 66 "..\..\Areas\Ukraine\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
           Write(Html.TemplateField(m => m.TaxationType, FieldFlex.lone, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n        </div>\r\n");

            
            #line 69 "..\..\Areas\Ukraine\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
