﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34003
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Views.CreateOrUpdate
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Position.cshtml")]
    public partial class Position : System.Web.Mvc.WebViewPage<PositionViewModel>
    {
        public Position()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 4 "..\..\Views\CreateOrUpdate\Position.cshtml"
  
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

            
            #line 14 "..\..\Views\CreateOrUpdate\Position.cshtml"
    
            
            #line default
            #line hidden
            
            #line 14 "..\..\Views\CreateOrUpdate\Position.cshtml"
      
        var elementStyle = Model.IsPublished
                                ? new Dictionary<string, object> { { "disabled", "disabled" } }
                                : new Dictionary<string, object>();
    
            
            #line default
            #line hidden
WriteLiteral("\r\n    \r\n");

WriteLiteral("    ");

            
            #line 20 "..\..\Views\CreateOrUpdate\Position.cshtml"
Write(Html.HiddenFor(m => m.IdentityServiceUrl));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 21 "..\..\Views\CreateOrUpdate\Position.cshtml"
Write(Html.HiddenFor(m => m.IsPublished));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 22 "..\..\Views\CreateOrUpdate\Position.cshtml"
Write(Html.HiddenFor(m => m.IsReadonlyTemplate));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 23 "..\..\Views\CreateOrUpdate\Position.cshtml"
Write(Html.HiddenFor(m => m.RestrictChildPositionPlatformsCanBeChanged));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 614), Tuple.Create("\"", 654)
            
            #line 25 "..\..\Views\CreateOrUpdate\Position.cshtml"
, Tuple.Create(Tuple.Create("", 622), Tuple.Create<System.Object, System.Int32>(BLResources.TabTitleInformation
            
            #line default
            #line hidden
, 622), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 27 "..\..\Views\CreateOrUpdate\Position.cshtml"
       Write(Html.EditableId(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 30 "..\..\Views\CreateOrUpdate\Position.cshtml"
       Write(Html.TemplateField(m => m.Name, FieldFlex.lone, new Dictionary<string, object> {{"class", "inputfields"}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 33 "..\..\Views\CreateOrUpdate\Position.cshtml"
       Write(Html.TemplateField(m => m.IsComposite, FieldFlex.lone, elementStyle));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 36 "..\..\Views\CreateOrUpdate\Position.cshtml"
       Write(Html.TemplateField(m => m.RestrictChildPositionPlatforms, FieldFlex.lone, elementStyle));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 39 "..\..\Views\CreateOrUpdate\Position.cshtml"
       Write(Html.TemplateField(m => m.DgppId, FieldFlex.lone, new Dictionary<string, object> {{"class", "inputfields"}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 42 "..\..\Views\CreateOrUpdate\Position.cshtml"
       Write(Html.TemplateField(m => m.ExportCode, FieldFlex.lone, new Dictionary<string, object> {{"class", "inputfields"}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 45 "..\..\Views\CreateOrUpdate\Position.cshtml"
       Write(Html.TemplateField(m => m.CalculationMethod, FieldFlex.lone, elementStyle, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 48 "..\..\Views\CreateOrUpdate\Position.cshtml"
       Write(Html.TemplateField(m => m.BindingObjectType, FieldFlex.lone, elementStyle, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 51 "..\..\Views\CreateOrUpdate\Position.cshtml"
       Write(Html.TemplateField(m => m.AccountingMethod, FieldFlex.lone, elementStyle, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 54 "..\..\Views\CreateOrUpdate\Position.cshtml"
       Write(Html.TemplateField(m => m.Platform, FieldFlex.lone, new LookupSettings { EntityName = EntityName.Platform, ReadOnly = Model.IsPublished }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 57 "..\..\Views\CreateOrUpdate\Position.cshtml"
       Write(Html.TemplateField(m => m.PositionCategory, FieldFlex.lone, new LookupSettings { EntityName = EntityName.PositionCategory, ReadOnly = Model.IsPublished }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 60 "..\..\Views\CreateOrUpdate\Position.cshtml"
       Write(Html.TemplateField(m => m.AdvertisementTemplate, FieldFlex.lone, new LookupSettings { EntityName = EntityName.AdvertisementTemplate, ReadOnly = Model.IsPublished }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 63 "..\..\Views\CreateOrUpdate\Position.cshtml"
       Write(Html.TemplateField(m => m.IsControledByAmount, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
