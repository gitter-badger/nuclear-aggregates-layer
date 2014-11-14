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

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Views.Deal
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Deal/PickCreateReason.cshtml")]
    public partial class PickCreateReason : System.Web.Mvc.WebViewPage<Models.DealCreateReasonViewModel>
    {
        public PickCreateReason()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 4 "..\..\Views\Deal\PickCreateReason.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 8 "..\..\Views\Deal\PickCreateReason.cshtml"
            Write(BLResources.CreateNewDeal);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 9 "..\..\Views\Deal\PickCreateReason.cshtml"
                  Write(BLResources.PickReasonForNewDeal);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 10 "..\..\Views\Deal\PickCreateReason.cshtml"
                    Write(BLResources.PickReasonForNewDeal);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

WriteLiteral("\r\n");

DefineSection("PageContent", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">
        Ext.onReady(
    function ()
    {
        Ext.get(""Cancel"").on(""click"", function () { window.returnValue = null; window.close(); });
        Ext.get(""OK"").on(""click"", function ()
        {
            var reason = Ext.getDom(""ReasonForNewDeal"");
            if (reason.value == """")
            {
                alert(""");

            
            #line 24 "..\..\Views\Deal\PickCreateReason.cshtml"
                  Write(BLResources.PickReasonForNewDeal);

            
            #line default
            #line hidden
WriteLiteral("\");\r\n                reason.focus();\r\n                return;\r\n            }\r\n   " +
"         window.returnValue = { ReasonForNewDeal: reason.value };\r\n            w" +
"indow.close();\r\n        });\r\n    });\r\n\r\n    </script>\r\n    <table");

WriteLiteral(" cellspacing=\"5\"");

WriteLiteral(" cellpadding=\"5\"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(" height=\"80px\"");

WriteLiteral(">\r\n        <colgroup>\r\n            <col");

WriteLiteral(" width=\"40px\"");

WriteLiteral(" />\r\n            <col");

WriteLiteral(" width=\"10px\"");

WriteLiteral(" />\r\n            <col");

WriteLiteral(" width=\"300px\"");

WriteLiteral("/>\r\n        </colgroup>\r\n        <tbody>\r\n            <tr>\r\n                <td>\r" +
"\n");

WriteLiteral("                    ");

            
            #line 43 "..\..\Views\Deal\PickCreateReason.cshtml"
               Write(Html.LabelFor(m => m.ReasonForNewDeal));

            
            #line default
            #line hidden
WriteLiteral(":\r\n                </td>\r\n                <td></td>\r\n                <td>\r\n");

WriteLiteral("                    ");

            
            #line 47 "..\..\Views\Deal\PickCreateReason.cshtml"
               Write(Html.DropDownListFor(m => m.ReasonForNewDeal, EnumResources.ResourceManager, new Dictionary<string, object> { { "class", "inputfields" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </td>\r\n            </tr>\r\n        </tbody>\r\n    </table>\r\n");

});

        }
    }
}
#pragma warning restore 1591
