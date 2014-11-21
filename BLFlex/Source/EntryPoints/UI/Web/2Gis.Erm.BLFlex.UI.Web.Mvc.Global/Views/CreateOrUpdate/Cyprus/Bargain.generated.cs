﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.0
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Cyprus/Bargain.cshtml")]
    public partial class Bargain : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Cyprus.CyprusBargainViewModel>
    {
        public Bargain()
        {
        }
        public override void Execute()
        {
            
            #line 2 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
  
    Layout = "../../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 178), Tuple.Create("\"", 235)
, Tuple.Create(Tuple.Create("", 184), Tuple.Create("/Scripts/Ext.DoubleGis.Print.js?", 184), true)
            
            #line 8 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
, Tuple.Create(Tuple.Create("", 216), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 216), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 282), Tuple.Create("\"", 344)
, Tuple.Create(Tuple.Create("", 288), Tuple.Create("/Scripts/Ext.DoubleGis.UI.Bargain.js?", 288), true)
            
            #line 9 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
, Tuple.Create(Tuple.Create("", 325), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 325), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n    <div");

WriteLiteral(" style=\"display: none\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 15 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
   Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 16 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
   Write(Html.HiddenFor(m => m.UserCanWorkWithAdvertisingAgencies));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 17 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
   Write(Html.HiddenFor(m => m.IsBranchOfficeOrganizationUnitChoosingDenied));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 18 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
   Write(Html.HiddenFor(m => m.IsLegalPersonChoosingDenied));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 19 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
   Write(Html.HiddenFor(m => m.ClientId));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 770), Tuple.Create("\"", 806)
            
            #line 21 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
, Tuple.Create(Tuple.Create("", 778), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 778), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 23 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
       Write(Html.TemplateField(m => m.Number, FieldFlex.twins, new Dictionary<string, object> { { "readonly", "readonly" }, { "class", "readonly" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 24 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
       Write(Html.TemplateField(m => m.BargainType, FieldFlex.twins, new LookupSettings { EntityName = EntityName.BargainType, ShowReadOnlyCard = true, ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 27 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
       Write(Html.TemplateField(m => m.SignedOn, FieldFlex.twins, new DateTimeSettings { ReadOnly = true, ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 28 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
       Write(Html.TemplateField(m => m.ClosedOn, FieldFlex.twins, new DateTimeSettings { ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 31 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
       Write(Html.TemplateField(m => m.BargainKind, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 32 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
       Write(Html.TemplateField(m => m.BargainEndDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

            
            #line 35 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
            
            
            #line default
            #line hidden
            
            #line 35 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
             if (Model.ClientId != 0)
            {
                
            
            #line default
            #line hidden
            
            #line 37 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
           Write(Html.TemplateField(m => m.LegalPerson, FieldFlex.lone, new LookupSettings
                    {
                        EntityName = EntityName.LegalPerson,
                        ShowReadOnlyCard = true,
                        ExtendedInfo = "filterToParent=true",
                        ParentEntityName = EntityName.Client,
                        ParentIdPattern = "ClientId"
                    }));

            
            #line default
            #line hidden
            
            #line 44 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
                      
            }
            else
            {
                
            
            #line default
            #line hidden
            
            #line 48 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
           Write(Html.TemplateField(m => m.LegalPerson, FieldFlex.lone, new LookupSettings
                    {
                        EntityName = EntityName.LegalPerson,
                        ShowReadOnlyCard = true
                    }));

            
            #line default
            #line hidden
            
            #line 52 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
                      
            }

            
            #line default
            #line hidden
WriteLiteral("        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 56 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
       Write(Html.TemplateField(m => m.BranchOfficeOrganizationUnit, FieldFlex.lone, new LookupSettings { EntityName = EntityName.BranchOfficeOrganizationUnit, ShowReadOnlyCard = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 59 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
       Write(Html.TemplateField(m => m.Comment, FieldFlex.lone, new Dictionary<string, object> { { "rows", "4" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 62 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
       Write(Html.TemplateField(m => m.HasDocumentsDebt, FieldFlex.lone, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 65 "..\..\Views\CreateOrUpdate\Cyprus\Bargain.cshtml"
       Write(Html.TemplateField(m => m.DocumentsComment, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
