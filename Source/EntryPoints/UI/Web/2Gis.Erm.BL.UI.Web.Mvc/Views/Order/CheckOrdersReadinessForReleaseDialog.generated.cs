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

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Views.Order
{
    using System;
    using System.Collections.Generic;
    
    #line 1 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
    using System.Globalization;
    
    #line default
    #line hidden
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
    
    #line 2 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
    using Platform.Model.Metadata.Enums;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Order/CheckOrdersReadinessForReleaseDialog.cshtml")]
    public partial class CheckOrdersReadinessForReleaseDialog : System.Web.Mvc.WebViewPage<Models.CheckOrdersReadinessForReleaseDialogViewModel>
    {
        public CheckOrdersReadinessForReleaseDialog()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 6 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 10 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
            Write(BLResources.OrdersReadinessForReleaseReportTitle);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 11 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
                  Write(BLResources.OrdersReadinessForReleaseReportTitle);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 12 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
                    Write(BLResources.OrdersReadinessForReleaseReportParametersInformation);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

WriteLiteral("\r\n");

DefineSection("PageContent", () => {

WriteLiteral("\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 498), Tuple.Create("\"", 582)
, Tuple.Create(Tuple.Create("", 505), Tuple.Create("/Content/Ext.ux.DetailedProgressWindow.css?", 505), true)
            
            #line 16 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
             , Tuple.Create(Tuple.Create("", 548), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 548), false)
);

WriteLiteral(" />\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 605), Tuple.Create("\"", 687)
, Tuple.Create(Tuple.Create("", 611), Tuple.Create("/Scripts/Ext.ux.DetailedProgressWindow.js?", 611), true)
            
            #line 18 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
, Tuple.Create(Tuple.Create("", 653), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 653), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 734), Tuple.Create("\"", 821)
, Tuple.Create(Tuple.Create("", 740), Tuple.Create("/Scripts/Ext.ux.AsyncOperationClientManager.js?", 740), true)
            
            #line 19 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
, Tuple.Create(Tuple.Create("", 787), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 787), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n\r\n    Ext.DoubleGis.ValidatorRegistry.validators[\"validatePeriod\"] = function " +
"(rule)\r\n    {\r\n        return function (value, context)\r\n        {\r\n            " +
"var getValue = function (id) { return Ext.getCmp(id) ? Ext.getCmp(id).getValue()" +
" : Ext.getDom(id).value; };\r\n\r\n            var startDate = new Date(getValue(rul" +
"e.ValidationParameters.startDateField));\r\n            var endDate = new Date(get" +
"Value(rule.ValidationParameters.endDateField));\r\n\r\n            var expectedStart" +
"Date = new Date(startDate.getFullYear(), startDate.getMonth(), 1, 0, 0, 0, 0);\r\n" +
"\r\n            var tmpDate = new Date((new Date(expectedStartDate.getFullYear(), " +
"expectedStartDate.getMonth() + 1, 1)) - 1);\r\n            var expectedEndDate = n" +
"ew Date(tmpDate.getFullYear(), tmpDate.getMonth(), tmpDate.getDate(), 0, 0, 0, 0" +
");\r\n\r\n            if (startDate.getTime() != expectedStartDate.getTime())\r\n     " +
"           return false;\r\n\r\n            if (endDate.getTime() != expectedEndDate" +
".getTime())\r\n                return false;\r\n\r\n            return true;\r\n        " +
"};\r\n    };\r\n\r\n    Ext.DoubleGis.ValidatorRegistry.validators[\"checkPeriod\"] = fu" +
"nction (rule)\r\n    {\r\n        return function (value, context)\r\n        {\r\n     " +
"       var startDate = new Date(Ext.getCmp(rule.ValidationParameters.startDateFi" +
"eld) ? Ext.getCmp(rule.ValidationParameters.startDateField).getValue() : Ext.get" +
"Dom(rule.ValidationParameters.startDateField).value);\r\n            var endDate =" +
" new Date(Ext.getCmp(rule.ValidationParameters.endDateField) ? Ext.getCmp(rule.V" +
"alidationParameters.endDateField).getValue() : Ext.getDom(rule.ValidationParamet" +
"ers.endDateField).value);\r\n            return (startDate.getTime() < endDate.get" +
"Time());\r\n        };\r\n    };\r\n\r\n    Ext.DoubleGis.ValidatorRegistry.validators[\"" +
"checkDate\"] = function (rule)\r\n    {\r\n        return function (value, context)\r\n" +
"        {\r\n            var date = new Date(Ext.getCmp(rule.ValidationParameters." +
"dateField) ? Ext.getCmp(rule.ValidationParameters.dateField).getValue() : Ext.ge" +
"tDom(rule.ValidationParameters.dateField).value);\r\n            var expectedDate " +
"= new Date(date.getFullYear(), date.getMonth(), 1, 0, 0, 0, 0); //first day\r\n   " +
"         if (rule.ValidationParameters.isFirstDay == false)\r\n            {\r\n    " +
"            var tmpDate = new Date((new Date(date.getFullYear(), date.getMonth()" +
" + 1, 1)) - 1);\r\n                expectedDate = new Date(tmpDate.getFullYear(), " +
"tmpDate.getMonth(), tmpDate.getDate(), 0, 0, 0, 0);\r\n            }\r\n            " +
"return (date.getTime() == expectedDate.getTime());\r\n        };\r\n    };\r\n\r\n    Ex" +
"t.onReady(function ()\r\n    {\r\n        if (Ext.getDom(\'ErrorsLink\'))\r\n        {\r\n" +
"            Ext.getDom(\'ErrorsLink\').onclick = function ()\r\n            {\r\n     " +
"           Ext.getDom(\'ErrorsForm\').submit();\r\n            };\r\n        }\r\n\r\n    " +
"    //Show error messages\r\n        if (Ext.getDom(\"Notifications\").innerHTML.tri" +
"m() != \"\")\r\n        {\r\n            Ext.get(\"Notifications\").addClass(\"Notificati" +
"ons\");\r\n        }\r\n        else\r\n        {\r\n            Ext.get(\"Notifications\")" +
".removeClass(\"Notifications\");\r\n        }\r\n\r\n        Ext.get(\"Cancel\").on(\"click" +
"\", function ()\r\n        {\r\n            window.close();\r\n        });\r\n\r\n        E" +
"xt.get(\"OK\").on(\"click\", function ()\r\n        {\r\n            if (Ext.DoubleGis.F" +
"ormValidator.validate(EntityForm))\r\n            {\r\n                Ext.getDom(\"S" +
"tartPeriodDate\").value = new Date(Ext.getCmp(\"StartPeriodDate\").getValue()).form" +
"at(Ext.CultureInfo.DateTimeFormatInfo.PhpInvariantDateTimePattern);\r\n           " +
"     Ext.getDom(\"OK\").disabled = true;\r\n                EntityForm.submit();\r\n  " +
"              Ext.getDom(\"StartPeriodDate\").value = Date.parseDate(Ext.getDom(\"S" +
"tartPeriodDate\").value, Ext.CultureInfo.DateTimeFormatInfo.PhpInvariantDateTimeP" +
"attern).format(Ext.CultureInfo.DateTimeFormatInfo.PhpShortDatePattern);\r\n       " +
"     }\r\n        });\r\n\r\n        // Временно отключаем галку \"Включая подчинённых\"" +
", ERM-925\r\n        var tt = new Ext.ToolTip({\r\n            target: \'IncludeOwner" +
"Descendants-wrapper\',\r\n            // TODO {all, 18.12.2013}: возможно некоректн" +
"ое отображение диакритики\r\n            // TODO {all, 18.12.2013}: ресурс можно п" +
"еренести в ClientResourceStorage\r\n            html: \'");

            
            #line 113 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
              Write(BLResources.DisabledFunctionality);

            
            #line default
            #line hidden
WriteLiteral("\'\r\n        });\r\n\r\n        Ext.each(Ext.CardLookupSettings, function (item, i)\r\n  " +
"      {\r\n            new window.Ext.ux.LookupField(item);\r\n        }, this);\r\n  " +
"  });\r\n\r\n    </script>\r\n    \r\n");

            
            #line 124 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
    
            
            #line default
            #line hidden
            
            #line 124 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
     if(Model.HasErrors == true)
    {

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" style=\"height: 8px; padding-left: 5px;padding-top: 4px;position: fixed;\"");

WriteLiteral(" id=\"DivErrors\"");

WriteLiteral(">\r\n");

            
            #line 127 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
        
            
            #line default
            #line hidden
            
            #line 127 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
         using(Html.BeginForm("GetOperationLog", "Operation", FormMethod.Post, new Dictionary<string, object> { { "target", "_blank" }, {"id", "ErrorsForm"} }))
        {

            
            #line default
            #line hidden
WriteLiteral("            <input");

WriteLiteral(" type=\"hidden\"");

WriteLiteral(" name=\"operationId\"");

WriteAttribute("value", Tuple.Create(" value=\"", 5696), Tuple.Create("\"", 5725)
            
            #line 129 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
, Tuple.Create(Tuple.Create("", 5704), Tuple.Create<System.Object, System.Int32>(Model.ErrorLogFileId
            
            #line default
            #line hidden
, 5704), false)
);

WriteLiteral(" />\r\n");

            
            #line 130 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n");

            
            #line 132 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 134 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
    
            
            #line default
            #line hidden
            
            #line 134 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {
        
            
            #line default
            #line hidden
            
            #line 136 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
   Write(Html.Hidden("now", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)));

            
            #line default
            #line hidden
            
            #line 136 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
                                                                                   

            
            #line default
            #line hidden
WriteLiteral("        <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" style=\"height: 18px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(" class=\"Notifications\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 139 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
       Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 140 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
            
            
            #line default
            #line hidden
            
            #line 140 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
             if (Model.HasErrors == true)
            {

            
            #line default
            #line hidden
WriteLiteral("                <a");

WriteLiteral(" href=\"#\"");

WriteLiteral(" id=\"ErrorsLink\"");

WriteLiteral("> Просмотреть ошибки...</a>\r\n");

            
            #line 143 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
            }

            
            #line default
            #line hidden
WriteLiteral("        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 146 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
       Write(Html.TemplateField(m => m.StartPeriodDate, FieldFlex.lone, new DateTimeSettings { ShiftOffset = false, PeriodType = PeriodType.MonthlyLowerBound, DisplayStyle = DisplayStyle.WithoutDayNumber }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 149 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
       Write(Html.TemplateField(m => m.OrganizationUnit, FieldFlex.lone, new LookupSettings { EntityName = EntityName.OrganizationUnit, SearchFormFilterInfo = "IsDeleted=false&&IsActive=true" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 152 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
       Write(Html.TemplateField(m => m.Owner, FieldFlex.lone, new LookupSettings { EntityName = EntityName.User}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 155 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
       Write(Html.TemplateField(m => m.IncludeOwnerDescendants, FieldFlex.lone, new Dictionary<string, object> {{"disabled", "disabled"}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 158 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
       Write(Html.TemplateField(m => m.CheckAccountBalance, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

            
            #line 161 "..\..\Views\Order\CheckOrdersReadinessForReleaseDialog.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
