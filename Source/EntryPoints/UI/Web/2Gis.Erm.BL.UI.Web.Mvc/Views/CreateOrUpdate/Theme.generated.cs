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

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Views.CreateOrUpdate
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Theme.cshtml")]
    public partial class Theme : System.Web.Mvc.WebViewPage<Models.ThemeViewModel>
    {
        public Theme()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\CreateOrUpdate\Theme.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 123), Tuple.Create("\"", 198)
, Tuple.Create(Tuple.Create("", 129), Tuple.Create("/Scripts/Ext.DoubleGis.UI.Theme.js?", 129), true)
            
            #line 9 "..\..\Views\CreateOrUpdate\Theme.cshtml"
, Tuple.Create(Tuple.Create("", 164), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 164), false)
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

            
            #line 15 "..\..\Views\CreateOrUpdate\Theme.cshtml"
   Write(Html.HiddenFor(m => m.FileName));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 16 "..\..\Views\CreateOrUpdate\Theme.cshtml"
   Write(Html.HiddenFor(m => m.FileContentType));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 17 "..\..\Views\CreateOrUpdate\Theme.cshtml"
   Write(Html.HiddenFor(m => m.FileContentLength));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 18 "..\..\Views\CreateOrUpdate\Theme.cshtml"
   Write(Html.HiddenFor(m => m.IdentityServiceUrl));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 19 "..\..\Views\CreateOrUpdate\Theme.cshtml"
   Write(Html.HiddenFor(m => m.OrganizationUnitCount));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 588), Tuple.Create("\"", 624)
            
            #line 21 "..\..\Views\CreateOrUpdate\Theme.cshtml"
, Tuple.Create(Tuple.Create("", 596), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 596), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 23 "..\..\Views\CreateOrUpdate\Theme.cshtml"
       Write(Html.EditableId(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 26 "..\..\Views\CreateOrUpdate\Theme.cshtml"
       Write(Html.TemplateField(m => m.Name, FieldFlex.twins, new Dictionary<string, object> {{"class", "inputfields"}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 27 "..\..\Views\CreateOrUpdate\Theme.cshtml"
       Write(Html.TemplateField(m => m.ThemeTemplate, FieldFlex.twins, new LookupSettings { EntityName = EntityName.ThemeTemplate }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 30 "..\..\Views\CreateOrUpdate\Theme.cshtml"
       Write(Html.TemplateField(m => m.BeginDistribution, FieldFlex.twins, new DateTimeSettings {ShiftOffset = false, PeriodType = PeriodType.MonthlyLowerBound, DisplayStyle = DisplayStyle.WithoutDayNumber}));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 31 "..\..\Views\CreateOrUpdate\Theme.cshtml"
       Write(Html.TemplateField(m => m.EndDistribution, FieldFlex.twins, new DateTimeSettings {ShiftOffset = false, PeriodType = PeriodType.MonthlyUpperBound, DisplayStyle = DisplayStyle.WithoutDayNumber}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 34 "..\..\Views\CreateOrUpdate\Theme.cshtml"
       Write(Html.TemplateField(m => m.IsDefault, FieldFlex.lone, new Dictionary<string, object> { {"disabled", "disabled"} }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 37 "..\..\Views\CreateOrUpdate\Theme.cshtml"
       Write(Html.TemplateField(m => m.Description, FieldFlex.lone, new Dictionary<string, object> {{"class", "inputfields"}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 40 "..\..\Views\CreateOrUpdate\Theme.cshtml"
       Write(Html.TemplateField(m => m.FileId, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" id=\"UploadedFile-wrapper\"");

WriteLiteral(" class=\"display-wrapper field-wrapper lone\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n                <img");

WriteAttribute("alt", Tuple.Create(" alt=\"", 2110), Tuple.Create("\"", 2142)
            
            #line 44 "..\..\Views\CreateOrUpdate\Theme.cshtml"
, Tuple.Create(Tuple.Create("", 2116), Tuple.Create<System.Object, System.Int32>(BLResources.UploadedImage
            
            #line default
            #line hidden
, 2116), false)
);

WriteLiteral(" id = \"UploadedImage\" src=\"/Content/images/default/blank-image.gif?");

            
            #line 44 "..\..\Views\CreateOrUpdate\Theme.cshtml"
                                                                                                                   Write(SolutionInfo.ProductVersion.Build);

            
            #line default
            #line hidden
WriteLiteral("\"/>\r\n            </div>\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
