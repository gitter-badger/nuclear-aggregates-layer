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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/PricePosition.cshtml")]
    public partial class PricePosition : System.Web.Mvc.WebViewPage<Models.PricePositionViewModel>
    {
        public PricePosition()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\CreateOrUpdate\PricePosition.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">
        window.InitPage = function ()
        {
            this.on(""beforebuild"", function (card)
            {
                card.CopyPricePosition = function ()
                {
                    var entityId = Ext.getDom(""Id"").value;
                    var params = ""dialogWidth:"" + 500 + ""px; dialogHeight:"" + 200 + ""px; status:yes; scroll:no;resizable:no;"";
                    var url = '/PricePosition/Copy/' + entityId;

                    this.Items.Toolbar.disable();
                    window.showModalDialog(url, null, params);
                    this.Items.Toolbar.enable();
                };
            });

            var setupAmountVisibility = function (amountSpecificationMode)
            {
                Ext.get(""Amount"").dom.parentNode.style.display = amountSpecificationMode == 'FixedValue' ? 'block' : 'none';
            };

            window.Card.on(""afterbuild"", function (card)
            {
                Ext.get(""AmountSpecificationMode"").on(""change"", function (args, sender)
                {
                    setupAmountVisibility(sender.value);
                });

                setupAmountVisibility(Ext.getDom(""AmountSpecificationMode"").value);
            });

        };
    </script>
");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 1495), Tuple.Create("\"", 1531)
            
            #line 47 "..\..\Views\CreateOrUpdate\PricePosition.cshtml"
, Tuple.Create(Tuple.Create("", 1503), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 1503), false)
);

WriteLiteral(">\r\n");

            
            #line 48 "..\..\Views\CreateOrUpdate\PricePosition.cshtml"
        
            
            #line default
            #line hidden
            
            #line 48 "..\..\Views\CreateOrUpdate\PricePosition.cshtml"
         if (Model != null)
        {
            
            
            #line default
            #line hidden
            
            #line 50 "..\..\Views\CreateOrUpdate\PricePosition.cshtml"
       Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
            
            #line 50 "..\..\Views\CreateOrUpdate\PricePosition.cshtml"
                                      
            
            
            #line default
            #line hidden
            
            #line 51 "..\..\Views\CreateOrUpdate\PricePosition.cshtml"
       Write(Html.HiddenFor(m => m.IsPositionControlledByAmount));

            
            #line default
            #line hidden
            
            #line 51 "..\..\Views\CreateOrUpdate\PricePosition.cshtml"
                                                                
            
            
            #line default
            #line hidden
            
            #line 52 "..\..\Views\CreateOrUpdate\PricePosition.cshtml"
       Write(Html.HiddenFor(m => m.IsRatePricePositionAvailable));

            
            #line default
            #line hidden
            
            #line 52 "..\..\Views\CreateOrUpdate\PricePosition.cshtml"
                                                                
        }

            
            #line default
            #line hidden
WriteLiteral("        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 55 "..\..\Views\CreateOrUpdate\PricePosition.cshtml"
       Write(Html.TemplateField(m => m.Price, FieldFlex.twins, new LookupSettings {EntityName = EntityName.Price, SearchFormFilterInfo = "IsDeleted=false && IsActive=true"}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 58 "..\..\Views\CreateOrUpdate\PricePosition.cshtml"
       Write(Html.TemplateField(m => m.Position, FieldFlex.twins, new LookupSettings {EntityName = EntityName.Position, SearchFormFilterInfo = "IsDeleted=false && IsActive=true", ExtendedInfo = "isSupportedByExport=true"}));

            
            #line default
            #line hidden
WriteLiteral("\r\n            <div");

WriteLiteral(" class=\"display-wrapper field-wrapper twins\"");

WriteLiteral(" id=\"Amount-wrapper\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"label-wrapper\"");

WriteLiteral(" id=\"Amount-caption\"");

WriteLiteral(">\r\n");

WriteLiteral("                    ");

            
            #line 61 "..\..\Views\CreateOrUpdate\PricePosition.cshtml"
               Write(Html.LabelFor(m => m.AmountSpecificationMode));

            
            #line default
            #line hidden
WriteLiteral("<span");

WriteLiteral(" class=\"req\"");

WriteLiteral(">*</span>:\r\n                </div>\r\n                <div");

WriteLiteral(" class=\"input-wrapper\"");

WriteLiteral(" style=\"width: 50px; float: right; margin-right: 5px; margin-top: 2px\"");

WriteLiteral(">\r\n");

WriteLiteral("                    ");

            
            #line 64 "..\..\Views\CreateOrUpdate\PricePosition.cshtml"
               Write(Html.TextBoxFor(m => m.Amount, new Dictionary<string, object> {{"class", "inputfields"}, {"style", "width:50px"}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </div>\r\n                <div");

WriteLiteral(" class=\"input-wrapper\"");

WriteLiteral(" style=\"margin-top: 3px\"");

WriteLiteral(">\r\n");

WriteLiteral("                    ");

            
            #line 67 "..\..\Views\CreateOrUpdate\PricePosition.cshtml"
               Write(Html.DropDownListFor(m => m.AmountSpecificationMode, EnumResources.ResourceManager, new Dictionary<string, object> {{"class", "inputfields"}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </div>\r\n                <div");

WriteLiteral(" style=\"clear: both\"");

WriteLiteral(">\r\n                </div>\r\n                <div");

WriteLiteral(" style=\"float: right; margin-right: 9px\"");

WriteLiteral(">\r\n");

WriteLiteral("                    ");

            
            #line 72 "..\..\Views\CreateOrUpdate\PricePosition.cshtml"
               Write(Html.ValidationMessageFor(m => m.Amount));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </div>\r\n                <div");

WriteLiteral(" style=\"display: none\"");

WriteLiteral(">\r\n");

WriteLiteral("                    ");

            
            #line 75 "..\..\Views\CreateOrUpdate\PricePosition.cshtml"
               Write(Html.LabelFor(m => m.Amount));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </div>\r\n            </div>\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 81 "..\..\Views\CreateOrUpdate\PricePosition.cshtml"
       Write(Html.TemplateField(m => m.Cost, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 82 "..\..\Views\CreateOrUpdate\PricePosition.cshtml"
       Write(Html.TemplateField(m => m.Currency, FieldFlex.twins, new LookupSettings {EntityName = EntityName.Currency, SearchFormFilterInfo = "IsDeleted=false && IsActive=true", ReadOnly = true}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 86 "..\..\Views\CreateOrUpdate\PricePosition.cshtml"
       Write(Html.TemplateField(m => m.MinAdvertisementAmount, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral(" \r\n");

WriteLiteral("            ");

            
            #line 87 "..\..\Views\CreateOrUpdate\PricePosition.cshtml"
       Write(Html.TemplateField(m => m.MaxAdvertisementAmount, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral(" \r\n        </div>\r\n        \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 91 "..\..\Views\CreateOrUpdate\PricePosition.cshtml"
       Write(Html.TemplateField(m => m.RatePricePosition, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
