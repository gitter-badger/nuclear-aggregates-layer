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

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Views.Deal
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Deal/ChangeDealClient.cshtml")]
    public partial class ChangeDealClient : System.Web.Mvc.WebViewPage<ChangeDealClientViewModel>
    {
        public ChangeDealClient()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 4 "..\..\Views\Deal\ChangeDealClient.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 8 "..\..\Views\Deal\ChangeDealClient.cshtml"
            Write(BLResources.ChangeClient);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 9 "..\..\Views\Deal\ChangeDealClient.cshtml"
                  Write(BLResources.ChangeClient);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 10 "..\..\Views\Deal\ChangeDealClient.cshtml"
                    Write(BLResources.ChangeClientLegend);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

WriteLiteral("\r\n");

DefineSection("PageContent", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.onReady(function ()\r\n        {\r\n\r\n            Ext.each(Ext.CardLoo" +
"kupSettings, function (item, i)\r\n            {\r\n                new window.Ext.u" +
"x.LookupField(item);\r\n            }, this);\r\n\r\n            //Show error messages" +
"\r\n            if (Ext.getDom(\"Notifications\").innerHTML.trim() == \"OK\")\r\n       " +
"     {\r\n                window.returnValue = true;\r\n                window.close" +
"();\r\n                return;\r\n            }\r\n            else if (Ext.getDom(\"No" +
"tifications\").innerHTML.trim() != \"\")\r\n            {\r\n                Ext.get(\"N" +
"otifications\").addClass(\"Notifications\");\r\n            }\r\n\r\n            if (!win" +
"dow.dialogArguments.length)\r\n            {\r\n                alert(\'\"Resources.Ne" +
"edToSelectOneOrMoreItems\');\r\n                window.close();\r\n                re" +
"turn;\r\n            }\r\n            else\r\n            {\r\n                Ext.getDo" +
"m(\"DealId\").value = window.dialogArguments;\r\n            }\r\n\r\n            //writ" +
"e eventhandlers for buttons\r\n            Ext.get(\"Cancel\").on(\"click\", function " +
"() { window.returnValue = null; window.close(); });\r\n            Ext.get(\"OK\").o" +
"n(\"click\", function ()\r\n            {\r\n                if (window.Ext.getDom(\"Cl" +
"ientId\").value == \"\")\r\n                {\r\n                    window.Ext.Message" +
"Box.show({\r\n                        title: \'\',\r\n                        msg: \'Не" +
"обходимо указать клиента\',\r\n                        buttons: window.Ext.MessageB" +
"ox.OK,\r\n                        width: 300,\r\n                        icon: windo" +
"w.Ext.MessageBox.ERROR\r\n                    });\r\n                    return;\r\n  " +
"              }\r\n\r\n                Ext.getDom(\"OK\").disabled = \"disabled\";\r\n    " +
"            Ext.getDom(\"Cancel\").disabled = \"disabled\";\r\n                EntityF" +
"orm.submit();\r\n            });\r\n        });\r\n\r\n    </script>\r\n");

            
            #line 69 "..\..\Views\Deal\ChangeDealClient.cshtml"
    
            
            #line default
            #line hidden
            
            #line 69 "..\..\Views\Deal\ChangeDealClient.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {

            
            #line default
            #line hidden
WriteLiteral("    <table");

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

            
            #line 80 "..\..\Views\Deal\ChangeDealClient.cshtml"
                   Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                </td>\r\n            </tr>\r\n            <tr>\r\n             " +
"       <td>Клиент:</td>\r\n                    <td>\r\n                    <table");

WriteLiteral(" style=\"table-layout: fixed\"");

WriteLiteral(" cellspacing=\"0\"");

WriteLiteral(" cellpadding=\"0\"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(">\r\n                        <tbody>\r\n                            <tr>\r\n           " +
"                     <td>\r\n");

WriteLiteral("                                    ");

            
            #line 90 "..\..\Views\Deal\ChangeDealClient.cshtml"
                               Write(Html.LookupFor(m => m.Client, new LookupSettings { EntityName = EntityName.Client}));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                    ");

            
            #line 91 "..\..\Views\Deal\ChangeDealClient.cshtml"
                               Write(Html.ValidationMessageFor(m => m.Client, null, new Dictionary<string, object> { { "class", "error" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                            </tr>\r\n     " +
"                   </tbody>\r\n                    </table>\r\n                </td>" +
"\r\n            </tr>\r\n            <tr");

WriteLiteral(" style=\"display: none\"");

WriteLiteral(">\r\n                <td>\r\n");

WriteLiteral("                    ");

            
            #line 100 "..\..\Views\Deal\ChangeDealClient.cshtml"
               Write(Html.HiddenFor(m=>m.DealId));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </td>\r\n            </tr>\r\n        </tbody>\r\n    </table>\r\n");

            
            #line 105 "..\..\Views\Deal\ChangeDealClient.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
