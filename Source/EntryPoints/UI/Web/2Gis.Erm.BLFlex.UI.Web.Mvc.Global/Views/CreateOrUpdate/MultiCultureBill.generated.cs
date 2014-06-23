﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Views.CreateOrUpdate
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
    
    #line 1 "..\..\Views\CreateOrUpdate\MultiCultureBill.cshtml"
    using BLCore.UI.Web.Mvc.Utils;
    
    #line default
    #line hidden
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/MultiCultureBill.cshtml")]
    public partial class MultiCultureBill : System.Web.Mvc.WebViewPage<Models.MultiCultureBillViewModel>
    {
        public MultiCultureBill()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 5 "..\..\Views\CreateOrUpdate\MultiCultureBill.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 168), Tuple.Create("\"", 240)
, Tuple.Create(Tuple.Create("", 174), Tuple.Create("/Scripts/Ext.DoubleGis.Print.js?", 174), true)
            
            #line 11 "..\..\Views\CreateOrUpdate\MultiCultureBill.cshtml"
, Tuple.Create(Tuple.Create("", 206), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 206), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>       \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 294), Tuple.Create("\"", 368)
, Tuple.Create(Tuple.Create("", 300), Tuple.Create("/Scripts/Ext.DoubleGis.UI.Bill.js?", 300), true)
            
            #line 12 "..\..\Views\CreateOrUpdate\MultiCultureBill.cshtml"
, Tuple.Create(Tuple.Create("", 334), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 334), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(" ></script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 465), Tuple.Create("\"", 501)
            
            #line 17 "..\..\Views\CreateOrUpdate\MultiCultureBill.cshtml"
, Tuple.Create(Tuple.Create("", 473), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 473), false)
);

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 18 "..\..\Views\CreateOrUpdate\MultiCultureBill.cshtml"
   Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 19 "..\..\Views\CreateOrUpdate\MultiCultureBill.cshtml"
   Write(Html.HiddenFor(m => m.IsOrderActive));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 21 "..\..\Views\CreateOrUpdate\MultiCultureBill.cshtml"
       Write(Html.TemplateField(m => m.BillNumber, FieldFlex.twins, new Dictionary<string, object> { { "readonly", "readonly" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 22 "..\..\Views\CreateOrUpdate\MultiCultureBill.cshtml"
       Write(Html.TemplateField(m => m.BillDate, FieldFlex.twins, new DateTimeSettings { ReadOnly = true, ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 25 "..\..\Views\CreateOrUpdate\MultiCultureBill.cshtml"
       Write(Html.TemplateField(m => m.BeginDistributionDate, FieldFlex.twins, new DateTimeSettings { ReadOnly = true, ShiftOffset = false}));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 26 "..\..\Views\CreateOrUpdate\MultiCultureBill.cshtml"
       Write(Html.TemplateField(m => m.PaymentDatePlan, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 29 "..\..\Views\CreateOrUpdate\MultiCultureBill.cshtml"
       Write(Html.TemplateField(m => m.EndDistributionDate, FieldFlex.twins, new DateTimeSettings { ReadOnly = true, ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 30 "..\..\Views\CreateOrUpdate\MultiCultureBill.cshtml"
       Write(Html.TemplateField(m => m.PayablePlan, FieldFlex.twins, new Dictionary<string, object> { { "readonly", "readonly" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 33 "..\..\Views\CreateOrUpdate\MultiCultureBill.cshtml"
       Write(Html.TemplateField(m => m.Comment, FieldFlex.lone, new Dictionary<string, object> { { "rows", 5 } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

WriteLiteral("\r\n");

            
            #line 38 "..\..\Views\CreateOrUpdate\MultiCultureBill.cshtml"
Write(RenderBody());

            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591
