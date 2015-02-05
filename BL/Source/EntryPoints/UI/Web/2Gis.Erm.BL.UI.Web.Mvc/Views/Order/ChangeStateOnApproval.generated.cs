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

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Views.Order
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
    
    #line 1 "..\..\Views\Order\ChangeStateOnApproval.cshtml"
    using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
    
    #line default
    #line hidden
    using DoubleGis.Erm.Platform.Common;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.Model.Entities.Enums;
    using DoubleGis.Erm.Platform.Model.Metadata.Enums;
    using DoubleGis.Erm.Platform.UI.Web.Mvc;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Order/ChangeStateOnApproval.cshtml")]
    public partial class ChangeStateOnApproval : System.Web.Mvc.WebViewPage<ChangeOrderStateOnApprovalViewModel>
    {
        public ChangeStateOnApproval()
        {
        }
        public override void Execute()
        {
            
            #line 4 "..\..\Views\Order\ChangeStateOnApproval.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

});

WriteLiteral("\r\n");

DefineSection("PageContent", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">
        Ext.onReady(function ()
        {

            var processResult = function ()
            {
                return {
                    InspectorId: Ext.getDom(""InspectorId"").value,
                    InspectorName: Ext.getDom(""InspectorName"").value
                };
            };

            //Show error messages
            if (Ext.getDom(""Notifications"").innerHTML.trim() == ""OK"")
            {
                window.returnValue = processResult();
                window.close();
                return;
            }
            else if (Ext.getDom(""Notifications"").innerHTML.trim() != """")
            {
                Ext.getDom(""Notifications"").style.display = ""block"";
            }

            Ext.get(""Cancel"").on(""click"", function () { window.close(); });
            Ext.get(""OK"").on(""click"", function ()
            {
                if (Ext.DoubleGis.FormValidator.validate(window.EntityForm))
                {
                    Ext.getDom(""OK"").disabled = ""disabled"";
                    Ext.getDom(""Cancel"").disabled = ""disabled"";
                    window.EntityForm.submit();
                }
            });
        });
    </script>
");

            
            #line 50 "..\..\Views\Order\ChangeStateOnApproval.cshtml"
    
            
            #line default
            #line hidden
            
            #line 50 "..\..\Views\Order\ChangeStateOnApproval.cshtml"
     using (Html.BeginForm("ChangeStateOnApproval", null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 53 "..\..\Views\Order\ChangeStateOnApproval.cshtml"
   Write(Html.HiddenFor(m => m.OrderId));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" style=\"display: none; height: 15px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(" class=\"Notifications\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 55 "..\..\Views\Order\ChangeStateOnApproval.cshtml"
       Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 58 "..\..\Views\Order\ChangeStateOnApproval.cshtml"
       Write(Html.TemplateField(m => m.Inspector, FieldFlex.lone, new LookupSettings
                { 
                    EntityName = EntityName.User, 
                    ExtendedInfo = "privilege=" + (int)FunctionalPrivilegeName.OrderStatesAccess + "&orgUnitId=" + Model.SourceOrganizationUnitId 
                }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

            
            #line 65 "..\..\Views\Order\ChangeStateOnApproval.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
