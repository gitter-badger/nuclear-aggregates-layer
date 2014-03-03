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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Russia/LegalPersonProfile.cshtml")]
    public partial class LegalPersonProfile : System.Web.Mvc.WebViewPage<Models.Russia.LegalPersonProfileViewModel>
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
                        var optionToDisable = operatesValues[disabledValues[i]];
                        if(optionToDisable != undefined) {
                            optionToDisable.disabled = true;
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

            
            #line 31 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
Write(Html.HiddenFor(m => m.LegalPersonType));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 32 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 1263), Tuple.Create("\"", 1299)
            
            #line 34 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
, Tuple.Create(Tuple.Create("", 1271), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 1271), false)
);

WriteLiteral(">    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 36 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.Name, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 40 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.LegalPerson, FieldFlex.lone, new LookupSettings { EntityName = EntityName.LegalPerson, ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 44 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.DocumentsDeliveryAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 48 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.RecipientName, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 49 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.DocumentsDeliveryMethod, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 53 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PersonResponsibleForDocuments, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 54 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.EmailForAccountingDocuments, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 58 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.AdditionalEmail, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 62 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PostAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 66 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PaymentEssentialElements, FieldFlex.lone, new Dictionary<string, object> { { "rows", 5 } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        \r\n");

WriteLiteral("        ");

            
            #line 69 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
   Write(Html.SectionHead("sectionInfo", @BLResources.TitleEmployeeInformation));

            
            #line default
            #line hidden
WriteLiteral("\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 72 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.ChiefNameInNominative, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 73 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PositionInNominative, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 77 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.ChiefNameInGenitive, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 78 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PositionInGenitive, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 82 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.Phone, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 83 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.OperatesOnTheBasisInGenitive, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 87 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.CertificateNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 88 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.CertificateDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 92 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.WarrantyNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 96 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.WarrantyBeginDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 97 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.WarrantyEndDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 101 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.BargainNumber, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 105 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.BargainBeginDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 106 "..\..\Views\CreateOrUpdate\Russia\LegalPersonProfile.cshtml"
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
