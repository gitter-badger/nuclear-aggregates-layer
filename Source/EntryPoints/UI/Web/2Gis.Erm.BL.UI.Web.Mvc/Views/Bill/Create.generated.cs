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

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Views.Bill
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
    
    #line 1 "..\..\Views\Bill\Create.cshtml"
    using BLCore.API.Operations.Concrete.Old.Bills;
    
    #line default
    #line hidden
    
    #line 2 "..\..\Views\Bill\Create.cshtml"
    using BLCore.UI.Web.Mvc.Settings;
    
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Bill/Create.cshtml")]
    public partial class Create : System.Web.Mvc.WebViewPage<Models.CreateBillViewModel>
    {
        public Create()
        {
        }
        public override void Execute()
        {
            
            #line 5 "..\..\Views\Bill\Create.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 9 "..\..\Views\Bill\Create.cshtml"
            Write(BLResources.CreateBill);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 10 "..\..\Views\Bill\Create.cshtml"
                  Write(BLResources.CreateBill);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 11 "..\..\Views\Bill\Create.cshtml"
                    Write(BLResources.CreateBill);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

WriteLiteral("\r\n");

DefineSection("PageContent", () => {

WriteLiteral("\r\n    <link");

WriteLiteral("  rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 394), Tuple.Create("\"", 455)
, Tuple.Create(Tuple.Create("", 401), Tuple.Create("/Content/slider.css?", 401), true)
            
            #line 15 "..\..\Views\Bill\Create.cshtml"
, Tuple.Create(Tuple.Create("", 421), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 421), false)
);

WriteLiteral(" />\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 478), Tuple.Create("\"", 560)
, Tuple.Create(Tuple.Create("", 484), Tuple.Create("/Scripts/Ext.DoubleGis.UI.BillPayments.js?", 484), true)
            
            #line 17 "..\..\Views\Bill\Create.cshtml"
, Tuple.Create(Tuple.Create("", 526), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 526), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 607), Tuple.Create("\"", 680)
, Tuple.Create(Tuple.Create("", 613), Tuple.Create("/Scripts/Ext.grid.CheckColumn.js?", 613), true)
            
            #line 18 "..\..\Views\Bill\Create.cshtml"
, Tuple.Create(Tuple.Create("", 646), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 646), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 727), Tuple.Create("\"", 825)
, Tuple.Create(Tuple.Create("", 733), Tuple.Create("/Scripts/Ext.DoubleGis.UI.RelatedOrdersSelectorControl.js?", 733), true)
            
            #line 19 "..\..\Views\Bill\Create.cshtml"
, Tuple.Create(Tuple.Create("", 791), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 791), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">
        Ext.namespace('Ext.DoubleGis.UI.Bill');
        Ext.DoubleGis.UI.Bill.Configurator = Ext.extend(Object, {
        Config: { 
            OrderId: -1, // идентификатор заказа для которого созается счет на оплату
            BillPaymentControlElement: null,
            PaymentsAmountSliderElement: null,
            OrderSumElement: null,
            RelatedOrderControlElement: null,
            RelatedOrderGridHeight: -1,
            RelatedOrderGridWidth: -1
        },
        PaymentsControl: null,
        RelatedOrderControl: null,
        Payments: null,
        RelatedOrders: null,
        PaymentTypes: {
                Single: ");

            
            #line 38 "..\..\Views\Bill\Create.cshtml"
                    Write((int)BillPaymentType.Single);

            
            #line default
            #line hidden
WriteLiteral(",\r\n                ByPlan: ");

            
            #line 39 "..\..\Views\Bill\Create.cshtml"
                    Write((int)BillPaymentType.ByPlan);

            
            #line default
            #line hidden
WriteLiteral(",\r\n                Custom: ");

            
            #line 40 "..\..\Views\Bill\Create.cshtml"
                    Write((int)BillPaymentType.Custom);

            
            #line default
            #line hidden
WriteLiteral("\r\n            },\r\n        constructor: function(config) {\r\n            Ext.apply(" +
"this.Config, config);\r\n            \r\n            this.PaymentsControl = new Ext." +
"DoubleGis.UI.BillPaymentsControl({\r\n                orderId: this.Config.OrderId" +
", \r\n                billPaymentControlElement: this.Config.BillPaymentControlEle" +
"ment,\r\n                paymentsAmountSliderElement: this.Config.PaymentsAmountSl" +
"iderElement,\r\n                orderSumElement: this.Config.OrderSumElement,\r\n   " +
"             paymentTypes: this.PaymentTypes\r\n            });\r\n\r\n            thi" +
"s.RelatedOrderControl = new Ext.DoubleGis.UI.RelatedOrdersSelectorControl({\r\n   " +
"             RelatedOrderControlElement: this.Config.RelatedOrderControlElement," +
"\r\n                GridHeight: this.Config.RelatedOrderGridHeight,\r\n             " +
"   GridWidth: this.Config.RelatedOrderGridWidth\r\n            });\r\n        },\r\n  " +
"      GetInitPaymentsInfo: function() {\r\n            // По-умолчанию считаем, чт" +
"о счета создаются \"одним платежом\"\r\n            var paymentType = this.PaymentTy" +
"pes.Single;\r\n            \r\n            if (Ext.getDom(\"PaymentTypeSingle\").check" +
"ed) {\r\n                paymentType = this.PaymentTypes.Single;\r\n            }\r\n " +
"           else if (Ext.getDom(\"PaymentTypeByPlan\").checked) {\r\n                " +
"paymentType = this.PaymentTypes.ByPlan;\r\n            }\r\n            this.Payment" +
"sControl.getInitPayments(paymentType);\r\n        },\r\n        GetRelatedOrdersForB" +
"illOrder: function () {\r\n            Ext.Ajax.request({\r\n                timeout" +
": 1200000,\r\n                method: \'POST\',\r\n                url: \'/Bill/GetRela" +
"tedOrdersInfoForCreateBill\',\r\n                params: { orderId: this.Config.Ord" +
"erId},\r\n                success: function(xhr, options) {\r\n                    v" +
"ar relatedOrders = Ext.decode(xhr.responseText);\r\n                    this.Relat" +
"edOrderControl.SetRelatedOrders(relatedOrders);\r\n                },\r\n           " +
"     failure: function(xhr, options) {\r\n                    Ext.MessageBox.show(" +
"{\r\n                        title: \'\',\r\n                        msg: xhr.response" +
"Text,\r\n                        buttons: Ext.MessageBox.OK,\r\n                    " +
"    width: 300,\r\n                        icon: Ext.MessageBox.ERROR\r\n           " +
"         });\r\n                },\r\n                scope: this\r\n            });\r\n" +
"        },\r\n        OnRadioClick: function() {\r\n            this.GetInitPayments" +
"Info();\r\n        },\r\n        OnBillCreateExecuteStage1: function(){\r\n           " +
" var paymentsResult = this.PaymentsControl.getPayments();\r\n            if (payme" +
"ntsResult.HasError) {\r\n                Ext.MessageBox.show({\r\n                  " +
"      title: \'\',\r\n                        msg: paymentsResult.ErrorMessage,\r\n   " +
"                     buttons: Ext.MessageBox.OK,\r\n                        width:" +
" 300,\r\n                        icon: Ext.MessageBox.ERROR\r\n                    }" +
");\r\n                return;\r\n            }\r\n            \r\n            this.Payme" +
"nts = paymentsResult.Payments;\r\n            if (paymentsResult.ConfirmationRequi" +
"red) {\r\n                Ext.MessageBox.show({\r\n                        title: \'\'" +
",\r\n                        msg: paymentsResult.ConfirmationMessage,\r\n           " +
"             buttons: Ext.MessageBox.OKCANCEL,\r\n                        width: 3" +
"00,\r\n                        icon: Ext.MessageBox.QUESTION,\r\n                   " +
"     fn: function (btn) {\r\n                            if (btn == \'ok\') {\r\n     " +
"                           this.OnBillCreateExecuteStage2();\r\n                  " +
"          }\r\n                        },\r\n                        scope: this\r\n  " +
"                  });\r\n            }\r\n            else {\r\n                 this." +
"OnBillCreateExecuteStage2();\r\n            }\r\n        },\r\n        OnBillCreateExe" +
"cuteStage2: function () {\r\n            this.RelatedOrders = this.RelatedOrderCon" +
"trol.GetOrders();\r\n            if (this.RelatedOrders.length > 0) \r\n            " +
"{\r\n                if (Ext.getDom(\"PaymentTypeByPlan\").checked) {\r\n             " +
"       var orderSum = this.PaymentsControl.getOrderSum();\r\n                    i" +
"f (orderSum == 0) {\r\n                        // заказ с нулевой суммой не может " +
"использоваться как шаблон для массового создания счетов на оплату, с использован" +
"ием рассрочки\r\n                        Ext.MessageBox.show({\r\n                  " +
"          title: \'\',\r\n                            msg: Ext.LocalizedResources.Bi" +
"llMassCreateOrderHasZeroPayablePlan,\r\n                            buttons: Ext.M" +
"essageBox.OK,\r\n                            width: 300,\r\n                        " +
"    icon: Ext.MessageBox.INFO\r\n                        });\r\n                    " +
"    return;\r\n                    }\r\n                }\r\n                \r\n       " +
"         Ext.MessageBox.show({\r\n                    title: \'\',\r\n                " +
"    msg: Ext.LocalizedResources.BillMassCreateConfirmMessage,\r\n                 " +
"   buttons: Ext.MessageBox.OKCANCEL,\r\n                    width: 300,\r\n         " +
"           icon: Ext.MessageBox.QUESTION,\r\n                    fn: function(btn)" +
" {\r\n                        if (btn == \'ok\') {\r\n                            this" +
".OnBillCreateExecuteFinal(this.Payments, this.RelatedOrders);\r\n                 " +
"       }\r\n                    },\r\n                    scope: this\r\n             " +
"   });\r\n            }\r\n            else {\r\n                this.OnBillCreateExec" +
"uteFinal(this.Payments, null);\r\n            }\r\n        },\r\n        OnBillCreateE" +
"xecuteFinal: function (payments, orders) {\r\n            window.Ext.Ajax.request(" +
"{\r\n                timeout: 1200000,\r\n                scope: this,\r\n            " +
"    method: \'POST\',\r\n                url: \'/Bill/SavePayments\',\r\n               " +
" params: {\r\n                     orderId: this.Config.OrderId, \r\n               " +
"      paymentsInfo: Ext.encode(payments), \r\n                     relatedOrders: " +
"Ext.encode(orders)\r\n                },\r\n                success: function (xhr, " +
"options) {\r\n                    window.close();\r\n                },\r\n           " +
"     failure: function (xhr, options) {\r\n                    Ext.MessageBox.show" +
"({\r\n                        title: \'\',\r\n                        msg: xhr.respons" +
"eText,\r\n                        buttons: window.Ext.MessageBox.OK,\r\n            " +
"            width: 300,\r\n                        icon: window.Ext.MessageBox.ERR" +
"OR\r\n                    });\r\n                }\r\n            });\r\n        },\r\n   " +
"     Configure: function () {\r\n            //Show error messages\r\n            if" +
" (Ext.getDom(\"Notifications\").innerHTML.trim() != \"\") {\r\n                Ext.get" +
"(\"Notifications\").addClass(\"Notifications\");\r\n            }\r\n            \r\n     " +
"       Ext.getDom(\"PaymentTypeSingle\").checked = true;\r\n            this.GetInit" +
"PaymentsInfo();\r\n            \r\n            Ext.get(\"PaymentTypeSingle\").on(\"clic" +
"k\", this.OnRadioClick, this);\r\n            Ext.get(\"PaymentTypeByPlan\").on(\"clic" +
"k\", this.OnRadioClick, this);\r\n            \r\n            if (Ext.getDom(\"IsMassB" +
"illCreateAvailable\").value == \'True\') {\r\n                this.GetRelatedOrdersFo" +
"rBillOrder();\r\n            }\r\n\r\n            //write eventhandlers for buttons\r\n " +
"           Ext.get(\"Cancel\").on(\"click\", function () { window.close(); });\r\n    " +
"        Ext.get(\"OK\").on(\"click\", function () {this.OnBillCreateExecuteStage1();" +
"}, this);\r\n        }\r\n        });\r\n        Ext.onReady(function () {\r\n          " +
"  var config = {\r\n                OrderId: Ext.getDom(\"OrderId\").value, // идент" +
"ификатор заказа для которого созается счет на оплату\r\n                BillPaymen" +
"tControlElement: \'billPaymentsControl\',\r\n                PaymentsAmountSliderEle" +
"ment: \'paymentsAmountSlider\',\r\n                OrderSumElement: \'orderSum\', \r\n  " +
"              RelatedOrderControlElement: \'massCreateBillsControl\',\r\n           " +
"     RelatedOrderGridHeight: 110,\r\n                RelatedOrderGridWidth: 760\r\n " +
"           };\r\n            var billConfigurator = new Ext.DoubleGis.UI.Bill.Conf" +
"igurator(config);\r\n            billConfigurator.Configure();\r\n    });\r\n    </scr" +
"ipt>\r\n    \r\n    <div");

WriteLiteral(" style=\"display: none\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 228 "..\..\Views\Bill\Create.cshtml"
   Write(Html.HiddenFor(m => m.IsMassBillCreateAvailable));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 229 "..\..\Views\Bill\Create.cshtml"
   Write(Html.HiddenFor(m => m.OrderId));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div");

WriteLiteral(" style=\"padding: 5px 0 0 10px\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 232 "..\..\Views\Bill\Create.cshtml"
   Write(BLResources.BillPaymentType);

            
            #line default
            #line hidden
WriteLiteral("\r\n        <table");

WriteLiteral(" width=\"80%\"");

WriteLiteral(">\r\n            <colgroup>\r\n                <col />\r\n                <col />\r\n    " +
"            <col />\r\n                <col />\r\n            </colgroup>\r\n         " +
"   <tbody>\r\n                <tr>\r\n                    <td");

WriteLiteral(" colspan=\"4\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" style=\"height: 15px;display: none;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 244 "..\..\Views\Bill\Create.cshtml"
                       Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n                        </div>\r\n                    </td>\r\n                </tr" +
">\r\n                <tr>\r\n                    <td>\r\n");

WriteLiteral("                        ");

            
            #line 250 "..\..\Views\Bill\Create.cshtml"
                   Write(Html.RadioButtonFor(m => m.PaymentType, BillPaymentType.Single, new Dictionary<string, object> { { "id", "PaymentTypeSingle" } } ));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        <l" +
"abel");

WriteLiteral(" style=\"font-weight: bold\"");

WriteLiteral(" for=\"PaymentTypeSingle\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 254 "..\..\Views\Bill\Create.cshtml"
                       Write(BLResources.BillPaymentTypeSingle);

            
            #line default
            #line hidden
WriteLiteral("\r\n                        </label>\r\n                        <br />\r\n             " +
"           <div");

WriteLiteral(" style=\"color: #444444; padding-top: 5px\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 258 "..\..\Views\Bill\Create.cshtml"
                       Write(BLResources.BillPaymentTypeSingleLegend);

            
            #line default
            #line hidden
WriteLiteral("\r\n                        </div>\r\n                    </td>\r\n                    " +
"<td>\r\n");

WriteLiteral("                        ");

            
            #line 262 "..\..\Views\Bill\Create.cshtml"
                   Write(Html.RadioButtonFor(m => m.PaymentType, BillPaymentType.ByPlan, new Dictionary<string, object> { { "id", "PaymentTypeByPlan" } } ));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        <l" +
"abel");

WriteLiteral(" style=\"font-weight: bold\"");

WriteLiteral(" for=\"PaymentTypeByPlan\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 266 "..\..\Views\Bill\Create.cshtml"
                       Write(BLResources.BillPaymentTypeByPlan);

            
            #line default
            #line hidden
WriteLiteral("\r\n                        </label>\r\n                        <br />\r\n             " +
"           <div");

WriteLiteral(" style=\"color: #444444; padding-top: 5px\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 270 "..\..\Views\Bill\Create.cshtml"
                       Write(BLResources.BillPaymentTypeByPlanLegend);

            
            #line default
            #line hidden
WriteLiteral("\r\n                        </div>\r\n                    </td>\r\n                </tr" +
">\r\n            </tbody>\r\n        </table>\r\n    </div>\r\n    <div");

WriteLiteral(" style=\"padding: 10px\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 278 "..\..\Views\Bill\Create.cshtml"
   Write(MetadataResources.PaymentsAmount);

            
            #line default
            #line hidden
WriteLiteral("\r\n     </div>\r\n    <div");

WriteLiteral(" id=\"billsAmountWarning\"");

WriteLiteral(" style=\"padding-left: 100px\"");

WriteLiteral("></div>\r\n    <div");

WriteLiteral(" id=\"paymentsAmountSlider\"");

WriteLiteral("></div>\r\n    <div");

WriteLiteral(" id=\"billPaymentsControl\"");

WriteLiteral(" style=\"margin: 10px\"");

WriteLiteral("></div>\r\n    <span");

WriteLiteral(" style=\"float: right; padding: 0 10px;\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 284 "..\..\Views\Bill\Create.cshtml"
   Write(BLResources.OrderSum);

            
            #line default
            #line hidden
WriteLiteral(", ");

            
            #line 284 "..\..\Views\Bill\Create.cshtml"
                          Write(ViewData.GetErmBaseCurrencySymbol());

            
            #line default
            #line hidden
WriteLiteral("\r\n        <span");

WriteLiteral(" id=\"orderSum\"");

WriteLiteral("></span>\r\n    </span>\r\n    <div");

WriteLiteral(" id=\"massBillsCreate\"");

WriteLiteral(" style=\"clear: both;\"");

WriteLiteral(">\r\n       <div");

WriteLiteral(" style=\"padding-left: 10px\"");

WriteLiteral(">\r\n            <span");

WriteLiteral(" style=\"font-weight: bold\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 290 "..\..\Views\Bill\Create.cshtml"
           Write(BLResources.MassCreateBillsOperationName);

            
            #line default
            #line hidden
WriteLiteral("\r\n             </span>\r\n        </div>\r\n        <div");

WriteLiteral(" id=\"massCreateBillsControl\"");

WriteLiteral(" style=\"margin: 10px\"");

WriteLiteral("/> \r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
