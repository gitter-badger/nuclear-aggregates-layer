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
    using System.Web.Mvc.Html;

    using DoubleGis.Erm.BLCore.Resources.Server.Properties;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    using NuClear.Model.Common.Entities;

    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/MultiCultureFirm.cshtml")]
    public partial class MultiCultureFirm : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.MultiCultureFirmViewModel>
    {
        public MultiCultureFirm()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 174), Tuple.Create("\"", 233)
, Tuple.Create(Tuple.Create("", 180), Tuple.Create("/Scripts/Ext.DoubleGis.UI.Firm.js?", 180), true)
            
            #line 9 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
, Tuple.Create(Tuple.Create("", 214), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 214), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 14 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n    ");

WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 17 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
Write(Html.HiddenFor(m=>m.ClientName));

            
            #line default
            #line hidden
WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 451), Tuple.Create("\"", 487)
            
            #line 18 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
, Tuple.Create(Tuple.Create("", 459), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 459), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 20 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
       Write(Html.TemplateField(m => m.Name, FieldFlex.lone, new Dictionary<string, object> { { "readonly", "readonly" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 23 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
       Write(Html.TemplateField(m => m.Client, FieldFlex.lone, new LookupSettings { EntityName = EntityType.Instance.Client(), ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 26 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
       Write(Html.TemplateField(m => m.Comment, FieldFlex.lone, new Dictionary<string, object>{{"rows", "5"}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n");

WriteLiteral("        ");

            
            #line 28 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
   Write(Html.SectionHead("SectionHead2", BLResources.AdditionalTabTitle));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 30 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
       Write(Html.TemplateField(m => m.OrganizationUnit, FieldFlex.twins, new LookupSettings { EntityName = EntityType.Instance.OrganizationUnit(), Disabled = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 31 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
       Write(Html.TemplateField(m => m.LastQualifyTime, FieldFlex.twins, new DateTimeSettings { Disabled = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 34 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
       Write(Html.TemplateField(m => m.LastDisqualifyTime, FieldFlex.twins, new DateTimeSettings { Disabled = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 37 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
       Write(Html.TemplateField(m => m.ClosedForAscertainment, FieldFlex.twins, new Dictionary<string, object> { { "disabled", "disabled" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"AdministrationTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 1858), Tuple.Create("\"", 1901)
            
            #line 40 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
, Tuple.Create(Tuple.Create("", 1866), Tuple.Create<System.Object, System.Int32>(BLResources.AdministrationTabTitle
            
            #line default
            #line hidden
, 1866), false)
);

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 41 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
   Write(Html.SectionHead("adminHeader", BLResources.AdministrationTabTitle));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 42 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
        
            
            #line default
            #line hidden
            
            #line 42 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
         if (Model.IsCurated && Model.IsSecurityRoot)
        {

            
            #line default
            #line hidden
WriteLiteral("        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 45 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
       Write(Html.TemplateField(m => m.Owner, FieldFlex.twins, new LookupSettings { EntityName = EntityType.Instance.User(), Plugins = new[] { "new Ext.ux.LookupFieldOwner()" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 46 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
       Write(Html.TemplateField(m => m.Territory, FieldFlex.twins, new LookupSettings { EntityName = EntityType.Instance.Territory(), ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n");

            
            #line 48 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 50 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
       Write(Html.TemplateField(m => m.CreatedBy, FieldFlex.twins, new LookupSettings { EntityName = EntityType.Instance.User(), Disabled = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 51 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
       Write(Html.TemplateField(m => m.CreatedOn, FieldFlex.twins, new DateTimeSettings { Disabled = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 54 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
       Write(Html.TemplateField(m => m.ModifiedBy, FieldFlex.twins, new LookupSettings { EntityName = EntityType.Instance.User(), Disabled = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 55 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
       Write(Html.TemplateField(m => m.ModifiedOn, FieldFlex.twins, new DateTimeSettings { Disabled = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

WriteLiteral("\r\n");

            
            #line 60 "..\..\Views\CreateOrUpdate\MultiCultureFirm.cshtml"
Write(RenderBody());

            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591
