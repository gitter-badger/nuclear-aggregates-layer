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

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Views.CreateOrUpdate.Cyprus
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc.Html;
#line 1 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
    using BLCore.UI.Web.Mvc.Utils;
    
    #line default
    #line hidden
    using DoubleGis.Erm.BLCore.Resources.Server.Properties;
    using DoubleGis.Erm.Platform.Common;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Cyprus/BranchOfficeOrganizationUnit.cshtml")]
    public partial class BranchOfficeOrganizationUnit : System.Web.Mvc.WebViewPage<Models.Cyprus.CyprusBranchOfficeOrganizationUnitViewModel>
    {
        public BranchOfficeOrganizationUnit()
        {
        }
        public override void Execute()
        {
            
            #line 4 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
  
    Layout = "../../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 194), Tuple.Create("\"", 292)
, Tuple.Create(Tuple.Create("", 200), Tuple.Create("/Scripts/Ext.DoubleGis.UI.BranchOfficeOrganizationUnit.js?", 200), true)
            
            #line 10 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
, Tuple.Create(Tuple.Create("", 258), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 258), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(">\r\n        DIV.label-wrapper\r\n        {\r\n            width: 150px !important;\r\n  " +
"      }\r\n    </style>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 21 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
Write(Html.HiddenFor(m => m.IdentityServiceUrl));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 22 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
Write(Html.HiddenFor(m => m.BranchOfficeAddlId));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 616), Tuple.Create("\"", 652)
            
            #line 24 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
, Tuple.Create(Tuple.Create("", 624), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 624), false)
);

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 25 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
   Write(Html.SectionHead("ouinfo", BLResources.BranchOfficeOrgUnitInfo));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 27 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.EditableId(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 30 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.ShortLegalName, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 33 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.BranchOffice, FieldFlex.lone, new LookupSettings { EntityName = EntityName.BranchOffice}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 36 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.OrganizationUnit, FieldFlex.twins, new LookupSettings { EntityName = EntityName.OrganizationUnit}));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 37 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.PhoneNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 40 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.ChiefNameInNominative, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 41 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.PositionInNominative, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 44 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.OperatesOnTheBasisInGenitive, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 45 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.RegistrationCertificate, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 48 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.SyncCode1C, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 51 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.ActualAddress, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 52 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.Email, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 55 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.PostalAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 58 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.PaymentEssentialElements, FieldFlex.lone, new Dictionary<string, object> { { "rows", "5" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 61 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.IsPrimary, FieldFlex.twins, new Dictionary<string, object>{{"disabled", "disabled"}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 62 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.IsPrimaryForRegionalSales, FieldFlex.twins, new Dictionary<string, object> { { "disabled", "disabled" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(" id=\"boinfo\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 65 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.SectionHead("lpinfo", BLResources.BranchOfficeInfo));

            
            #line default
            #line hidden
WriteLiteral("\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 67 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
           Write(Html.TemplateField(m => m.BranchOfficeAddlName, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                ");

            
            #line 68 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
           Write(Html.TemplateField(m => m.BranchOfficeAddlInn, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 71 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
           Write(Html.TemplateField(m => m.BranchOfficeAddlLegalAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
