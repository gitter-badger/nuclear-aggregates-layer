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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/ReleaseInfo.cshtml")]
    public partial class ReleaseInfo : System.Web.Mvc.WebViewPage<ReleaseInfoViewModel>
    {
        public ReleaseInfo()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 4 "..\..\Views\CreateOrUpdate\ReleaseInfo.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 124), Tuple.Create("\"", 205)
, Tuple.Create(Tuple.Create("", 130), Tuple.Create("/Scripts/Ext.DoubleGis.UI.ReleaseInfo.js?", 130), true)
            
            #line 10 "..\..\Views\CreateOrUpdate\ReleaseInfo.cshtml"
, Tuple.Create(Tuple.Create("", 171), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 171), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 301), Tuple.Create("\"", 337)
            
            #line 15 "..\..\Views\CreateOrUpdate\ReleaseInfo.cshtml"
, Tuple.Create(Tuple.Create("", 309), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 309), false)
);

WriteLiteral(">\r\n");

            
            #line 16 "..\..\Views\CreateOrUpdate\ReleaseInfo.cshtml"
        
            
            #line default
            #line hidden
            
            #line 16 "..\..\Views\CreateOrUpdate\ReleaseInfo.cshtml"
         if (Model != null)
        {
            
            
            #line default
            #line hidden
            
            #line 18 "..\..\Views\CreateOrUpdate\ReleaseInfo.cshtml"
       Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
            
            #line 18 "..\..\Views\CreateOrUpdate\ReleaseInfo.cshtml"
                                      
        }

            
            #line default
            #line hidden
WriteLiteral("        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 21 "..\..\Views\CreateOrUpdate\ReleaseInfo.cshtml"
       Write(Html.TemplateField(m => m.PeriodStartDate, FieldFlex.twins, new DateTimeSettings{ShiftOffset = false}));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 22 "..\..\Views\CreateOrUpdate\ReleaseInfo.cshtml"
       Write(Html.TemplateField(m => m.PeriodEndDate, FieldFlex.twins, new DateTimeSettings{ShiftOffset = false}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 25 "..\..\Views\CreateOrUpdate\ReleaseInfo.cshtml"
       Write(Html.TemplateField(m => m.OrganizationUnit, FieldFlex.twins, new LookupSettings { EntityName = EntityName.OrganizationUnit, Disabled = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 26 "..\..\Views\CreateOrUpdate\ReleaseInfo.cshtml"
       Write(Html.TemplateField(m => m.Status, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 29 "..\..\Views\CreateOrUpdate\ReleaseInfo.cshtml"
       Write(Html.TemplateField(m => m.IsBeta, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 32 "..\..\Views\CreateOrUpdate\ReleaseInfo.cshtml"
       Write(Html.TemplateField(m => m.Comment, FieldFlex.lone, new Dictionary<string, object>{{"rows", "5"}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
