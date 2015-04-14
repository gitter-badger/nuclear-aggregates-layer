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
    
    #line 1 "..\..\Views\CreateOrUpdate\Cyprus\Account.cshtml"
    using BLCore.UI.Web.Mvc.Utils;
    
    #line default
    #line hidden
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Cyprus/Account.cshtml")]
    public partial class Account : System.Web.Mvc.WebViewPage<Models.Cyprus.CyprusAccountViewModel>
    {
        public Account()
        {
        }
        public override void Execute()
        {
            
            #line 4 "..\..\Views\CreateOrUpdate\Cyprus\Account.cshtml"
  
    Layout = "../../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(">\r\n        div.label-wrapper\r\n        {\r\n            width: 180px !important;\r\n  " +
"      }\r\n    </style>           \r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n<div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 359), Tuple.Create("\"", 395)
            
            #line 20 "..\..\Views\CreateOrUpdate\Cyprus\Account.cshtml"
, Tuple.Create(Tuple.Create("", 367), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 367), false)
);

WriteLiteral(">\r\n");

WriteLiteral("    ");

            
            #line 21 "..\..\Views\CreateOrUpdate\Cyprus\Account.cshtml"
Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

            
            #line 23 "..\..\Views\CreateOrUpdate\Cyprus\Account.cshtml"
    
            
            #line default
            #line hidden
            
            #line 23 "..\..\Views\CreateOrUpdate\Cyprus\Account.cshtml"
      
        var readonlyFieldHtmlAttributes = new Dictionary<string, object> { { "class", "readonly inputfields" }, { "readonly", "readonly" } };
    
            
            #line default
            #line hidden
WriteLiteral("\r\n       \r\n    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 28 "..\..\Views\CreateOrUpdate\Cyprus\Account.cshtml"
   Write(Html.TemplateField(m => m.LegalPerson, FieldFlex.lone, new LookupSettings { EntityName = EntityName.LegalPerson, ReadOnly = !Model.IsNew && !string.IsNullOrEmpty(Model.LegalPerson.Value) }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 31 "..\..\Views\CreateOrUpdate\Cyprus\Account.cshtml"
   Write(Html.TemplateField(m => m.BranchOfficeOrganizationUnit, FieldFlex.lone, new LookupSettings { EntityName = EntityName.BranchOfficeOrganizationUnit, ReadOnly = !Model.IsNew && !string.IsNullOrEmpty(Model.BranchOfficeOrganizationUnit.Value) }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 34 "..\..\Views\CreateOrUpdate\Cyprus\Account.cshtml"
   Write(Html.TemplateField(m => m.AccountDetailBalance, FieldFlex.twins, readonlyFieldHtmlAttributes));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 35 "..\..\Views\CreateOrUpdate\Cyprus\Account.cshtml"
   Write(Html.TemplateField(m => m.Currency, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Currency, ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 38 "..\..\Views\CreateOrUpdate\Cyprus\Account.cshtml"
   Write(Html.TemplateField(m => m.LockDetailBalance, FieldFlex.twins, readonlyFieldHtmlAttributes));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 39 "..\..\Views\CreateOrUpdate\Cyprus\Account.cshtml"
   Write(Html.TemplateField(m => m.LegalPesonSyncCode1C, FieldFlex.twins, readonlyFieldHtmlAttributes));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n</div>\r\n<div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"AdministrationTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 1716), Tuple.Create("\"", 1759)
            
            #line 42 "..\..\Views\CreateOrUpdate\Cyprus\Account.cshtml"
, Tuple.Create(Tuple.Create("", 1724), Tuple.Create<System.Object, System.Int32>(BLResources.AdministrationTabTitle
            
            #line default
            #line hidden
, 1724), false)
);

WriteLiteral(">\r\n");

WriteLiteral("    ");

            
            #line 43 "..\..\Views\CreateOrUpdate\Cyprus\Account.cshtml"
Write(Html.SectionHead("adminHeader", BLResources.AdministrationTabTitle));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 44 "..\..\Views\CreateOrUpdate\Cyprus\Account.cshtml"
    
            
            #line default
            #line hidden
            
            #line 44 "..\..\Views\CreateOrUpdate\Cyprus\Account.cshtml"
     if (Model.IsCurated && Model.IsSecurityRoot)
    {

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 47 "..\..\Views\CreateOrUpdate\Cyprus\Account.cshtml"
   Write(Html.TemplateField(m=>m.Owner, FieldFlex.twins, new LookupSettings{EntityName = EntityName.User, Disabled = !Model.OwnerCanBeChanged, Plugins = new[] { "new Ext.ux.LookupFieldOwner()" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n");

            
            #line 49 "..\..\Views\CreateOrUpdate\Cyprus\Account.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("    \r\n    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 52 "..\..\Views\CreateOrUpdate\Cyprus\Account.cshtml"
   Write(Html.TemplateField(m=>m.CreatedBy, FieldFlex.twins, new LookupSettings{EntityName = EntityName.User, ReadOnly = true}));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 53 "..\..\Views\CreateOrUpdate\Cyprus\Account.cshtml"
   Write(Html.TemplateField(m => m.CreatedOn, FieldFlex.twins, new DateTimeSettings { ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 56 "..\..\Views\CreateOrUpdate\Cyprus\Account.cshtml"
   Write(Html.TemplateField(m => m.ModifiedBy, FieldFlex.twins, new LookupSettings { EntityName = EntityName.User, ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 57 "..\..\Views\CreateOrUpdate\Cyprus\Account.cshtml"
   Write(Html.TemplateField(m => m.ModifiedOn, FieldFlex.twins, new DateTimeSettings { ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n</div>   \r\n");

});

        }
    }
}
#pragma warning restore 1591
