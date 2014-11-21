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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Limit/PrintLimits.cshtml")]
    public partial class PrintLimits : System.Web.Mvc.WebViewPage<Models.PrintLimitsViewModel>
    {
        public PrintLimits()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\Limit\PrintLimits.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 7 "..\..\Views\Limit\PrintLimits.cshtml"
            Write(BLResources.PrintLimits);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 8 "..\..\Views\Limit\PrintLimits.cshtml"
                  Write(BLResources.PrintLimits);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

});

WriteLiteral("\r\n");

DefineSection("PageContent", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">
        Ext.onReady(function ()
        {
            //Show error messages
            if (Ext.getDom(""Notifications"").innerHTML.trim() != """")
            {
                Ext.getDom(""Notifications"").style.display = ""block"";

                Ext.get(""Message"").hide();
                Ext.get(""OK"").disable();
            }

            //write eventhandlers for buttons
            Ext.get(""Cancel"").on(""click"", function () { window.close(); });
            Ext.get(""OK"").on(""click"", function ()
            {
                EntityForm.submit();
            });
        });
    </script>
");

            
            #line 33 "..\..\Views\Limit\PrintLimits.cshtml"
    
            
            #line default
            #line hidden
            
            #line 33 "..\..\Views\Limit\PrintLimits.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {

            
            #line default
            #line hidden
WriteLiteral("    <table");

WriteLiteral(" cellspacing=\"5\"");

WriteLiteral(" cellpadding=\"5\"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(" height=\"100%\"");

WriteLiteral(">\r\n        <tbody>\r\n            <tr>\r\n                <td>\r\n                    <" +
"div");

WriteLiteral(" style=\"display: none; height: 15px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(" class=\"Notifications\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 40 "..\..\Views\Limit\PrintLimits.cshtml"
                   Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n                </td>\r\n            </tr>\r\n         " +
"   <tr>\r\n                <td>\r\n                    &nbsp;\r\n                </td>" +
"\r\n            </tr>\r\n            <tr>\r\n                <td>\r\n                   " +
" <label");

WriteLiteral(" style=\"font-weight: bold\"");

WriteLiteral(" id=\"Message\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 52 "..\..\Views\Limit\PrintLimits.cshtml"
                   Write(string.Format(BLResources.NumberOfLimitsWillBePrinted, Model.LimitCount));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </label>\r\n                </td>\r\n            </tr>\r\n       " +
"     <tr");

WriteLiteral(" style=\"display: none\"");

WriteLiteral(">\r\n                <td>\r\n");

WriteLiteral("                    ");

            
            #line 58 "..\..\Views\Limit\PrintLimits.cshtml"
               Write(Html.HiddenFor(m => m.LimitIds));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </td>\r\n            </tr>\r\n        </tbody>\r\n    </table>\r\n");

            
            #line 63 "..\..\Views\Limit\PrintLimits.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
