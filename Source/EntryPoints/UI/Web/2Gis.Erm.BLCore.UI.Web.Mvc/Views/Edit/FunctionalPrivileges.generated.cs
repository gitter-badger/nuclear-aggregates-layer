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

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Views.Edit
{
    using System;
    using System.Web.Mvc.Html;

    using DoubleGis.Erm.BLCore.Resources.Server.Properties;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
    using DoubleGis.Erm.Platform.Common;

    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Edit/FunctionalPrivileges.cshtml")]
    public partial class FunctionalPrivileges : System.Web.Mvc.WebViewPage<EditPrivilegeViewModel>
    {
        public FunctionalPrivileges()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\Edit\FunctionalPrivileges.cshtml"
  
    Layout = "../Shared/_SiteLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("TitleContent", () => {

WriteLiteral(" ");

});

WriteLiteral("\r\n");

DefineSection("HeadContent", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">
        window.BeforeSave = function (onSuccess, onFailure)
        {
            if (Ext.DoubleGis.UI.FunctionalPermissionControlInstance)
            {
                Ext.DoubleGis.UI.FunctionalPermissionControlInstance.Save(onSuccess, onFailure);
            }
        }
    </script>
    
    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 493), Tuple.Create("\"", 566)
, Tuple.Create(Tuple.Create("", 499), Tuple.Create("/Scripts/Ext.grid.CheckColumn.js?", 499), true)
            
            #line 21 "..\..\Views\Edit\FunctionalPrivileges.cshtml"
, Tuple.Create(Tuple.Create("", 532), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 532), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 613), Tuple.Create("\"", 718)
, Tuple.Create(Tuple.Create("", 619), Tuple.Create("/Scripts/Ext.DoubleGis.UI.Security.FunctionalPrivilegeControl.js?", 619), true)
            
            #line 22 "..\..\Views\Edit\FunctionalPrivileges.cshtml"
   , Tuple.Create(Tuple.Create("", 684), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 684), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("MainContent", () => {

WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 27 "..\..\Views\Edit\FunctionalPrivileges.cshtml"
Write(Html.HiddenFor(m => m.RoleId));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 28 "..\..\Views\Edit\FunctionalPrivileges.cshtml"
Write(Html.Hidden("privilegeNameLocalized", BLResources.PrivilegeName));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 29 "..\..\Views\Edit\FunctionalPrivileges.cshtml"
Write(Html.Hidden("valueLocalized", BLResources.Value));

            
            #line default
            #line hidden
WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"FunctionalPermission\"");

WriteAttribute("title", Tuple.Create(" title=\"", 992), Tuple.Create("\"", 1033)
            
            #line 30 "..\..\Views\Edit\FunctionalPrivileges.cshtml"
, Tuple.Create(Tuple.Create("", 1000), Tuple.Create<System.Object, System.Int32>(BLResources.FunctionalPrivileges
            
            #line default
            #line hidden
, 1000), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" id=\"funcPermissionPanel\"");

WriteLiteral("/>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
