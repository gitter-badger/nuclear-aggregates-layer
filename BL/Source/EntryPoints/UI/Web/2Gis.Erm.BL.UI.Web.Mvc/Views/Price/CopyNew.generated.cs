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

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Views.Price
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
    using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
    using DoubleGis.Erm.BLCore.Resources.Server.Properties;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
    using DoubleGis.Erm.Platform.Common;
    
    #line 2 "..\..\Views\Price\CopyNew.cshtml"
    using DoubleGis.Erm.Platform.Common.Utils;
    
    #line default
    #line hidden
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.Model.Entities.Enums;
    using DoubleGis.Erm.Platform.Model.Metadata.Enums;
    using DoubleGis.Erm.Platform.UI.Web.Mvc;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Price/CopyNew.cshtml")]
    public partial class CopyNew : System.Web.Mvc.WebViewPage<CopyNewPriceViewModel>
    {
        public CopyNew()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 5 "..\..\Views\Price\CopyNew.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 9 "..\..\Views\Price\CopyNew.cshtml"
            Write(BLResources.PriceCopyNewConfirmationLabel);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 10 "..\..\Views\Price\CopyNew.cshtml"
                  Write(BLResources.PriceCopyNewConfirmationLabel);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 11 "..\..\Views\Price\CopyNew.cshtml"
                    Write(BLResources.PriceCopyNewConfirmation);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

WriteLiteral("\r\n");

DefineSection("PageContent", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.DoubleGis.ValidatorRegistry.validators[\"checkDate\"] = function (ru" +
"le)\r\n        {\r\n            return function (value, context)\r\n            {\r\n   " +
"             var date = Date.parseDate(Ext.get(rule.ValidationParameters.dateFie" +
"ld).getValue(), \'c\');\r\n                var expectedDate = new Date(date.getFullY" +
"ear(), date.getMonth(), 1, 0, 0, 0, 0); //first day\r\n                if (rule.Va" +
"lidationParameters.isFirstDay == false)\r\n                {\r\n                    " +
"var tmpDate = new Date((new Date(date.getFullYear(), date.getMonth() + 1, 1)) - " +
"1);\r\n                    expectedDate = new Date(tmpDate.getFullYear(), tmpDate." +
"getMonth(), tmpDate.getDate(), 0, 0, 0, 0);\r\n                }\r\n                " +
"return (date.getTime() == expectedDate.getTime());\r\n            };\r\n        };\r\n" +
"\r\n\r\n        var setVisualFeatures = function ()\r\n        {\r\n            var divR" +
"ows = window.Ext.query(\"div.field-wrapper\");\r\n            var i;\r\n            fo" +
"r (i = 0; i < divRows.length; i++)\r\n            {\r\n                window.Ext.fl" +
"y(divRows[i]).addClassOnOver(\"field-wrapper-over\");\r\n            }\r\n\r\n          " +
"  var inputs = window.Ext.query(\".inputfields\");\r\n            for (i = 0; i < in" +
"puts.length; i++)\r\n            {\r\n                window.Ext.fly(inputs[i]).addC" +
"lassOnFocus(\"inputfields-selected\");\r\n            }\r\n        };\r\n        var Sub" +
"mitForm = function ()\r\n        {\r\n            if (Ext.DoubleGis.FormValidator.va" +
"lidate(EntityForm, null))\r\n            {\r\n                Ext.getDom(\"Id\").value" +
" = window.dialogArguments.sourcePriceId;\r\n\r\n                Ext.getDom(\"OK\").dis" +
"abled = \"disabled\";\r\n                Ext.getDom(\"Cancel\").disabled = \"disabled\";" +
"\r\n\r\n                Ext.DoubleGis.FormValidator.validate(EntityForm, null);\r\n\r\n " +
"               EntityForm.submit();\r\n            }\r\n        };\r\n\r\n        Ext.on" +
"Ready(function ()\r\n        {\r\n            setVisualFeatures();\r\n\r\n            //" +
"Show error messages\r\n            if (Ext.getDom(\"Notifications\").innerHTML.trim(" +
") == \"OK\")\r\n            {\r\n                window.close();\r\n                retu" +
"rn;\r\n            }\r\n            else if (Ext.getDom(\"Notifications\").innerHTML.t" +
"rim() != \"\")\r\n            {\r\n                Ext.get(\"Notifications\").addClass(\"" +
"Notifications\");\r\n            }\r\n\r\n            //write eventhandlers for buttons" +
"\r\n            Ext.get(\"Cancel\").on(\"click\", function () { window.returnValue = \"" +
"Cancel\"; window.close(); });\r\n            Ext.get(\"OK\").on(\"click\", SubmitForm);" +
"\r\n        });\r\n    </script>\r\n");

            
            #line 82 "..\..\Views\Price\CopyNew.cshtml"
    
            
            #line default
            #line hidden
            
            #line 82 "..\..\Views\Price\CopyNew.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {
    
            
            #line default
            #line hidden
            
            #line 84 "..\..\Views\Price\CopyNew.cshtml"
Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
            
            #line 84 "..\..\Views\Price\CopyNew.cshtml"
                              

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" style=\"height: 15px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 86 "..\..\Views\Price\CopyNew.cshtml"
   Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n");

WriteLiteral("    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 89 "..\..\Views\Price\CopyNew.cshtml"
   Write(Html.TemplateField(m => m.BeginDate, FieldFlex.lone, new CalendarSettings { MinDate = DateTime.Today.GetNextMonthFirstDate(), Store = CalendarSettings.StoreMode.Relative }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n");

WriteLiteral("    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 92 "..\..\Views\Price\CopyNew.cshtml"
   Write(Html.TemplateField(m => m.PublishDate, FieldFlex.lone, new CalendarSettings { MinDate = DateTime.Today, Store = CalendarSettings.StoreMode.Relative }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n");

WriteLiteral("    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 95 "..\..\Views\Price\CopyNew.cshtml"
   Write(Html.TemplateField(m => m.OrganizationUnit, FieldFlex.lone, new LookupSettings { EntityName = EntityName.OrganizationUnit }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n");

            
            #line 97 "..\..\Views\Price\CopyNew.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
