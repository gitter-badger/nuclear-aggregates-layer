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

namespace DoubleGis.Erm.BLFlex.Web.Mvc.Global.Views.CreateOrUpdate
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
    using DoubleGis.Erm.BL.Resources.Server.Properties;
    using DoubleGis.Erm.Core;
    using DoubleGis.Erm.Core.Enums;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.UI.Web.Mvc;
    using DoubleGis.Erm.UI.Web.Mvc.Controllers;
    using DoubleGis.Erm.UI.Web.Mvc.Models;
    
    #line 1 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
    using DoubleGis.Erm.UI.Web.Mvc.Utils;
    
    #line default
    #line hidden
    
    #line 2 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
    using Platform.Web.Mvc.Utils;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/CyprusLegalPersonProfile.cshtml")]
    public partial class CyprusLegalPersonProfile : System.Web.Mvc.WebViewPage<CyprusLegalPersonProfileViewModel>
    {
        public CyprusLegalPersonProfile()
        {
        }
        public override void Execute()
        {
            
            #line 5 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
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

            
            #line 15 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
                                Write(Html.Raw(Json.Encode(Model.DisabledDocuments)));

            
            #line default
            #line hidden
WriteLiteral(@";
                for (var i = 0; i < disabledValues.length; i++) {
                    var optionToDisable = operatesValues[disabledValues[i]];
                    optionToDisable.disabled = true;
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

            
            #line 27 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
Write(Html.HiddenFor(m => m.LegalPersonType));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 28 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 951), Tuple.Create("\"", 987)
            
            #line 30 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
, Tuple.Create(Tuple.Create("", 959), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 959), false)
);

WriteLiteral(">    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 32 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.Name, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 36 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.LegalPerson, FieldFlex.lone, new LookupSettings { EntityName = EntityName.LegalPerson, ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 40 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.DocumentsDeliveryAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 44 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.RecipientName, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 45 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.DocumentsDeliveryMethod, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 49 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PersonResponsibleForDocuments, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 50 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.EmailForAccountingDocuments, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 54 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.AdditionalEmail, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 57 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PostAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 60 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.HiddenFor(m => m.PaymentEssentialElements));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 63 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PaymentMethod, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 66 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.AccountNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 67 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.BankName, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 70 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.IBAN, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 71 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.SWIFT, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 74 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.AdditionalPaymentElements, FieldFlex.lone, new Dictionary<string, object> { { "rows", 5 } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n\r\n");

WriteLiteral("        ");

            
            #line 77 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
   Write(Html.SectionHead("sectionInfo", @BLResources.TitleEmployeeInformation));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 80 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.ChiefNameInNominative, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 81 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.PositionInNominative, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 85 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.HiddenFor(m => m.ChiefNameInGenitive));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 86 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.HiddenFor(m => m.PositionInGenitive));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 90 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.Phone, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 91 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.OperatesOnTheBasisInGenitive, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 95 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.CertificateNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 96 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.CertificateDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 100 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.WarrantyNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 104 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.RegistrationCertificateNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 105 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.RegistrationCertificateDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 109 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.WarrantyBeginDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 110 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.WarrantyEndDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 114 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.BargainNumber, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 118 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
       Write(Html.TemplateField(m => m.BargainBeginDate, FieldFlex.twins, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 119 "..\..\Views\CreateOrUpdate\CyprusLegalPersonProfile.cshtml"
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
