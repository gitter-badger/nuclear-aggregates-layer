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

namespace DoubleGis.Erm.BLFlex.Web.Mvc.Global.Areas.Czech.Views.LegalPerson
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
    
    #line 1 "..\..\Areas\Czech\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
    using DoubleGis.Erm.UI.Web.Mvc.Utils;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Areas/Czech/Views/LegalPerson/ChangeLegalPersonRequisites.cshtml")]
    public partial class ChangeLegalPersonRequisites : System.Web.Mvc.WebViewPage<CzechChangeLegalPersonRequisitesViewModel>
    {
        public ChangeLegalPersonRequisites()
        {
        }
        public override void Execute()
        {
            
            #line 4 "..\..\Areas\Czech\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 8 "..\..\Areas\Czech\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
            Write(BLResources.ControlChangeLegalPersonRequisites);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 9 "..\..\Areas\Czech\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
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
        Ext.onReady(function ()
        {
            if (Ext.getDom(""Notifications"").innerHTML.trim() == ""OK"")
            {
                window.returnValue = 'OK';
                window.close();
                return;
            } else if (Ext.getDom(""Notifications"").innerHTML.trim() != """")
            {
                Ext.getDom(""Notifications"").style.display = ""block"";
            }

            var depList = window.Ext.getDom(""ViewConfig_DependencyList"");
            if (depList.value)
            {
                this.DependencyHandler = new window.Ext.DoubleGis.DependencyHandler();
                this.DependencyHandler.register(window.Ext.decode(depList.value), window.EntityForm);
            }
            depList.value = null;
            Ext.get(""Cancel"").on(""click"", function () { window.returnValue = 'CANCELED'; window.close(); });
            Ext.get(""OK"").on(""click"", function ()
            {
                if (Ext.DoubleGis.FormValidator.validate(window.EntityForm))
                {
                    Ext.getDom(""OK"").disabled = ""disabled"";
                    Ext.getDom(""Cancel"").disabled = ""disabled"";
                    EntityForm.submit();
                }
            });
        });
    </script>
");

            
            #line 46 "..\..\Areas\Czech\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
    
            
            #line default
            #line hidden
            
            #line 46 "..\..\Areas\Czech\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 49 "..\..\Areas\Czech\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
   Write(Html.HiddenFor(m => m.LegalPersonP));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 50 "..\..\Areas\Czech\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
   Write(Html.HiddenFor(m => m.LegalPersonType));

            
            #line default
            #line hidden
WriteLiteral("\r\n        \r\n");

WriteLiteral("        ");

            
            #line 52 "..\..\Areas\Czech\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
   Write(Html.Hidden("ViewConfig_DependencyList", Model.ViewConfig.DependencyList));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" style=\"display: none; height: 15px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(" class=\"Notifications\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 54 "..\..\Areas\Czech\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
       Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 57 "..\..\Areas\Czech\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
       Write(Html.TemplateField(m => m.LegalName, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 60 "..\..\Areas\Czech\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
       Write(Html.TemplateField(m => m.LegalAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 63 "..\..\Areas\Czech\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
       Write(Html.TemplateField(m => m.Ic, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 64 "..\..\Areas\Czech\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
       Write(Html.TemplateField(m => m.BusinessmanInn, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 65 "..\..\Areas\Czech\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
       Write(Html.TemplateField(m => m.Inn, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

            
            #line 68 "..\..\Areas\Czech\Views\LegalPerson\ChangeLegalPersonRequisites.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
