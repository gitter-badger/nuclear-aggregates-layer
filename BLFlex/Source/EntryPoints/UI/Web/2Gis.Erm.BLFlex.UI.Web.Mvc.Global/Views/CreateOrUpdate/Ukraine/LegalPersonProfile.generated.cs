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

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Views.CreateOrUpdate.Ukraine
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Ukraine/LegalPersonProfile.cshtml")]
    public partial class LegalPersonProfile : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Ukraine.UkraineLegalPersonProfileViewModel>
    {
        public LegalPersonProfile()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
  
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

            
            #line 13 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
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
            });
        };
    </script>   
");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("    \r\n");

WriteLiteral("    ");

            
            #line 29 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 1131), Tuple.Create("\"", 1167)
            
            #line 31 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
, Tuple.Create(Tuple.Create("", 1139), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 1139), false)
);

WriteLiteral(">    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 33 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.Name, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 37 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.LegalPerson, FieldFlex.lone, new LookupSettings { EntityName = EntityType.Instance.LegalPerson(), ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 41 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.DocumentsDeliveryAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 45 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.RecipientName, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 46 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.DocumentsDeliveryMethod, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 50 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PersonResponsibleForDocuments, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 51 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.EmailForAccountingDocuments, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 55 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.Email, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 58 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PostAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 61 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PaymentMethod, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 62 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.Mfo, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 65 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.AccountNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 66 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.BankName, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 69 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PaymentEssentialElements, FieldFlex.lone, new Dictionary<string, object> { { "rows", 5 } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n");

WriteLiteral("        ");

            
            #line 72 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
   Write(Html.SectionHead("sectionInfo", @BLResources.TitleEmployeeInformation));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 75 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.ChiefNameInNominative, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 76 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PositionInNominative, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 80 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.ChiefNameInGenitive, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 81 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PositionInGenitive, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 85 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.Phone, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 86 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.OperatesOnTheBasisInGenitive, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 90 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.CertificateNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 91 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.CertificateDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 95 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.WarrantyNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 99 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.WarrantyBeginDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 100 "..\..\Views\CreateOrUpdate\Ukraine\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.WarrantyEndDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

WriteLiteral("\r\n    ");

        }
    }
}
#pragma warning restore 1591
