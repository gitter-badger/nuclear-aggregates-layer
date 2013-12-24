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

namespace DoubleGis.Erm.BLFlex.Web.Mvc.Global.Views.CreateOrUpdate
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
    using DoubleGis.Erm.BL.Resources.Server.Properties;
    using DoubleGis.Erm.Core;
    using DoubleGis.Erm.Core.Enums;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.UI.Web.Mvc;
    using DoubleGis.Erm.UI.Web.Mvc.Controllers;
    using DoubleGis.Erm.UI.Web.Mvc.Models;
    
    #line 1 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
    using DoubleGis.Erm.UI.Web.Mvc.Utils;
    
    #line default
    #line hidden
    
    #line 2 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
    using Platform.Web.Mvc.Utils;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/CzechBranchOfficeOrganizationUnit.cshtml")]
    public partial class CzechBranchOfficeOrganizationUnit : System.Web.Mvc.WebViewPage<CzechBranchOfficeOrganizationUnitViewModel>
    {
        public CzechBranchOfficeOrganizationUnit()
        {
        }
        public override void Execute()
        {
            
            #line 5 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 11 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
Write(Html.HiddenFor(m => m.IdentityServiceUrl));

            
            #line default
            #line hidden
WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">
        window.InitPage = function ()
        {
            this.on(""beforebuild"", function (cardObject)
            {
                Ext.apply(this,
                {
                    SetAsPrimary: function ()
                    {
                        var params = ""dialogHeight:210px; dialogWidth:500px; status:yes; scroll:no; resizable:no; "";
                        var sUrl = ""/BranchOfficeOrganizationUnit/SetAsPrimary"";
                        window.showModalDialog(sUrl, [Ext.getDom(""Id"").value], params);
                        this.refresh(true);
                    },
                    SetAsPrimaryForRegSales: function ()
                    {
                        var params = ""dialogHeight:210px; dialogWidth:500px; status:yes; scroll:no; resizable:no; "";
                        var sUrl = ""/BranchOfficeOrganizationUnit/SetAsPrimaryForRegSales"";
                        window.showModalDialog(sUrl, [Ext.getDom(""Id"").value], params);
                        this.refresh(true);
                    }
                });
            });
        };
        
    </script>
    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(">\r\n        DIV.label-wrapper\r\n        {\r\n            width: 150px !important;\r\n  " +
"      }\r\n    </style>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 48 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
Write(Html.HiddenFor(m => m.BranchOfficeAddlId));

            
            #line default
            #line hidden
WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 1653), Tuple.Create("\"", 1689)
            
            #line 49 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
, Tuple.Create(Tuple.Create("", 1661), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 1661), false)
);

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 50 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
   Write(Html.SectionHead("ouinfo", BLResources.BranchOfficeOrgUnitInfo));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 52 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
       Write(Html.EditableId(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 55 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.ShortLegalName, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 58 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.BranchOffice, FieldFlex.lone, new LookupSettings { EntityName = EntityName.BranchOffice}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 61 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.OrganizationUnit, FieldFlex.twins, new LookupSettings { EntityName = EntityName.OrganizationUnit}));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 62 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.PhoneNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 65 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.ChiefNameInNominative, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 66 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.ChiefNameInGenitive, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 69 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.PositionInNominative, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 70 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.PositionInGenitive, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 73 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.Registered, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 74 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.RegistrationCertificate, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 77 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.ActualAddress, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 78 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.Email, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 81 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.PostalAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 84 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.PaymentEssentialElements, FieldFlex.lone, new Dictionary<string, object> { { "rows", "5" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 87 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.IsPrimary, FieldFlex.twins, new Dictionary<string, object>{{"disabled", "disabled"}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 88 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
       Write(Html.TemplateField(m => m.IsPrimaryForRegionalSales, FieldFlex.twins, new Dictionary<string, object> { { "disabled", "disabled" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(" id=\"boinfo\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 91 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
       Write(Html.SectionHead("lpinfo", BLResources.BranchOfficeInfo));

            
            #line default
            #line hidden
WriteLiteral("\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 93 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
           Write(Html.TemplateField(m => m.BranchOfficeAddlName, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 96 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
           Write(Html.TemplateField(m => m.BranchOfficeAddlIc, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                ");

            
            #line 97 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
           Write(Html.TemplateField(m => m.BranchOfficeAddlInn, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 100 "..\..\Views\CreateOrUpdate\CzechBranchOfficeOrganizationUnit.cshtml"
           Write(Html.TemplateField(m => m.BranchOfficeAddlLegalAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
