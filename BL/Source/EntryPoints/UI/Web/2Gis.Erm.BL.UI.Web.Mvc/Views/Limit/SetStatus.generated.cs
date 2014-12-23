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

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Views.Limit
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Limit/SetStatus.cshtml")]
    public partial class SetStatus : System.Web.Mvc.WebViewPage<SetLimitStatusViewModel>
    {
        public SetStatus()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\Limit\SetStatus.cshtml"
  
	Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 7 "..\..\Views\Limit\SetStatus.cshtml"
            Write(BLResources.SetLimitStatusDialogTitle);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 8 "..\..\Views\Limit\SetStatus.cshtml"
                  Write(BLResources.SetLimitStatusDialogTitle);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

});

WriteLiteral("\r\n");

DefineSection("PageScript", () => {

WriteLiteral("\r\n\t<script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n    Ext.onReady(function () {\r\n        Ext.get(\"Cancel\").on(\"click\", function " +
"() { window.close(); });\r\n        Ext.get(\"OK\").on(\"click\", function () {\r\n     " +
"       Ext.getDom(\"Id\").value = limitId;\r\n            Ext.getDom(\"Status\").value" +
" = status;\r\n            Ext.getDom(\"CrmIds\").value = crmIds;\r\n\r\n            Ext." +
"getDom(\"OK\").disabled = \"disabled\";\r\n            Ext.getDom(\"Cancel\").disabled =" +
" \"disabled\";\r\n            EntityForm.submit();\r\n        });\r\n\r\n        if (Ext.g" +
"etDom(\"Notifications\").innerHTML.trim() == \"OK\") {\r\n            window.close();\r" +
"\n            return;\r\n        }\r\n        else if (Ext.getDom(\"Notifications\").in" +
"nerHTML.trim() != \"\") {\r\n            Ext.get(\"Notifications\").addClass(\"Notifica" +
"tions\");\r\n            Ext.getDom(\"OK\").disabled = \"disabled\";\r\n            retur" +
"n;\r\n        }\r\n\r\n        var crmIds;\r\n        var limitId;\r\n        var status;\r" +
"\n\r\n        if (window.dialogArguments) {\r\n            limitId = window.dialogArg" +
"uments.limitId;\r\n            status = window.dialogArguments.status;\r\n        } " +
"else {\r\n            var queryParameters = Ext.urlDecode(window.location.search.s" +
"ubstring(1));\r\n            crmIds = queryParameters.CrmIds;\r\n            status " +
"= queryParameters.Status;\r\n        }\r\n\r\n        var statusLocalized;\r\n        sw" +
"itch (status) {\r\n            case \'Opened\':\r\n                statusLocalized = E" +
"xt.LocalizedResources.LimitStatusOpened;\r\n                break;\r\n            ca" +
"se \'Approved\':\r\n                statusLocalized = Ext.LocalizedResources.LimitSt" +
"atusApproved;\r\n                break;\r\n            case \'Rejected\':\r\n           " +
"     statusLocalized = Ext.LocalizedResources.LimitStatusRejected;\r\n            " +
"    break;\r\n            default:\r\n                statusLocalized = Ext.Localize" +
"dResources.LimitStatusError;\r\n                break;\r\n        }\r\n        Ext.get" +
"Dom(\"TopBarMessage\").innerText = Ext.LocalizedResources.SetLimitStatusDialogConf" +
"irmation + \' \' + statusLocalized + \'?\';\r\n    });\r\n\t</script>    \r\n");

});

WriteLiteral("\r\n");

DefineSection("PageContent", () => {

WriteLiteral("\r\n    <div");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 72 "..\..\Views\Limit\SetStatus.cshtml"
   Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    \r\n");

            
            #line 75 "..\..\Views\Limit\SetStatus.cshtml"
    
            
            #line default
            #line hidden
            
            #line 75 "..\..\Views\Limit\SetStatus.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {
		
            
            #line default
            #line hidden
            
            #line 77 "..\..\Views\Limit\SetStatus.cshtml"
   Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
            
            #line 77 "..\..\Views\Limit\SetStatus.cshtml"
                                  
		
            
            #line default
            #line hidden
            
            #line 78 "..\..\Views\Limit\SetStatus.cshtml"
   Write(Html.HiddenFor(m => m.Status));

            
            #line default
            #line hidden
            
            #line 78 "..\..\Views\Limit\SetStatus.cshtml"
                                      
		
            
            #line default
            #line hidden
            
            #line 79 "..\..\Views\Limit\SetStatus.cshtml"
   Write(Html.HiddenFor(m => m.CrmIds));

            
            #line default
            #line hidden
            
            #line 79 "..\..\Views\Limit\SetStatus.cshtml"
                                      
	}

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
