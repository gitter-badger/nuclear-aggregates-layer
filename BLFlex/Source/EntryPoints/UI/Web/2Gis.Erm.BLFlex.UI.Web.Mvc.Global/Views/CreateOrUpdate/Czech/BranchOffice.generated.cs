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

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Views.CreateOrUpdate.Czech
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Czech/BranchOffice.cshtml")]
    public partial class BranchOffice : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Czech.CzechBranchOfficeViewModel>
    {
        public BranchOffice()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\CreateOrUpdate\Czech\BranchOffice.cshtml"
  
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

WriteAttribute("title", Tuple.Create(" title=\"", 238), Tuple.Create("\"", 274)
            
            #line 14 "..\..\Views\CreateOrUpdate\Czech\BranchOffice.cshtml"
, Tuple.Create(Tuple.Create("", 246), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 246), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 16 "..\..\Views\CreateOrUpdate\Czech\BranchOffice.cshtml"
       Write(Html.EditableId(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 19 "..\..\Views\CreateOrUpdate\Czech\BranchOffice.cshtml"
       Write(Html.TemplateField(m => m.Name, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 22 "..\..\Views\CreateOrUpdate\Czech\BranchOffice.cshtml"
       Write(Html.TemplateField(m => m.LegalAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 25 "..\..\Views\CreateOrUpdate\Czech\BranchOffice.cshtml"
       Write(Html.TemplateField(m => m.Ic, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 26 "..\..\Views\CreateOrUpdate\Czech\BranchOffice.cshtml"
       Write(Html.TemplateField(m => m.Dic, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 29 "..\..\Views\CreateOrUpdate\Czech\BranchOffice.cshtml"
       Write(Html.TemplateField(m => m.BargainType, FieldFlex.twins, new LookupSettings { EntityName = EntityType.Instance.BargainType(), ReadOnly = !Model.IsNew, ShowReadOnlyCard = true}));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 30 "..\..\Views\CreateOrUpdate\Czech\BranchOffice.cshtml"
       Write(Html.TemplateField(m => m.ContributionType, FieldFlex.twins, new LookupSettings { EntityName = EntityType.Instance.ContributionType(), ReadOnly = !Model.IsNew, ShowReadOnlyCard = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
