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
    using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
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
    using NuClear.Model.Common.Entities;
    using NuClear.Model.Common.Operations.Identity;
    using NuClear.Model.Common.Operations.Identity.Generic;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/AssociatedPositionsGroup.cshtml")]
    public partial class AssociatedPositionsGroup : System.Web.Mvc.WebViewPage<AssociatedPositionsGroupViewModel>
    {
        public AssociatedPositionsGroup()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\CreateOrUpdate\AssociatedPositionsGroup.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n<div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 180), Tuple.Create("\"", 216)
            
            #line 13 "..\..\Views\CreateOrUpdate\AssociatedPositionsGroup.cshtml"
, Tuple.Create(Tuple.Create("", 188), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 188), false)
);

WriteLiteral(">\r\n");

            
            #line 14 "..\..\Views\CreateOrUpdate\AssociatedPositionsGroup.cshtml"
    
            
            #line default
            #line hidden
            
            #line 14 "..\..\Views\CreateOrUpdate\AssociatedPositionsGroup.cshtml"
     if (Model != null)
    {
        
            
            #line default
            #line hidden
            
            #line 16 "..\..\Views\CreateOrUpdate\AssociatedPositionsGroup.cshtml"
   Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
            
            #line 16 "..\..\Views\CreateOrUpdate\AssociatedPositionsGroup.cshtml"
                                  
    }

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 19 "..\..\Views\CreateOrUpdate\AssociatedPositionsGroup.cshtml"
   Write(Html.TemplateField(m => m.Name, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 20 "..\..\Views\CreateOrUpdate\AssociatedPositionsGroup.cshtml"
   Write(Html.TemplateField(m => m.PricePosition, FieldFlex.twins, new LookupSettings { EntityName = EntityType.Instance.PricePosition(), ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n</div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
