﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Views.CreateOrUpdate
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc.Html;
#line 1 "..\..\Views\CreateOrUpdate\MultiCultureBargain.cshtml"
    using BLCore.UI.Web.Mvc.Utils;
    
    #line default
    #line hidden
    using DoubleGis.Erm.BLCore.Resources.Server.Properties;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/MultiCultureBargain.cshtml")]
    public partial class MultiCultureBargain : System.Web.Mvc.WebViewPage<Models.MultiCultureBargainViewModel>
    {
        public MultiCultureBargain()
        {
        }
        public override void Execute()
        {
            
            #line 4 "..\..\Views\CreateOrUpdate\MultiCultureBargain.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n<script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n    window.InitPage = function ()\r\n    {\r\n        window.Card.on(\'beforepost\'," +
" function () { window.returnValue = true; });\r\n    }\r\n</script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n<div");

WriteLiteral(" style=\"display: none\"");

WriteLiteral(">\r\n");

WriteLiteral("    ");

            
            #line 21 "..\..\Views\CreateOrUpdate\MultiCultureBargain.cshtml"
Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n</div>\r\n<div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 459), Tuple.Create("\"", 495)
            
            #line 23 "..\..\Views\CreateOrUpdate\MultiCultureBargain.cshtml"
, Tuple.Create(Tuple.Create("", 467), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 467), false)
);

WriteLiteral(">\r\n    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 25 "..\..\Views\CreateOrUpdate\MultiCultureBargain.cshtml"
   Write(Html.TemplateField(m => m.Number, FieldFlex.twins, new Dictionary<string, object> {{"readonly", "readonly"}, {"class", "readonly"}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 26 "..\..\Views\CreateOrUpdate\MultiCultureBargain.cshtml"
   Write(Html.TemplateField(m => m.BargainType, FieldFlex.twins, new LookupSettings { EntityName = EntityName.BargainType, ShowReadOnlyCard = true, ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 29 "..\..\Views\CreateOrUpdate\MultiCultureBargain.cshtml"
   Write(Html.TemplateField(m => m.SignedOn, FieldFlex.twins, new DateTimeSettings {ReadOnly = true}));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 30 "..\..\Views\CreateOrUpdate\MultiCultureBargain.cshtml"
   Write(Html.TemplateField(m => m.ClosedOn, FieldFlex.twins, new DateTimeSettings {ReadOnly = true}));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 33 "..\..\Views\CreateOrUpdate\MultiCultureBargain.cshtml"
   Write(Html.TemplateField(m => m.LegalPerson, FieldFlex.lone, new LookupSettings {EntityName = EntityName.LegalPerson, ShowReadOnlyCard = true, ReadOnly = true}));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 36 "..\..\Views\CreateOrUpdate\MultiCultureBargain.cshtml"
   Write(Html.TemplateField(m => m.BranchOfficeOrganizationUnit, FieldFlex.lone, new LookupSettings { EntityName = EntityName.BranchOfficeOrganizationUnit, ShowReadOnlyCard = true, ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 39 "..\..\Views\CreateOrUpdate\MultiCultureBargain.cshtml"
   Write(Html.TemplateField(m => m.Comment, FieldFlex.lone, new Dictionary<string, object> {{"rows", "4"}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n</div>\r\n");

});

WriteLiteral("\r\n");

            
            #line 44 "..\..\Views\CreateOrUpdate\MultiCultureBargain.cshtml"
Write(RenderBody());

            
            #line default
            #line hidden
WriteLiteral("\r\n");

        }
    }
}
#pragma warning restore 1591
