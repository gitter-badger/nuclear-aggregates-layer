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

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Views.CreateOrUpdate
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc.Html;

    using DoubleGis.Erm.BLCore.Resources.Server.Properties;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    using NuClear.Model.Common.Entities;

    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/MultiCultureOrderPosition.cshtml")]
    public partial class MultiCultureOrderPosition : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BL.UI.Web.Mvc.Models.OrderPositionViewModel>
    {
        public MultiCultureOrderPosition()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 4 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 192), Tuple.Create("\"", 247)
, Tuple.Create(Tuple.Create("", 199), Tuple.Create("/Content/ext-ux-treegrid.css?", 199), true)
            
            #line 10 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 228), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 228), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 295), Tuple.Create("\"", 349)
, Tuple.Create(Tuple.Create("", 302), Tuple.Create("/Content/order-position.css?", 302), true)
            
            #line 11 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 330), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 330), false)
);

WriteLiteral(" />\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 372), Tuple.Create("\"", 431)
, Tuple.Create(Tuple.Create("", 378), Tuple.Create("/Scripts/Ext.ux.TreeGridSorter.js?", 378), true)
            
            #line 13 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 412), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 412), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 478), Tuple.Create("\"", 544)
, Tuple.Create(Tuple.Create("", 484), Tuple.Create("/Scripts/Ext.ux.TreeGridColumnResizer.js?", 484), true)
            
            #line 14 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 525), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 525), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 591), Tuple.Create("\"", 650)
, Tuple.Create(Tuple.Create("", 597), Tuple.Create("/Scripts/Ext.ux.TreeGridNodeUI.js?", 597), true)
            
            #line 15 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 631), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 631), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 697), Tuple.Create("\"", 756)
, Tuple.Create(Tuple.Create("", 703), Tuple.Create("/Scripts/Ext.ux.TreeGridLoader.js?", 703), true)
            
            #line 16 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 737), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 737), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 803), Tuple.Create("\"", 863)
, Tuple.Create(Tuple.Create("", 809), Tuple.Create("/Scripts/Ext.ux.TreeGridColumns.js?", 809), true)
            
            #line 17 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 844), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 844), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 910), Tuple.Create("\"", 963)
, Tuple.Create(Tuple.Create("", 916), Tuple.Create("/Scripts/Ext.ux.TreeGrid.js?", 916), true)
            
            #line 18 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 944), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 944), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1016), Tuple.Create("\"", 1084)
, Tuple.Create(Tuple.Create("", 1022), Tuple.Create("/Scripts/Ext.DoubleGis.UI.OrderPosition.js?", 1022), true)
            
            #line 20 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 1065), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1065), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1131), Tuple.Create("\"", 1213)
, Tuple.Create(Tuple.Create("", 1137), Tuple.Create("/Scripts/Ext.DoubleGis.UI.OrderPosition.BusinessLogic.js?", 1137), true)
            
            #line 21 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 1194), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1194), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1260), Tuple.Create("\"", 1342)
, Tuple.Create(Tuple.Create("", 1266), Tuple.Create("/Scripts/Ext.DoubleGis.UI.OrderPosition.LinkingObject.js?", 1266), true)
            
            #line 22 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 1323), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1323), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1389), Tuple.Create("\"", 1481)
, Tuple.Create(Tuple.Create("", 1395), Tuple.Create("/Scripts/Ext.DoubleGis.UI.OrderPosition.AdvertisementController.js?", 1395), true)
            
            #line 23 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
    , Tuple.Create(Tuple.Create("", 1462), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 1462), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(@">
        .display-wrapper .captionRadioDiv
        {
            padding-top: 0px;
        }
        
        div.label-wrapper
        {
            width: 140px;
        }
        
        div.Tab
        {
            padding: 0px;
        }
    </style>
");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 1880), Tuple.Create("\"", 1916)
            
            #line 45 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 1888), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 1888), false)
);

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 46 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
   Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 47 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
   Write(Html.HiddenFor(m => m.OrderId));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 48 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
   Write(Html.HiddenFor(m => m.OrderFirmId));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 49 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
   Write(Html.HiddenFor(m => m.PriceId));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 50 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
   Write(Html.HiddenFor(m => m.OrganizationUnitId));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 51 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
   Write(Html.HiddenFor(m => m.IsLocked));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 52 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
   Write(Html.HiddenFor(m => m.IsComposite));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 53 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
   Write(Html.HiddenFor(m => m.DiscountSum));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 54 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
   Write(Html.HiddenFor(m => m.DiscountPercent));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 55 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
   Write(Html.HiddenFor(m => m.AdvertisementsJson));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 56 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
   Write(Html.HiddenFor(m => m.MoneySignificantDigitsNumber));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <table");

WriteLiteral(" style=\"height: 100%; table-layout: fixed\"");

WriteLiteral(">\r\n            <tr");

WriteLiteral(" style=\"height: 250px\"");

WriteLiteral(">\r\n                <td");

WriteLiteral(" style=\"padding-right: 10px\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 61 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
                   Write(Html.TemplateField(m => m.PricePosition, FieldFlex.lone, new LookupSettings { ShowReadOnlyCard = true, EntityName = EntityType.Instance.PricePosition(), ExtendedInfo = "filterToParent=true&orderId={OrderId}", ParentEntityName = EntityType.Instance.Price(), ParentIdPattern = "PriceId" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n                    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 64 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
                   Write(Html.TemplateField(m => m.PricePerUnit, FieldFlex.lone, new Dictionary<string, object> { { "class", "inputfields readonly" }, { "readonly", "true" }, { "maxlength", "18" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n                    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 67 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
                   Write(Html.TemplateField(m => m.PricePerUnitWithVat, FieldFlex.lone, new Dictionary<string, object> { { "class", "inputfields readonly" }, { "maxlength", "18" }, { "readonly", "true" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n                    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 70 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
                   Write(Html.TemplateField(m => m.Amount, FieldFlex.lone, new Dictionary<string, object> { { "class", "inputfields" }, { "maxlength", "8" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n                    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 73 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
                   Write(Html.TemplateField(m => m.ShipmentPlan, FieldFlex.lone, new Dictionary<string, object> { { "class", "inputfields readonly"}, { "readonly", "true" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n                    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 76 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
                   Write(Html.TemplateField(m => m.PayablePrice, FieldFlex.lone, new Dictionary<string, object> { { "class", "inputfields readonly" }, { "readonly", "true" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n                    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 79 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
                   Write(Html.TemplateField(m => m.PayablePlan, FieldFlex.lone, new Dictionary<string, object> { { "id", "PayablePlan" }, { "class", "inputfields readonly" }, { "readonly", "true" }}));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n                </td>\r\n                <td");

WriteLiteral(" style=\"padding-left: 10px; padding-top: 14px; vertical-align: top\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(" id=\"discountPercentOuter\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" class=\"display-wrapper field-wrapper lone\"");

WriteLiteral(">\r\n                            <div");

WriteLiteral(" class=\"label-wrapper captionRadioDiv\"");

WriteLiteral(">\r\n                                <span>\r\n");

WriteLiteral("                                    ");

            
            #line 87 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
                               Write(Html.RadioButtonFor(m => m.CalculateDiscountViaPercent, true, new Dictionary<string, object> { { "id", "CalculateDiscountViaPercentTrue" } }));

            
            #line default
            #line hidden
WriteLiteral("</span>\r\n");

WriteLiteral("                                    ");

            
            #line 88 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
                               Write(Html.Label("CalculateDiscountViaPercentTrue", MetadataResources.DiscountPercent));

            
            #line default
            #line hidden
WriteLiteral("\r\n                            </div>\r\n                            <div");

WriteLiteral(" class=\"input-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                                ");

            
            #line 91 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
                           Write(Html.TextBox("DiscountPercentText", string.Empty, new Dictionary<string, object> { { "class", "inputfields" }, { "maxlength", "18" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                ");

            
            #line 92 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
                           Write(Html.ValidationMessageFor(m => m.DiscountPercent, null, new Dictionary<string, object> { { "class", "error" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                            </div>\r\n                        </div>\r\n           " +
"         </div>\r\n                    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(" id=\"discountSumOuter\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" class=\"display-wrapper field-wrapper lone\"");

WriteLiteral(">\r\n                            <div");

WriteLiteral(" class=\"label-wrapper captionRadioDiv\"");

WriteLiteral(">\r\n                                <span>\r\n");

WriteLiteral("                                    ");

            
            #line 100 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
                               Write(Html.RadioButtonFor(m => m.CalculateDiscountViaPercent, false, new Dictionary<string, object> { { "id", "CalculateDiscountViaPercentFalse" } }));

            
            #line default
            #line hidden
WriteLiteral("</span>\r\n");

WriteLiteral("                                    ");

            
            #line 101 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
                               Write(Html.Label("CalculateDiscountViaPercentFalse", MetadataResources.DiscountSum));

            
            #line default
            #line hidden
WriteLiteral("\r\n                            </div>\r\n                            <div");

WriteLiteral(" class=\"input-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                                ");

            
            #line 104 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
                           Write(Html.TextBox("DiscountSumText", string.Empty, new Dictionary<string, object> { { "class", "inputfields" }, { "maxlength", "18" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                ");

            
            #line 105 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
                           Write(Html.ValidationMessageFor(m => m.DiscountSum, null, new Dictionary<string, object> { { "class", "error" } } ));

            
            #line default
            #line hidden
WriteLiteral("\r\n                            </div>\r\n                        </div>\r\n           " +
"         </div>\r\n                    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" class=\"display-wrapper field-wrapper lone\"");

WriteLiteral(">\r\n                            <div");

WriteLiteral(" class=\"label-wrapper captionRadioDiv\"");

WriteLiteral(">\r\n");

WriteLiteral("                                ");

            
            #line 112 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
                           Write(Html.Label(MetadataResources.Platform));

            
            #line default
            #line hidden
WriteLiteral("\r\n                            </div>\r\n                            <div");

WriteLiteral(" class=\"input-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                                ");

            
            #line 115 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
                           Write(Html.TextBox("Platform", string.Empty, new Dictionary<string, object> { { "class", "inputfields readonly" }, { "maxlength", "18" }, { "readonly", "true" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                            </div>\r\n                        </div>\r\n           " +
"         </div>\r\n                    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 120 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
                   Write(Html.TemplateField(m => m.Comment, FieldFlex.lone, new Dictionary<string, object> { { "class", "inputfields" }, { "style", "height:73px" }, { "rows", "5" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n                </td>\r\n            </tr>\r\n         " +
"   <tr>\r\n                <td");

WriteLiteral(" colspan=\"2\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" id=\'linkingObjectContainer\'");

WriteLiteral(" style=\"width: 100%; height: 100%; overflow: hidden;\r\n                        mar" +
"gin: 0px 5px 5px 5px\"");

WriteLiteral(">\r\n                    </div>\r\n                </td>\r\n            </tr>\r\n        " +
"</table>\r\n    </div>\r\n");

});

WriteLiteral("\r\n");

            
            #line 135 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
Write(RenderBody());

            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591
