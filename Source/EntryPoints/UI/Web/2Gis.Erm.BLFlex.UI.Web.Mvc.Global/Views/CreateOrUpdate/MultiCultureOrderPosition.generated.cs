﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/MultiCultureOrderPosition.cshtml")]
    public partial class MultiCultureOrderPosition : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.MultiCultureOrderPositionViewModel>
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

WriteAttribute("href", Tuple.Create(" href=\"", 215), Tuple.Create("\"", 285)
, Tuple.Create(Tuple.Create("", 222), Tuple.Create("/Content/ext-ux-treegrid.css?", 222), true)
            
            #line 10 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 251), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 251), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 333), Tuple.Create("\"", 402)
, Tuple.Create(Tuple.Create("", 340), Tuple.Create("/Content/order-position.css?", 340), true)
            
            #line 11 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 368), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 368), false)
);

WriteLiteral(" />\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 425), Tuple.Create("\"", 499)
, Tuple.Create(Tuple.Create("", 431), Tuple.Create("/Scripts/Ext.ux.TreeGridSorter.js?", 431), true)
            
            #line 13 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 465), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 465), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 546), Tuple.Create("\"", 627)
, Tuple.Create(Tuple.Create("", 552), Tuple.Create("/Scripts/Ext.ux.TreeGridColumnResizer.js?", 552), true)
            
            #line 14 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 593), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 593), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 674), Tuple.Create("\"", 748)
, Tuple.Create(Tuple.Create("", 680), Tuple.Create("/Scripts/Ext.ux.TreeGridNodeUI.js?", 680), true)
            
            #line 15 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 714), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 714), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 795), Tuple.Create("\"", 869)
, Tuple.Create(Tuple.Create("", 801), Tuple.Create("/Scripts/Ext.ux.TreeGridLoader.js?", 801), true)
            
            #line 16 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 835), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 835), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 916), Tuple.Create("\"", 991)
, Tuple.Create(Tuple.Create("", 922), Tuple.Create("/Scripts/Ext.ux.TreeGridColumns.js?", 922), true)
            
            #line 17 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 957), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 957), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1038), Tuple.Create("\"", 1106)
, Tuple.Create(Tuple.Create("", 1044), Tuple.Create("/Scripts/Ext.ux.TreeGrid.js?", 1044), true)
            
            #line 18 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 1072), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1072), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1159), Tuple.Create("\"", 1242)
, Tuple.Create(Tuple.Create("", 1165), Tuple.Create("/Scripts/Ext.DoubleGis.UI.OrderPosition.js?", 1165), true)
            
            #line 20 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 1208), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1208), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1289), Tuple.Create("\"", 1386)
, Tuple.Create(Tuple.Create("", 1295), Tuple.Create("/Scripts/Ext.DoubleGis.UI.OrderPosition.BusinessLogic.js?", 1295), true)
            
            #line 21 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 1352), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1352), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1433), Tuple.Create("\"", 1530)
, Tuple.Create(Tuple.Create("", 1439), Tuple.Create("/Scripts/Ext.DoubleGis.UI.OrderPosition.LinkingObject.js?", 1439), true)
            
            #line 22 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 1496), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1496), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1577), Tuple.Create("\"", 1684)
, Tuple.Create(Tuple.Create("", 1583), Tuple.Create("/Scripts/Ext.DoubleGis.UI.OrderPosition.AdvertisementController.js?", 1583), true)
            
            #line 23 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
    , Tuple.Create(Tuple.Create("", 1650), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1650), false)
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

WriteAttribute("title", Tuple.Create(" title=\"", 2083), Tuple.Create("\"", 2119)
            
            #line 45 "..\..\Views\CreateOrUpdate\MultiCultureOrderPosition.cshtml"
, Tuple.Create(Tuple.Create("", 2091), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 2091), false)
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
                   Write(Html.TemplateField(m => m.PricePosition, FieldFlex.lone, new LookupSettings { ShowReadOnlyCard = true, EntityName = EntityName.PricePosition, ExtendedInfo = "filterToParent=true", ParentEntityName = EntityName.Price, ParentIdPattern = "PriceId" }));

            
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
