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

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Views.CreateOrUpdate.Chile
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Chile/Bill.cshtml")]
    public partial class Bill : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Chile.ChileBillViewModel>
    {
        public Bill()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 4 "..\..\Views\CreateOrUpdate\Chile\Bill.cshtml"
  
    Layout = "../../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 177), Tuple.Create("\"", 234)
, Tuple.Create(Tuple.Create("", 183), Tuple.Create("/Scripts/Ext.DoubleGis.Print.js?", 183), true)
            
            #line 10 "..\..\Views\CreateOrUpdate\Chile\Bill.cshtml"
, Tuple.Create(Tuple.Create("", 215), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 215), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>       \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 288), Tuple.Create("\"", 347)
, Tuple.Create(Tuple.Create("", 294), Tuple.Create("/Scripts/Ext.DoubleGis.UI.Bill.js?", 294), true)
            
            #line 11 "..\..\Views\CreateOrUpdate\Chile\Bill.cshtml"
, Tuple.Create(Tuple.Create("", 328), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 328), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 443), Tuple.Create("\"", 479)
            
            #line 16 "..\..\Views\CreateOrUpdate\Chile\Bill.cshtml"
, Tuple.Create(Tuple.Create("", 451), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 451), false)
);

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 17 "..\..\Views\CreateOrUpdate\Chile\Bill.cshtml"
   Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 18 "..\..\Views\CreateOrUpdate\Chile\Bill.cshtml"
   Write(Html.HiddenFor(m => m.IsOrderActive));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 20 "..\..\Views\CreateOrUpdate\Chile\Bill.cshtml"
       Write(Html.TemplateField(m => m.BillNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 21 "..\..\Views\CreateOrUpdate\Chile\Bill.cshtml"
       Write(Html.TemplateField(m => m.BillDate, FieldFlex.twins, new DateTimeSettings { ReadOnly = true, ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 24 "..\..\Views\CreateOrUpdate\Chile\Bill.cshtml"
       Write(Html.TemplateField(m => m.BeginDistributionDate, FieldFlex.twins, new DateTimeSettings { ReadOnly = true, ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 25 "..\..\Views\CreateOrUpdate\Chile\Bill.cshtml"
       Write(Html.TemplateField(m => m.PaymentDatePlan, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 28 "..\..\Views\CreateOrUpdate\Chile\Bill.cshtml"
       Write(Html.TemplateField(m => m.EndDistributionDate, FieldFlex.twins, new DateTimeSettings { ReadOnly = true, ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 29 "..\..\Views\CreateOrUpdate\Chile\Bill.cshtml"
       Write(Html.TemplateField(m => m.PayablePlan, FieldFlex.twins, new Dictionary<string, object> { { "readonly", "readonly" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 32 "..\..\Views\CreateOrUpdate\Chile\Bill.cshtml"
       Write(Html.TemplateField(m => m.Comment, FieldFlex.lone, new Dictionary<string, object> { { "rows", 5 } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
