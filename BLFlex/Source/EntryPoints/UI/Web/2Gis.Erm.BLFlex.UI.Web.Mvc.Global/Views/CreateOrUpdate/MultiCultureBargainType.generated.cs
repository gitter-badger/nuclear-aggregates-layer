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

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Views.CreateOrUpdate
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/MultiCultureBargainType.cshtml")]
    public partial class MultiCultureBargainType : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.MultiCultureBargainTypeViewModel>
    {
        public MultiCultureBargainType()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\CreateOrUpdate\MultiCultureBargainType.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 13 "..\..\Views\CreateOrUpdate\MultiCultureBargainType.cshtml"
Write(Html.HiddenFor(m => m.IdentityServiceUrl));

            
            #line default
            #line hidden
WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 288), Tuple.Create("\"", 324)
            
            #line 14 "..\..\Views\CreateOrUpdate\MultiCultureBargainType.cshtml"
, Tuple.Create(Tuple.Create("", 296), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 296), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 16 "..\..\Views\CreateOrUpdate\MultiCultureBargainType.cshtml"
       Write(Html.EditableId(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 19 "..\..\Views\CreateOrUpdate\MultiCultureBargainType.cshtml"
       Write(Html.TemplateField(m => m.Name, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 20 "..\..\Views\CreateOrUpdate\MultiCultureBargainType.cshtml"
       Write(Html.TemplateField(m=> m.VatRate, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

WriteLiteral("\r\n");

            
            #line 25 "..\..\Views\CreateOrUpdate\MultiCultureBargainType.cshtml"
Write(RenderBody());

            
            #line default
            #line hidden
WriteLiteral("\r\n");

        }
    }
}
#pragma warning restore 1591
