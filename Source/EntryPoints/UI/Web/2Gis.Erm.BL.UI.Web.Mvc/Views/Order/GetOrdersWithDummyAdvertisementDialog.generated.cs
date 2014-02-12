﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Views.Order
{
    using System;
    using System.Collections.Generic;
    
    #line 2 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
    using System.Globalization;
    
    #line default
    #line hidden
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Order/GetOrdersWithDummyAdvertisementDialog.cshtml")]
    public partial class GetOrdersWithDummyAdvertisementDialog : System.Web.Mvc.WebViewPage<Models.GetOrdersWithDummyAdvertisementDialogModel>
    {
        public GetOrdersWithDummyAdvertisementDialog()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 5 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 9 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
            Write(BLResources.GetOrdersWithDummyAdvertisementReportTitle);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 10 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
                  Write(BLResources.GetOrdersWithDummyAdvertisementReportTitle);

            
            #line default
            #line hidden
});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 11 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
                    Write(BLResources.GetOrdersWithDummyAdvertisementParametersInformation);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

WriteLiteral("\r\n");

DefineSection("PageContent", () => {

WriteLiteral("\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 468), Tuple.Create("\"", 552)
, Tuple.Create(Tuple.Create("", 475), Tuple.Create("/Content/Ext.ux.DetailedProgressWindow.css?", 475), true)
            
            #line 15 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
             , Tuple.Create(Tuple.Create("", 518), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 518), false)
);

WriteLiteral(" />\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 575), Tuple.Create("\"", 657)
, Tuple.Create(Tuple.Create("", 581), Tuple.Create("/Scripts/Ext.ux.DetailedProgressWindow.js?", 581), true)
            
            #line 17 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
, Tuple.Create(Tuple.Create("", 623), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 623), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 704), Tuple.Create("\"", 791)
, Tuple.Create(Tuple.Create("", 710), Tuple.Create("/Scripts/Ext.ux.AsyncOperationClientManager.js?", 710), true)
            
            #line 18 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
, Tuple.Create(Tuple.Create("", 757), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 757), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">

        Ext.onReady(function() {
            if (Ext.getDom('ReportLink')) {
                Ext.getDom('ReportLink').onclick = function() {
                    Ext.getDom('ReportForm').submit();
                };
            }

            //Show error messages
            if (Ext.getDom(""Notifications"").innerHTML.trim() != """") {
                Ext.get(""Notifications"").addClass(""Notifications"");
            } else {
                Ext.get(""Notifications"").removeClass(""Notifications"");
            }

            Ext.get(""Cancel"").on(""click"", function() {
                window.close();
            });

            Ext.get(""OK"").on(""click"", function () {
                if (Ext.DoubleGis.FormValidator.validate(window.EntityForm)) {
                    Ext.getDom(""OK"").disabled = ""disabled"";
                    Ext.getDom(""Cancel"").disabled = ""disabled"";
                    EntityForm.submit();
                }
            });

            Ext.each(Ext.CardLookupSettings, function(item, i) {
                new window.Ext.ux.LookupField(item);
            }, this);
        });

    </script>
    
");

            
            #line 55 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
    
            
            #line default
            #line hidden
            
            #line 55 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
     if (Model.HasOrders == true)
    {

            
            #line default
            #line hidden
WriteLiteral("        <div");

WriteLiteral(" style=\"height: 8px; padding-left: 5px; padding-top: 4px; position: fixed;\"");

WriteLiteral(" id=\"DivErrors\"");

WriteLiteral(">\r\n");

            
            #line 58 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
            
            
            #line default
            #line hidden
            
            #line 58 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
             using (Html.BeginForm("GetOperationLog", "Operation", FormMethod.Post, new Dictionary<string, object> { { "target", "_blank" }, { "id", "ReportForm" } }))
            {

            
            #line default
            #line hidden
WriteLiteral("                <input");

WriteLiteral(" type=\"hidden\"");

WriteLiteral(" name=\"operationId\"");

WriteAttribute("value", Tuple.Create(" value=\"", 2403), Tuple.Create("\"", 2434)
            
            #line 60 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
, Tuple.Create(Tuple.Create("", 2411), Tuple.Create<System.Object, System.Int32>(Model.OrdersListFileId
            
            #line default
            #line hidden
, 2411), false)
);

WriteLiteral(" />\r\n");

            
            #line 61 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
            }

            
            #line default
            #line hidden
WriteLiteral("        </div>\r\n");

            
            #line 63 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 65 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
    
            
            #line default
            #line hidden
            
            #line 65 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {
        
            
            #line default
            #line hidden
            
            #line 67 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
   Write(Html.Hidden("now", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)));

            
            #line default
            #line hidden
            
            #line 67 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
                                                                                   
        
            
            #line default
            #line hidden
            
            #line 68 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
   Write(Html.HiddenFor(m=>m.UserId));

            
            #line default
            #line hidden
            
            #line 68 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
                                    

            
            #line default
            #line hidden
WriteLiteral("        <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" style=\"height: 18px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(" class=\"Notifications\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 71 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
           Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 72 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
                
            
            #line default
            #line hidden
            
            #line 72 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
                 if (Model.HasOrders == true)
                {

            
            #line default
            #line hidden
WriteLiteral("                    <a");

WriteLiteral(" href=\"#\"");

WriteLiteral(" id=\"ReportLink\"");

WriteLiteral("> Просмотреть отчет...</a>\r\n");

            
            #line 75 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
                }

            
            #line default
            #line hidden
WriteLiteral("            </div>\r\n       \r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 79 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
           Write(Html.TemplateField(m => m.OrganizationUnit, FieldFlex.lone, new LookupSettings { EntityName = EntityName.OrganizationUnit, ExtendedInfo = "userId=" + Model.UserId }));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 82 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
           Write(Html.TemplateField(m => m.Owner, FieldFlex.lone, new LookupSettings { EntityName = EntityName.User, ExtendedInfo = "subordinatesOf=" + Model.UserId }));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 85 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
           Write(Html.TemplateField(m => m.IncludeOwnerDescendants, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n        </div>\r\n");

            
            #line 88 "..\..\Views\Order\GetOrdersWithDummyAdvertisementDialog.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
