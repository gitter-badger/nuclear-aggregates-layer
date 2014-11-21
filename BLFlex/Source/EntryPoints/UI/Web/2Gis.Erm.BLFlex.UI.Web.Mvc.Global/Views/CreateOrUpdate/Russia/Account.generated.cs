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

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Views.CreateOrUpdate.Russia
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc.Html;

    using DoubleGis.Erm.BLCore.Resources.Server.Properties;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Russia/Account.cshtml")]
    public partial class Account : System.Web.Mvc.WebViewPage<Models.Russia.AccountViewModel>
    {
        public Account()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\CreateOrUpdate\Russia\Account.cshtml"
  
    Layout = "../../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(">\r\n        div.label-wrapper\r\n        {\r\n            width: 180px !important;\r\n  " +
"      }\r\n    </style>\r\n    \r\n    \r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">
        window.InitPage = function () {

            window.Card.on(""afterbuild"", function (card) {
                if (window.Ext.getDom(""ViewConfig_Id"").value && window.Ext.getDom(""ViewConfig_Id"").value != ""0"") {
                    this.Items.TabPanel.add(
                        {
                            xtype: ""actionshistorytab"",
                            pCardInfo:
                            {
                                pTypeId: this.Settings.EntityId,
                                pId: window.Ext.getDom(""ViewConfig_Id"").value,
                                pTypeName: Ext.get(""ViewConfig_EntityName"").dom.value
                            }
                        });
                }
            });
        };
    </script>

");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n<div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 1137), Tuple.Create("\"", 1173)
            
            #line 41 "..\..\Views\CreateOrUpdate\Russia\Account.cshtml"
, Tuple.Create(Tuple.Create("", 1145), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 1145), false)
);

WriteLiteral(">\r\n");

WriteLiteral("    ");

            
            #line 42 "..\..\Views\CreateOrUpdate\Russia\Account.cshtml"
Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

            
            #line 44 "..\..\Views\CreateOrUpdate\Russia\Account.cshtml"
    
            
            #line default
            #line hidden
            
            #line 44 "..\..\Views\CreateOrUpdate\Russia\Account.cshtml"
      
        var readonlyFieldHtmlAttributes = new Dictionary<string, object> { { "class", "readonly inputfields" }, { "readonly", "readonly" } };
    
            
            #line default
            #line hidden
WriteLiteral("\r\n       \r\n    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 49 "..\..\Views\CreateOrUpdate\Russia\Account.cshtml"
   Write(Html.TemplateField(m => m.LegalPerson, FieldFlex.lone, new LookupSettings { EntityName = EntityName.LegalPerson, ReadOnly = !Model.IsNew && !string.IsNullOrEmpty(Model.LegalPerson.Value) }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 52 "..\..\Views\CreateOrUpdate\Russia\Account.cshtml"
   Write(Html.TemplateField(m => m.BranchOfficeOrganizationUnit, FieldFlex.lone, new LookupSettings { EntityName = EntityName.BranchOfficeOrganizationUnit, ReadOnly = !Model.IsNew && !string.IsNullOrEmpty(Model.BranchOfficeOrganizationUnit.Value) }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 55 "..\..\Views\CreateOrUpdate\Russia\Account.cshtml"
   Write(Html.TemplateField(m => m.AccountDetailBalance, FieldFlex.twins, readonlyFieldHtmlAttributes));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 56 "..\..\Views\CreateOrUpdate\Russia\Account.cshtml"
   Write(Html.TemplateField(m => m.Currency, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Currency, ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 59 "..\..\Views\CreateOrUpdate\Russia\Account.cshtml"
   Write(Html.TemplateField(m => m.LockDetailBalance, FieldFlex.twins, readonlyFieldHtmlAttributes));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 60 "..\..\Views\CreateOrUpdate\Russia\Account.cshtml"
   Write(Html.TemplateField(m => m.LegalPesonSyncCode1C, FieldFlex.twins, readonlyFieldHtmlAttributes));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n</div>\r\n<div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"AdministrationTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 2494), Tuple.Create("\"", 2537)
            
            #line 63 "..\..\Views\CreateOrUpdate\Russia\Account.cshtml"
, Tuple.Create(Tuple.Create("", 2502), Tuple.Create<System.Object, System.Int32>(BLResources.AdministrationTabTitle
            
            #line default
            #line hidden
, 2502), false)
);

WriteLiteral(">\r\n");

WriteLiteral("    ");

            
            #line 64 "..\..\Views\CreateOrUpdate\Russia\Account.cshtml"
Write(Html.SectionHead("adminHeader", BLResources.AdministrationTabTitle));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 65 "..\..\Views\CreateOrUpdate\Russia\Account.cshtml"
    
            
            #line default
            #line hidden
            
            #line 65 "..\..\Views\CreateOrUpdate\Russia\Account.cshtml"
     if (Model.IsCurated && Model.IsSecurityRoot)
    {

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 68 "..\..\Views\CreateOrUpdate\Russia\Account.cshtml"
   Write(Html.TemplateField(m=>m.Owner, FieldFlex.twins, new LookupSettings{EntityName = EntityName.User, Disabled = !Model.OwnerCanBeChanged, Plugins = new[] { "new Ext.ux.LookupFieldOwner()" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n");

            
            #line 70 "..\..\Views\CreateOrUpdate\Russia\Account.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("    \r\n    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 73 "..\..\Views\CreateOrUpdate\Russia\Account.cshtml"
   Write(Html.TemplateField(m=>m.CreatedBy, FieldFlex.twins, new LookupSettings{EntityName = EntityName.User, ReadOnly = true}));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 74 "..\..\Views\CreateOrUpdate\Russia\Account.cshtml"
   Write(Html.TemplateField(m => m.CreatedOn, FieldFlex.twins, new DateTimeSettings { ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 77 "..\..\Views\CreateOrUpdate\Russia\Account.cshtml"
   Write(Html.TemplateField(m => m.ModifiedBy, FieldFlex.twins, new LookupSettings { EntityName = EntityName.User, ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 78 "..\..\Views\CreateOrUpdate\Russia\Account.cshtml"
   Write(Html.TemplateField(m => m.ModifiedOn, FieldFlex.twins, new DateTimeSettings { ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n</div>   \r\n");

});

        }
    }
}
#pragma warning restore 1591
