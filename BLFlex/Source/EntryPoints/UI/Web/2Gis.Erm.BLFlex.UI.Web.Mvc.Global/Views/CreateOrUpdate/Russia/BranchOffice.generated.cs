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
    using NuClear.Model.Common.Entities;
    using NuClear.Model.Common.Operations.Identity;
    using NuClear.Model.Common.Operations.Identity.Generic;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Russia/BranchOffice.cshtml")]
    public partial class BranchOffice : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia.BranchOfficeViewModel>
    {
        public BranchOffice()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\CreateOrUpdate\Russia\BranchOffice.cshtml"
  
    Layout = "../../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n    \r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 234), Tuple.Create("\"", 270)
            
            #line 14 "..\..\Views\CreateOrUpdate\Russia\BranchOffice.cshtml"
, Tuple.Create(Tuple.Create("", 242), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 242), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 16 "..\..\Views\CreateOrUpdate\Russia\BranchOffice.cshtml"
       Write(Html.EditableId(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 19 "..\..\Views\CreateOrUpdate\Russia\BranchOffice.cshtml"
       Write(Html.TemplateField(m => m.Name, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 22 "..\..\Views\CreateOrUpdate\Russia\BranchOffice.cshtml"
       Write(Html.TemplateField(m => m.LegalAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 25 "..\..\Views\CreateOrUpdate\Russia\BranchOffice.cshtml"
       Write(Html.TemplateField(m => m.Inn, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 28 "..\..\Views\CreateOrUpdate\Russia\BranchOffice.cshtml"
       Write(Html.TemplateField(m => m.BargainType, FieldFlex.twins, new LookupSettings { EntityName = EntityType.Instance.BargainType(), ReadOnly = !Model.IsNew, ShowReadOnlyCard = true}));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 29 "..\..\Views\CreateOrUpdate\Russia\BranchOffice.cshtml"
       Write(Html.TemplateField(m => m.ContributionType, FieldFlex.twins, new LookupSettings { EntityName = EntityType.Instance.ContributionType(), ReadOnly = !Model.IsNew, ShowReadOnlyCard = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"AnnotationTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 1205), Tuple.Create("\"", 1231)
            
            #line 32 "..\..\Views\CreateOrUpdate\Russia\BranchOffice.cshtml"
, Tuple.Create(Tuple.Create("", 1213), Tuple.Create<System.Object, System.Int32>(BLResources.Notes
            
            #line default
            #line hidden
, 1213), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 34 "..\..\Views\CreateOrUpdate\Russia\BranchOffice.cshtml"
       Write(Html.TemplateField(m => m.UsnNotificationText, FieldFlex.lone, new Dictionary<string, object>{{"rows", "10"}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 37 "..\..\Views\CreateOrUpdate\Russia\BranchOffice.cshtml"
       Write(Html.TemplateField(m => m.Annotation, FieldFlex.lone, new Dictionary<string, object> { { "rows", "10" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"AdditionalTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 1634), Tuple.Create("\"", 1673)
            
            #line 40 "..\..\Views\CreateOrUpdate\Russia\BranchOffice.cshtml"
, Tuple.Create(Tuple.Create("", 1642), Tuple.Create<System.Object, System.Int32>(BLResources.AdditionalTabTitle
            
            #line default
            #line hidden
, 1642), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 42 "..\..\Views\CreateOrUpdate\Russia\BranchOffice.cshtml"
       Write(Html.TemplateField(m => m.DgppId, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
