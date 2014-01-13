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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/OperationType.cshtml")]
    public partial class OperationType : System.Web.Mvc.WebViewPage<OperationTypeViewModel>
    {
        public OperationType()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 4 "..\..\Views\CreateOrUpdate\OperationType.cshtml"
  
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

WriteLiteral("    ");

            
            #line 14 "..\..\Views\CreateOrUpdate\OperationType.cshtml"
Write(Html.HiddenFor(m => m.IdentityServiceUrl));

            
            #line default
            #line hidden
WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 223), Tuple.Create("\"", 259)
            
            #line 15 "..\..\Views\CreateOrUpdate\OperationType.cshtml"
, Tuple.Create(Tuple.Create("", 231), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 231), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 17 "..\..\Views\CreateOrUpdate\OperationType.cshtml"
       Write(Html.EditableId(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 20 "..\..\Views\CreateOrUpdate\OperationType.cshtml"
       Write(Html.TemplateField(m => m.Name, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n            <div");

WriteLiteral(" class=\"display-wrapper field-wrapper twins\"");

WriteLiteral(" id=\"IsPlus-wrapper\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"label-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                    ");

            
            #line 23 "..\..\Views\CreateOrUpdate\OperationType.cshtml"
               Write(Html.LabelFor(m => m.IsPlus));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </div>\r\n                <div");

WriteLiteral(" class=\"input-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                    ");

            
            #line 26 "..\..\Views\CreateOrUpdate\OperationType.cshtml"
               Write(Html.DropDownListFor(m => m.IsPlus,
                                          new[]
                                            {
                                                new SelectListItem { Text = BLResources.Withdrawal, Value = bool.FalseString },
                                                new SelectListItem { Text = BLResources.Charge, Value = bool.TrueString }
                                            }, 
                                          new Dictionary<string, object> { { "class", "inputfields" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </div>\r\n            </div>\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 37 "..\..\Views\CreateOrUpdate\OperationType.cshtml"
       Write(Html.TemplateField(m => m.Description, FieldFlex.lone, new Dictionary<string, object> { { "rows", 5 } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" style=\"width: 100%; height: 100%;\"");

WriteLiteral(" id=\"AdditionalTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 1578), Tuple.Create("\"", 1617)
            
            #line 40 "..\..\Views\CreateOrUpdate\OperationType.cshtml"
  , Tuple.Create(Tuple.Create("", 1586), Tuple.Create<System.Object, System.Int32>(BLResources.AdditionalTabTitle
            
            #line default
            #line hidden
, 1586), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 42 "..\..\Views\CreateOrUpdate\OperationType.cshtml"
       Write(Html.TemplateField(m => m.IsInSyncWith1C, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 43 "..\..\Views\CreateOrUpdate\OperationType.cshtml"
       Write(Html.TemplateField(m => m.SyncCode1C, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
