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

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Views.Shared
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
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
    using DoubleGis.Erm.Platform.Common;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.Model.Entities.Enums;
    using DoubleGis.Erm.Platform.Model.Metadata.Enums;
    using DoubleGis.Erm.Platform.UI.Web.Mvc;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Security;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Shared/AdministrationTab.cshtml")]
    public partial class AdministrationTab : System.Web.Mvc.WebViewPage<ViewModels.IEntityViewModelBase>
    {
        public AdministrationTab()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

WriteLiteral("<div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"AdministrationTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 84), Tuple.Create("\"", 127)
            
            #line 4 "..\..\Views\Shared\AdministrationTab.cshtml"
, Tuple.Create(Tuple.Create("", 92), Tuple.Create<System.Object, System.Int32>(BLResources.AdministrationTabTitle
            
            #line default
            #line hidden
, 92), false)
);

WriteLiteral(">\r\n");

WriteLiteral("    ");

            
            #line 5 "..\..\Views\Shared\AdministrationTab.cshtml"
Write(Html.SectionHead("adminHeader", BLResources.AdministrationTabTitle));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 6 "..\..\Views\Shared\AdministrationTab.cshtml"
    
            
            #line default
            #line hidden
            
            #line 6 "..\..\Views\Shared\AdministrationTab.cshtml"
     if (Model.IsCurated && Model.IsSecurityRoot)
    {

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 9 "..\..\Views\Shared\AdministrationTab.cshtml"
   Write(Html.TemplateField(m=>m.Owner, FieldFlex.twins, new LookupSettings{EntityName = EntityName.User, Plugins = new[] { "new Ext.ux.LookupFieldOwner()" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n");

            
            #line 11 "..\..\Views\Shared\AdministrationTab.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 13 "..\..\Views\Shared\AdministrationTab.cshtml"
   Write(Html.TemplateField(m=>m.CreatedBy, FieldFlex.twins, new LookupSettings{EntityName = EntityName.User, ReadOnly = true}));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 14 "..\..\Views\Shared\AdministrationTab.cshtml"
   Write(Html.TemplateField(m => m.CreatedOn, FieldFlex.twins, new DateTimeSettings { ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 17 "..\..\Views\Shared\AdministrationTab.cshtml"
   Write(Html.TemplateField(m => m.ModifiedBy, FieldFlex.twins, new LookupSettings { EntityName = EntityName.User, ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 18 "..\..\Views\Shared\AdministrationTab.cshtml"
   Write(Html.TemplateField(m => m.ModifiedOn, FieldFlex.twins, new DateTimeSettings { ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n</div>\r\n");

        }
    }
}
#pragma warning restore 1591
