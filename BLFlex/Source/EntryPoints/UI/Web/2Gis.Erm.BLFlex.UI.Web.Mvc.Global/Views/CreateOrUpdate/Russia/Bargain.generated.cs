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

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Views.CreateOrUpdate.Russia
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Russia/Bargain.cshtml")]
    public partial class Bargain : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia.BargainViewModel>
    {
        public Bargain()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 4 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
  
    Layout = "../../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 176), Tuple.Create("\"", 233)
, Tuple.Create(Tuple.Create("", 182), Tuple.Create("/Scripts/Ext.DoubleGis.Print.js?", 182), true)
            
            #line 10 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
, Tuple.Create(Tuple.Create("", 214), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 214), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 280), Tuple.Create("\"", 342)
, Tuple.Create(Tuple.Create("", 286), Tuple.Create("/Scripts/Ext.DoubleGis.UI.Bargain.js?", 286), true)
            
            #line 11 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
, Tuple.Create(Tuple.Create("", 323), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 323), false)
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

            
            #line 17 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
   Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 18 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
   Write(Html.HiddenFor(m => m.UserCanWorkWithAdvertisingAgencies));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 19 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
   Write(Html.HiddenFor(m => m.IsBranchOfficeOrganizationUnitChoosingDenied));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 20 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
   Write(Html.HiddenFor(m => m.IsLegalPersonChoosingDenied));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 21 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
   Write(Html.HiddenFor(m => m.ClientId));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 768), Tuple.Create("\"", 804)
            
            #line 23 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
, Tuple.Create(Tuple.Create("", 776), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 776), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 25 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
       Write(Html.TemplateField(m => m.Number, FieldFlex.twins, new Dictionary<string, object> { { "readonly", "readonly" }, { "class", "readonly" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 26 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
       Write(Html.TemplateField(m => m.BargainType, FieldFlex.twins, new LookupSettings { EntityName = EntityType.Instance.BargainType(), ShowReadOnlyCard = true, ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 29 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
       Write(Html.TemplateField(m => m.SignedOn, FieldFlex.twins, new DateTimeSettings { ReadOnly = true, ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 30 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
       Write(Html.TemplateField(m => m.ClosedOn, FieldFlex.twins, new DateTimeSettings { ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 33 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
       Write(Html.TemplateField(m => m.BargainKind, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 34 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
       Write(Html.TemplateField(m => m.BargainEndDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

            
            #line 37 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
            
            
            #line default
            #line hidden
            
            #line 37 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
             if (Model.ClientId != 0)
            {
                
            
            #line default
            #line hidden
            
            #line 39 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
           Write(Html.TemplateField(m => m.LegalPerson, FieldFlex.lone, new LookupSettings
                    {
                        EntityName = EntityType.Instance.LegalPerson(),
                        ShowReadOnlyCard = true,
                        ExtendedInfo = "filterToParent=true",
                        ParentEntityName = EntityType.Instance.Client(),
                        ParentIdPattern = "ClientId"
                    }));

            
            #line default
            #line hidden
            
            #line 46 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
                      
            }
            else
            {
                
            
            #line default
            #line hidden
            
            #line 50 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
           Write(Html.TemplateField(m => m.LegalPerson, FieldFlex.lone, new LookupSettings
                    {
                        EntityName = EntityType.Instance.LegalPerson(),
                        ShowReadOnlyCard = true
                    }));

            
            #line default
            #line hidden
            
            #line 54 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
                      
            }

            
            #line default
            #line hidden
WriteLiteral("        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 58 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
       Write(Html.TemplateField(m => m.BranchOfficeOrganizationUnit, FieldFlex.lone, new LookupSettings { EntityName = EntityType.Instance.BranchOfficeOrganizationUnit(), ShowReadOnlyCard = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 61 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
       Write(Html.TemplateField(m => m.Comment, FieldFlex.lone, new Dictionary<string, object> { { "rows", "4" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 64 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
       Write(Html.TemplateField(m => m.HasDocumentsDebt, FieldFlex.lone, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 67 "..\..\Views\CreateOrUpdate\Russia\Bargain.cshtml"
       Write(Html.TemplateField(m => m.DocumentsComment, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
