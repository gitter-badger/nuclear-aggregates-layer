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

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Views.CreateOrUpdate.Cyprus
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Cyprus/LegalPersonProfile.cshtml")]
    public partial class LegalPersonProfile : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Cyprus.CyprusLegalPersonProfileViewModel>
    {
        public LegalPersonProfile()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
  
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

            
            #line 13 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
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

            
            #line 29 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
Write(Html.HiddenFor(m => m.LegalPersonType));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 30 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 1174), Tuple.Create("\"", 1210)
            
            #line 32 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
, Tuple.Create(Tuple.Create("", 1182), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 1182), false)
);

WriteLiteral(">    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 34 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.Name, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 38 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.LegalPerson, FieldFlex.lone, new LookupSettings { EntityName = EntityName.LegalPerson, ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 42 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.DocumentsDeliveryAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 46 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.RecipientName, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 47 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.DocumentsDeliveryMethod, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 51 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PersonResponsibleForDocuments, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 52 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.EmailForAccountingDocuments, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 56 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.AdditionalEmail, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 59 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PostAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 62 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PaymentMethod, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 65 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.AccountNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 66 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.BankName, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 69 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.IBAN, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 70 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.SWIFT, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 73 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.AdditionalPaymentElements, FieldFlex.lone, new Dictionary<string, object> { { "rows", 5 } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n");

WriteLiteral("        ");

            
            #line 76 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
   Write(Html.SectionHead("sectionInfo", @BLResources.TitleEmployeeInformation));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 79 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.ChiefNameInNominative, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 80 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PositionInNominative, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 84 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.HiddenFor(m => m.ChiefNameInGenitive));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 85 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.HiddenFor(m => m.PositionInGenitive));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 89 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.Phone, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 90 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.OperatesOnTheBasisInGenitive, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 94 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.CertificateNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 95 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.CertificateDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 99 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.WarrantyNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 103 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.RegistrationCertificateNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 104 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.RegistrationCertificateDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 108 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.WarrantyBeginDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 109 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.WarrantyEndDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 113 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.BargainNumber, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 117 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.BargainBeginDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 118 "..\..\Views\CreateOrUpdate\Cyprus\LegalPersonProfile.cshtml"
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
