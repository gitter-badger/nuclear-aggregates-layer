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

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Views.BranchOfficeOrganizationUnit
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/BranchOfficeOrganizationUnit/SetAsPrimary.cshtml")]
    public partial class SetAsPrimary : System.Web.Mvc.WebViewPage<Models.SetBranchOfficeOrganizationUnitStatusViewModel>
    {
        public SetAsPrimary()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\BranchOfficeOrganizationUnit\SetAsPrimary.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 7 "..\..\Views\BranchOfficeOrganizationUnit\SetAsPrimary.cshtml"
            Write(BLResources.ConfirmYourAction);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 8 "..\..\Views\BranchOfficeOrganizationUnit\SetAsPrimary.cshtml"
                  Write(BLResources.SetAsPrimary);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 9 "..\..\Views\BranchOfficeOrganizationUnit\SetAsPrimary.cshtml"
                    Write(BLResources.SetAsPrimaryLegend);

            
            #line default
            #line hidden
WriteLiteral(" ");

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
                window.close();
                return;
            }
            else if (Ext.getDom(""Notifications"").innerHTML.trim() != """" && Ext.getDom(""Notifications"").innerHTML.trim() != ""OK"")
            {
                Ext.get(""Notifications"").addClass(""Notifications"");
            }

            // write eventhandlers for buttons
            Ext.get(""Cancel"").on(""click"", function () { window.close(); });
            Ext.get(""OK"").on(""click"", submitForm);

        });

        var submitForm = function ()
        {
            Ext.getDom(""OK"").disabled = ""disabled"";
            Ext.getDom(""Cancel"").disabled = ""disabled"";
            window.Ext.get(""Notifications"").removeClass(""Notifications"");

            if (window.dialogArguments && window.dialogArguments.length)
            {
                Ext.getDom(""Id"").value = window.dialogArguments[0];
                window.EntityForm.submit();
            }
        };
    </script>

");

            
            #line 46 "..\..\Views\BranchOfficeOrganizationUnit\SetAsPrimary.cshtml"
    
            
            #line default
            #line hidden
            
            #line 46 "..\..\Views\BranchOfficeOrganizationUnit\SetAsPrimary.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {

            
            #line default
            #line hidden
WriteLiteral("        <table");

WriteLiteral(" cellspacing=\"5\"");

WriteLiteral(" cellpadding=\"5\"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(" height=\"100%\"");

WriteLiteral(">\r\n        <colgroup>\r\n            <col");

WriteLiteral(" width=\"26\"");

WriteLiteral(" />\r\n            <col />\r\n        </colgroup>\r\n        <tbody>\r\n            <tr>\r" +
"\n                <td");

WriteLiteral(" colspan=\"2\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" style=\"height: 24px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 57 "..\..\Views\BranchOfficeOrganizationUnit\SetAsPrimary.cshtml"
                   Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                </td>\r\n            </tr>\r\n            <tr");

WriteLiteral(" style=\"display: none\"");

WriteLiteral(">\r\n                <td>\r\n");

WriteLiteral("                    ");

            
            #line 62 "..\..\Views\BranchOfficeOrganizationUnit\SetAsPrimary.cshtml"
               Write(Html.HiddenFor(m=>m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </td>\r\n            </tr>\r\n        </tbody>\r\n    </table>\r\n");

            
            #line 67 "..\..\Views\BranchOfficeOrganizationUnit\SetAsPrimary.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
