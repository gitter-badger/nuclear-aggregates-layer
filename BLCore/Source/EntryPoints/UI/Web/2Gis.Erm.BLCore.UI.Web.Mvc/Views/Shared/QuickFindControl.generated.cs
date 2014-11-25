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

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Views.Shared
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
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
    using DoubleGis.Erm.Platform.Common;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.Model.Entities.Enums;
    using DoubleGis.Erm.Platform.Model.Metadata.Enums;
    using DoubleGis.Erm.Platform.UI.Web.Mvc;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Security;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Shared/QuickFindControl.cshtml")]
    public partial class QuickFindControl : System.Web.Mvc.WebViewPage<ViewModels.QuickFindSettings>
    {
        public QuickFindControl()
        {
        }
        public override void Execute()
        {
WriteLiteral("<script");

WriteAttribute("src", Tuple.Create(" src=\"", 46), Tuple.Create("\"", 106)
, Tuple.Create(Tuple.Create("", 52), Tuple.Create("/Scripts/DoubleGis.UI.QuickFind.js?", 52), true)
            
            #line 3 "..\..\Views\Shared\QuickFindControl.cshtml"
, Tuple.Create(Tuple.Create("", 87), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 87), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n<script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\nExt.onReady(function () \r\n    {\r\n        var q = new Ext.DoubleGis.UI.QuickFin" +
"d(\"");

            
            #line 8 "..\..\Views\Shared\QuickFindControl.cshtml"
                                           Write(Model.Name);

            
            #line default
            #line hidden
WriteLiteral("\");\r\n        q.on(\"change\", ");

            
            #line 9 "..\..\Views\Shared\QuickFindControl.cshtml"
                  Write(Model.OnSearchFunction);

            
            #line default
            #line hidden
WriteLiteral(");\r\n        var filter = Ext.urlDecode(location.search.substring(1)).search;\r\n   " +
"     q.setValue(filter?decodeURIComponent(filter):\"\");\r\n    }\r\n);\r\n</script>\r\n<t" +
"able");

WriteLiteral(" cellpadding=\"0\"");

WriteLiteral(" cellspacing=\"0\"");

WriteLiteral(">\r\n    <colgroup>\r\n        <col");

WriteLiteral(" width=\"100%\"");

WriteLiteral(">\r\n        <col");

WriteLiteral(" width=\"21px\"");

WriteLiteral(">\r\n    </colgroup>\r\n    <tr>\r\n        <td>\r\n            <input");

WriteLiteral(" type=\"text\"");

WriteAttribute("id", Tuple.Create(" id=\"", 663), Tuple.Create("\"", 686)
            
            #line 22 "..\..\Views\Shared\QuickFindControl.cshtml"
, Tuple.Create(Tuple.Create("", 668), Tuple.Create<System.Object, System.Int32>(Model.Name
            
            #line default
            #line hidden
, 668), false)
, Tuple.Create(Tuple.Create("", 681), Tuple.Create("_Text", 681), true)
);

WriteLiteral(" class=\"QuickFind\"");

WriteLiteral(" style=\"width: 100%\"");

WriteLiteral(" />\r\n        </td>\r\n        <td>\r\n            <img");

WriteAttribute("id", Tuple.Create(" id=\"", 775), Tuple.Create("\"", 800)
            
            #line 25 "..\..\Views\Shared\QuickFindControl.cshtml"
, Tuple.Create(Tuple.Create("", 780), Tuple.Create<System.Object, System.Int32>(Model.Name
            
            #line default
            #line hidden
, 780), false)
, Tuple.Create(Tuple.Create("", 793), Tuple.Create("_Button", 793), true)
);

WriteLiteral(" class=\"QuickFind\"");

WriteAttribute("alt", Tuple.Create(" alt=\"", 819), Tuple.Create("\"", 842)
            
            #line 25 "..\..\Views\Shared\QuickFindControl.cshtml"
, Tuple.Create(Tuple.Create("", 825), Tuple.Create<System.Object, System.Int32>(BLResources.Find
            
            #line default
            #line hidden
, 825), false)
);

WriteLiteral(" />\r\n        </td>\r\n    </tr>\r\n</table>\r\n");

        }
    }
}
#pragma warning restore 1591
