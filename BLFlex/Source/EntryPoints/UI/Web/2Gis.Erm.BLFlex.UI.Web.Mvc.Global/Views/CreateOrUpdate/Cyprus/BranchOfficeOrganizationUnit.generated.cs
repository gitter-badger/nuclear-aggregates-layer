﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Views.CreateOrUpdate.Cyprus
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Cyprus/BranchOfficeOrganizationUnit.cshtml")]
    public partial class BranchOfficeOrganizationUnit : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Cyprus.CyprusBranchOfficeOrganizationUnitViewModel>
    {
        public BranchOfficeOrganizationUnit()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
  
    Layout = "../../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 201), Tuple.Create("\"", 284)
, Tuple.Create(Tuple.Create("", 207), Tuple.Create("/Scripts/Ext.DoubleGis.UI.BranchOfficeOrganizationUnit.js?", 207), true)
            
            #line 9 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
, Tuple.Create(Tuple.Create("", 265), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 265), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(">\r\n        DIV.label-wrapper\r\n        {\r\n            width: 150px !important;\r\n  " +
"      }\r\n    </style>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n    \r\n");

WriteLiteral("    ");

            
            #line 21 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
Write(Html.HiddenFor(m => m.BranchOfficeAddlId));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 566), Tuple.Create("\"", 602)
            
            #line 23 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
, Tuple.Create(Tuple.Create("", 574), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 574), false)
);

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 24 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
   Write(Html.SectionHead("ouinfo", BLResources.BranchOfficeOrgUnitInfo));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 26 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.EditableId(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 29 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.ShortLegalName, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 32 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.ApplicationCityName, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 35 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.BranchOffice, FieldFlex.lone, new LookupSettings { EntityName = EntityName.BranchOffice}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 38 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.OrganizationUnit, FieldFlex.twins, new LookupSettings { EntityName = EntityName.OrganizationUnit}));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 39 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.PhoneNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 42 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.ChiefNameInNominative, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 43 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.PositionInNominative, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 46 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.OperatesOnTheBasisInGenitive, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 47 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.RegistrationCertificate, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 50 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.SyncCode1C, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 53 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.ActualAddress, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 54 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.Email, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 57 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.PostalAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 60 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.PaymentEssentialElements, FieldFlex.lone, new Dictionary<string, object> { { "rows", "5" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 63 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.IsPrimary, FieldFlex.twins, new Dictionary<string, object>{{"disabled", "disabled"}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 64 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.IsPrimaryForRegionalSales, FieldFlex.twins, new Dictionary<string, object> { { "disabled", "disabled" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(" id=\"boinfo\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 67 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
       Write(Html.SectionHead("lpinfo", BLResources.BranchOfficeInfo));

            
            #line default
            #line hidden
WriteLiteral("\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 69 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
           Write(Html.TemplateField(m => m.BranchOfficeAddlName, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                ");

            
            #line 70 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
           Write(Html.TemplateField(m => m.BranchOfficeAddlTic, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 73 "..\..\Views\CreateOrUpdate\Cyprus\BranchOfficeOrganizationUnit.cshtml"
           Write(Html.TemplateField(m => m.BranchOfficeAddlLegalAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
