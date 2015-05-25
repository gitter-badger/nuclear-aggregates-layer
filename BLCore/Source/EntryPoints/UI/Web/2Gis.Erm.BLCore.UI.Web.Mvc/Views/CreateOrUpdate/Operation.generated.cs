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
    using DoubleGis.Erm.BLCore.UI.Metadata.Confirmations;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models.GroupOperation;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
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
    using NuClear.Model.Common.Entities;
    using NuClear.Model.Common.Operations.Identity;
    using NuClear.Model.Common.Operations.Identity.Generic;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Operation.cshtml")]
    public partial class Operation : System.Web.Mvc.WebViewPage<OperationViewModel>
    {
        public Operation()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 4 "..\..\Views\CreateOrUpdate\Operation.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(@">
        DIV.Tab TABLE TD
        {
            padding-bottom: 0px !important;
        }
        DIV.Tab TABLE
        {
            width: auto !important;
        }
        div.display-wrapper
        {
            padding-top: 3px !important;
            padding-bottom: 3px !important;
        }
        div.label-wrapper
        {
            width: 130px !important;
        }
    </style>    

");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 620), Tuple.Create("\"", 656)
            
            #line 34 "..\..\Views\CreateOrUpdate\Operation.cshtml"
, Tuple.Create(Tuple.Create("", 628), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 628), false)
);

WriteLiteral(">\r\n    <div");

WriteLiteral(" style=\"display: none\"");

WriteLiteral(">\r\n");

            
            #line 36 "..\..\Views\CreateOrUpdate\Operation.cshtml"
        
            
            #line default
            #line hidden
            
            #line 36 "..\..\Views\CreateOrUpdate\Operation.cshtml"
         if (Model != null)
        {
            
            
            #line default
            #line hidden
            
            #line 38 "..\..\Views\CreateOrUpdate\Operation.cshtml"
       Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
            
            #line 38 "..\..\Views\CreateOrUpdate\Operation.cshtml"
                                      
        }

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n        <br />\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 43 "..\..\Views\CreateOrUpdate\Operation.cshtml"
       Write(Html.TemplateField(m => m.Type, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 46 "..\..\Views\CreateOrUpdate\Operation.cshtml"
       Write(Html.TemplateField(m => m.StartTime, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 47 "..\..\Views\CreateOrUpdate\Operation.cshtml"
       Write(Html.TemplateField(m => m.FinishTime, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 50 "..\..\Views\CreateOrUpdate\Operation.cshtml"
       Write(Html.TemplateField(m => m.Status, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 51 "..\..\Views\CreateOrUpdate\Operation.cshtml"
       Write(Html.TemplateField(m => m.OrganizationUnit, FieldFlex.twins, new LookupSettings { EntityName = EntityType.Instance.OrganizationUnit(), ShowReadOnlyCard = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 54 "..\..\Views\CreateOrUpdate\Operation.cshtml"
       Write(Html.TemplateField(m => m.Description, FieldFlex.lone, new Dictionary<string, object>{{"rows", 5}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

WriteLiteral("\r\n\r\n\r\n");

        }
    }
}
#pragma warning restore 1591
