﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34011
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Views.Shared.EditorTemplates
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Shared/EditorTemplates/FormattedTextViewModel.cshtml")]
    public partial class _FormattedTextViewModel : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BL.UI.Web.Mvc.Models.AdvertisementElementModels.FormattedTextViewModel>
    {
        public _FormattedTextViewModel()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\Shared\EditorTemplates\FormattedTextViewModel.cshtml"
Write(Html.HiddenFor(m => m.TemplateTextLengthRestriction));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 4 "..\..\Views\Shared\EditorTemplates\FormattedTextViewModel.cshtml"
Write(Html.HiddenFor(m => m.TemplateMaxSymbolsInWord));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 5 "..\..\Views\Shared\EditorTemplates\FormattedTextViewModel.cshtml"
Write(Html.HiddenFor(m => m.TemplateTextLineBreaksRestriction));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

            
            #line 7 "..\..\Views\Shared\EditorTemplates\FormattedTextViewModel.cshtml"
Write(Html.HiddenFor(m => m.PlainText));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 8 "..\..\Views\Shared\EditorTemplates\FormattedTextViewModel.cshtml"
Write(Html.HiddenFor(m => m.FormattedText));

            
            #line default
            #line hidden
WriteLiteral("\r\n<div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n    <div");

WriteLiteral(" class=\"display-wrapper field-wrapper lone\"");

WriteLiteral(" id=\"Text-wrapper\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"label-wrapper\"");

WriteLiteral(">");

            
            #line 11 "..\..\Views\Shared\EditorTemplates\FormattedTextViewModel.cshtml"
                              Write(Html.LabelFor(m => m.FormattedText));

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n        <div");

WriteLiteral(" class=\"input-wrapper\"");

WriteLiteral(">\r\n                \r\n            ");

WriteLiteral("\r\n            <div");

WriteLiteral(" id=\"TxtContainer\"");

WriteLiteral(" style=\"width: 600px; padding-top: 5px\"");

WriteLiteral("></div>\r\n\r\n");

WriteLiteral("            ");

            
            #line 17 "..\..\Views\Shared\EditorTemplates\FormattedTextViewModel.cshtml"
       Write(Html.ValidationMessageFor(m => m.FormattedText, null, new Dictionary<string, object> { { "class", "error" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n            ");

WriteLiteral("\r\n            ");

WriteLiteral("\r\n        </div>\r\n    </div>\r\n</div>\r\n");

        }
    }
}
#pragma warning restore 1591
