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

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Views.CreateOrUpdate.Russia
{
    using System;
    using System.Collections.Generic;
    using System.Web.Helpers;
    using System.Web.Mvc.Html;

    using DoubleGis.Erm.BLCore.Resources.Server.Properties;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    using NuClear.Model.Common.Entities;

    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Russia/LegalPersonProfile.cshtml")]
    public partial class LegalPersonProfile : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia.LegalPersonProfileViewModel>
    {
        public LegalPersonProfile()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 4 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
  
    Layout = "../../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("    \r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">
            window.InitPage = function() {
                window.Card.on(""afterbuild"", function() {
                var operatesValues = document.getElementById('OperatesOnTheBasisInGenitive').getElementsByTagName('option');
                var disabledValues = ");

            
            #line 14 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
                                Write(Html.Raw(Json.Encode(Model.DisabledDocuments)));

            
            #line default
            #line hidden
WriteLiteral(@";
                for (var i = 0; i < disabledValues.length; i++) {
                    for (var j = 0; j < operatesValues.length; j++) {
                        if (operatesValues[j].value == disabledValues[i] || (disabledValues[i] == 'Undefined' && operatesValues[j].value == '')) {
                            operatesValues[j].disabled = true;
                            break;
                        }
                    }
                }

                    var documentsDeliveryMethodCombobox = document.getElementById('DocumentsDeliveryMethod');
                    Ext.DoubleGis.Global.Helpers.HideComboBoxItemsByValues(documentsDeliveryMethodCombobox, ['ByCourier']);
                });
            };
    </script>   
");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("    \r\n");

WriteLiteral("    ");

            
            #line 33 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
Write(Html.HiddenFor(m => m.LegalPersonType));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 34 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 1424), Tuple.Create("\"", 1460)
            
            #line 36 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
, Tuple.Create(Tuple.Create("", 1432), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 1432), false)
);

WriteLiteral(">    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 38 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.Name, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 42 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.LegalPerson, FieldFlex.lone, new LookupSettings { EntityName = EntityType.Instance.LegalPerson(), ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 46 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.DocumentsDeliveryAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 50 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.RecipientName, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 51 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.DocumentsDeliveryMethod, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 55 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PersonResponsibleForDocuments, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 56 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.EmailForAccountingDocuments, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 60 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.Email, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 64 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PostAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 68 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PaymentEssentialElements, FieldFlex.lone, new Dictionary<string, object> { { "rows", 5 } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        \r\n");

WriteLiteral("        ");

            
            #line 71 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
   Write(Html.SectionHead("sectionInfo", @BLResources.TitleEmployeeInformation));

            
            #line default
            #line hidden
WriteLiteral("\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 74 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.ChiefNameInNominative, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 75 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PositionInNominative, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 79 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.ChiefFullNameInNominative, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 83 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.ChiefNameInGenitive, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 84 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PositionInGenitive, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 88 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.Phone, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 89 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.OperatesOnTheBasisInGenitive, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 93 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.CertificateNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 94 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.CertificateDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 98 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.WarrantyNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 102 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.WarrantyBeginDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 103 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.WarrantyEndDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 107 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.BargainNumber, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 111 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.BargainBeginDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 112 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.BargainEndDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

WriteLiteral("\r\n    ");

        }
    }
}
#pragma warning restore 1591
