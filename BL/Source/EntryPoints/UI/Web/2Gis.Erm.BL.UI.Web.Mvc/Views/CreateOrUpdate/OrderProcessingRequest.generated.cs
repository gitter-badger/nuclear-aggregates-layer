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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/OrderProcessingRequest.cshtml")]
    public partial class OrderProcessingRequest : System.Web.Mvc.WebViewPage<OrderProcessingRequestViewModel>
    {
        public OrderProcessingRequest()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 4 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 135), Tuple.Create("\"", 198)
, Tuple.Create(Tuple.Create("", 141), Tuple.Create("/Scripts/Ext.ux.RequestMessagesTab.js?", 141), true)
            
            #line 10 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
, Tuple.Create(Tuple.Create("", 179), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 179), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    ");

WriteLiteral("\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 380), Tuple.Create("\"", 425)
, Tuple.Create(Tuple.Create("", 387), Tuple.Create("/Content/order.css?", 387), true)
            
            #line 12 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
, Tuple.Create(Tuple.Create("", 406), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 406), false)
);

WriteLiteral(" />\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 442), Tuple.Create("\"", 502)
, Tuple.Create(Tuple.Create("", 448), Tuple.Create("/Scripts/Ext.DoubleGis.UI.Order.js?", 448), true)
            
            #line 13 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
, Tuple.Create(Tuple.Create("", 483), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 483), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 549), Tuple.Create("\"", 626)
, Tuple.Create(Tuple.Create("", 555), Tuple.Create("/Scripts/Ext.DoubleGis.Order.UpgradeResultWindow.js?", 555), true)
            
            #line 14 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
, Tuple.Create(Tuple.Create("", 607), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 607), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 679), Tuple.Create("\"", 756)
, Tuple.Create(Tuple.Create("", 685), Tuple.Create("/Scripts/Ext.DoubleGis.UI.OrderProcessingRequest.js?", 685), true)
            
            #line 16 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
, Tuple.Create(Tuple.Create("", 737), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 737), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n");

            
            #line 21 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
    
            
            #line default
            #line hidden
            
            #line 21 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
       var readonlyTextbox = new Dictionary<string, object> { { "readonly", "readonly" } }; 
            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 22 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
    
            
            #line default
            #line hidden
            
            #line 22 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
       var readonlyCombobox = new Dictionary<string, object> { { "disabled", "disabled" } }; 
            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("    ");

            
            #line 24 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 25 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
Write(Html.HiddenFor(m => m.DueDate));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 26 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
Write(Html.HiddenFor(m => m.LegalPersonProfile));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 27 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
Write(Html.HiddenFor(m => m.RequestType));

            
            #line default
            #line hidden
WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 1203), Tuple.Create("\"", 1239)
            
            #line 28 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
, Tuple.Create(Tuple.Create("", 1211), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 1211), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 30 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
       Write(Html.TemplateField(m => m.Title, FieldFlex.lone, readonlyTextbox));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(" >\r\n");

WriteLiteral("            ");

            
            #line 33 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
       Write(Html.TemplateField(m => m.State, FieldFlex.lone, readonlyCombobox, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 36 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
       Write(Html.TemplateField(m => m.ReleaseCountPlan, FieldFlex.lone, readonlyTextbox));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div> \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 39 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
       Write(Html.TemplateField(m => m.SourceOrganizationUnit, FieldFlex.twins, new LookupSettings { EntityName = EntityType.Instance.OrganizationUnit(), ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 40 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
       Write(Html.TemplateField(m => m.Firm, FieldFlex.twins, new LookupSettings { EntityName = EntityType.Instance.Firm(), ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div> \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 43 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
       Write(Html.TemplateField(m => m.LegalPerson, FieldFlex.lone, new LookupSettings { EntityName = EntityType.Instance.LegalPerson(), ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 46 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
       Write(Html.TemplateField(m => m.BaseOrder, FieldFlex.twins, new LookupSettings { EntityName = EntityType.Instance.Order(), ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 47 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
       Write(Html.TemplateField(m => m.RenewedOrder, FieldFlex.twins, new LookupSettings { EntityName = EntityType.Instance.Order(), ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 50 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
       Write(Html.TemplateField(m => m.DueDateString, FieldFlex.twins, readonlyTextbox));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 51 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
       Write(Html.TemplateField(m => m.BeginDistributionDate, FieldFlex.twins, new DateTimeSettings { ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 54 "..\..\Views\CreateOrUpdate\OrderProcessingRequest.cshtml"
       Write(Html.TemplateField(m => m.Description, FieldFlex.lone, new Dictionary<string, object> { { "rows", 12 }, { "readonly", "readonly" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

WriteLiteral("\r\n");

        }
    }
}
#pragma warning restore 1591
