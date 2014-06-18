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

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Views.CreateOrUpdate.Emirates
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Emirates/FirmAddress.cshtml")]
    public partial class FirmAddress : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Emirates.EmiratesFirmAddressViewModel>
    {
        public FirmAddress()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\CreateOrUpdate\Emirates\FirmAddress.cshtml"
  
    Layout = "../../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">

        // saving of for additionalFirmAddressServices control
        window.InitPage = function ()
        {
            this.on('beforepost', function ()
            {
                this.genericSave(this.submitMode);
                return false;
            });

            Ext.apply(this,
                {
                    genericSave: function (submitMode)
                    {
                        var card = this;
                        var onSuccess = function ()
                        {
                            card.refresh();
                        };

                        var onFailure = function ()
                        {
                            // TODO {all, 18.12.2013}: возможно некоректное отображение диакритики
                            // TODO {all, 18.12.2013}: alert можно заменить на ext'овый messagebox
                            // TODO {all, 18.12.2013}: ресурс можно перенести в ClientResourceStorage
                            alert('");

            
            #line 35 "..\..\Views\CreateOrUpdate\Emirates\FirmAddress.cshtml"
                              Write(BLResources.SaveError);

            
            #line default
            #line hidden
WriteLiteral(@"');
                            card.Items.Toolbar.enable();
                        };

                        var additionalFirmAddressServicesIFrame = Ext.getDom('AdditionalFirmAddressServices_frame');
                        if (additionalFirmAddressServicesIFrame)
                        {
                            var contentWindow = additionalFirmAddressServicesIFrame.contentWindow;
                            contentWindow.Ext.DoubleGis.UI.AdditionalFirmServicesControlInstance.Save(onSuccess, onFailure);
                        }
                        else
                            onSuccess();
                    }
                });
        }
    </script>
");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 55 "..\..\Views\CreateOrUpdate\Emirates\FirmAddress.cshtml"
Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"Div1\"");

WriteAttribute("title", Tuple.Create(" title=\"", 2046), Tuple.Create("\"", 2082)
            
            #line 56 "..\..\Views\CreateOrUpdate\Emirates\FirmAddress.cshtml"
, Tuple.Create(Tuple.Create("", 2054), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 2054), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 58 "..\..\Views\CreateOrUpdate\Emirates\FirmAddress.cshtml"
       Write(Html.TemplateField(m => m.Firm, FieldFlex.lone, new LookupSettings { EntityName = EntityName.Firm, ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 61 "..\..\Views\CreateOrUpdate\Emirates\FirmAddress.cshtml"
       Write(Html.TemplateField(m => m.Address, FieldFlex.lone, new Dictionary<string, object> { { "readonly", "readonly" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 64 "..\..\Views\CreateOrUpdate\Emirates\FirmAddress.cshtml"
       Write(Html.TemplateField(m => m.PoBox, FieldFlex.lone, new Dictionary<string, object> { { "readonly", "readonly" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 67 "..\..\Views\CreateOrUpdate\Emirates\FirmAddress.cshtml"
       Write(Html.TemplateField(m => m.PaymentMethods, FieldFlex.lone, new Dictionary<string, object> { { "readonly", "readonly" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 70 "..\..\Views\CreateOrUpdate\Emirates\FirmAddress.cshtml"
       Write(Html.TemplateField(m => m.WorkingTime, FieldFlex.lone, new Dictionary<string, object> { { "readonly", "readonly" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 73 "..\..\Views\CreateOrUpdate\Emirates\FirmAddress.cshtml"
       Write(Html.TemplateField(m => m.ClosedForAscertainment, FieldFlex.twins, new Dictionary<string, object> { { "disabled", "disabled" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
