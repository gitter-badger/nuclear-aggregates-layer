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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/FirmContact.cshtml")]
    public partial class FirmContact : System.Web.Mvc.WebViewPage<Models.FirmContactViewModel>
    {
        public FirmContact()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 4 "..\..\Views\CreateOrUpdate\FirmContact.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        window.InitPage = function ()\r\n        {\r\n            window.Card.on(\"" +
"afterbuild\", function ()\r\n            {\r\n                if (Ext.getDom(\"Contact" +
"Type\").value == \"Email\")\r\n                    var v = new Ext.ux.LinkField(\r\n   " +
"                 {\r\n                        applyTo: \'Contact\',\r\n               " +
"         contactTypeCfg: Ext.ux.LinkField.prototype.contactTypeRegistry.email,\r\n" +
"                        listeners:\r\n                            {\r\n             " +
"                   invalid: function (el, msg)\r\n                                " +
"{\r\n                                    this.validator.updateValidationMessage({ " +
"FieldName: \'Contact\', ValidationMessageId: \'Contact_validationMessage\' }, msg);\r" +
"\n                                },\r\n                                valid: func" +
"tion (el)\r\n                                {\r\n                                  " +
"  this.validator.updateValidationMessage({ FieldName: \'Contact\', ValidationMessa" +
"geId: \'Contact_validationMessage\' }, \'\');\r\n                                },\r\n " +
"                               scope: this\r\n                            }\r\n     " +
"               });\r\n                if (Ext.getDom(\"ContactType\").value == \"Webs" +
"ite\")\r\n                    var v = new Ext.ux.LinkField(\r\n                    {\r" +
"\n                        applyTo: \'Contact\',\r\n                        contactTyp" +
"eCfg: Ext.ux.LinkField.prototype.contactTypeRegistry.url,\r\n                     " +
"   listeners:\r\n                            {\r\n                                in" +
"valid: function (el, msg)\r\n                                {\r\n                  " +
"                  this.validator.updateValidationMessage({ FieldName: \'Contact\'," +
" ValidationMessageId: \'Contact_validationMessage\' }, msg);\r\n                    " +
"            },\r\n                                valid: function (el)\r\n          " +
"                      {\r\n                                    this.validator.upda" +
"teValidationMessage({ FieldName: \'Contact\', ValidationMessageId: \'Contact_valida" +
"tionMessage\' }, \'\');\r\n                                },\r\n                      " +
"          scope: this\r\n                            }\r\n                    });\r\n " +
"           });\r\n        }\r\n    </script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 58 "..\..\Views\CreateOrUpdate\FirmContact.cshtml"
Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"Div2\"");

WriteAttribute("title", Tuple.Create(" title=\"", 2446), Tuple.Create("\"", 2482)
            
            #line 59 "..\..\Views\CreateOrUpdate\FirmContact.cshtml"
, Tuple.Create(Tuple.Create("", 2454), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 2454), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 61 "..\..\Views\CreateOrUpdate\FirmContact.cshtml"
       Write(Html.TemplateField(m => m.ContactType, FieldFlex.twins, new Dictionary<string, object> { { "disabled", "disabled" } }, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 64 "..\..\Views\CreateOrUpdate\FirmContact.cshtml"
       Write(Html.TemplateField(m => m.Contact, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
