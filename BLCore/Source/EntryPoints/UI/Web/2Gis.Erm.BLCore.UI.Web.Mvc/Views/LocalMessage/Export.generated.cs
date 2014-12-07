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

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Views.LocalMessage
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
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
    using DoubleGis.Erm.Platform.Common;
    
    #line 1 "..\..\Views\LocalMessage\Export.cshtml"
    using DoubleGis.Erm.Platform.Common.Utils;
    
    #line default
    #line hidden
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.Model.Entities.Enums;
    using DoubleGis.Erm.Platform.Model.Metadata.Enums;
    using DoubleGis.Erm.Platform.UI.Web.Mvc;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Security;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/LocalMessage/Export.cshtml")]
    public partial class Export : System.Web.Mvc.WebViewPage<LocalMessageExportViewModel>
    {
        public Export()
        {
        }
        public override void Execute()
        {
            
            #line 4 "..\..\Views\LocalMessage\Export.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" Выгрузить сообщение ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" Выгрузить сообщение ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" Выгрузить сообщение ");

});

WriteLiteral("\r\n");

DefineSection("PageContent", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 311), Tuple.Create("\"", 356)
, Tuple.Create(Tuple.Create("", 317), Tuple.Create("/Scripts/Tooltip.js?", 317), true)
            
            #line 14 "..\..\Views\LocalMessage\Export.cshtml"
, Tuple.Create(Tuple.Create("", 337), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 337), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.onReady(function() {\r\n\r\n            // show error messages\r\n      " +
"      if (Ext.getDom(\"Notifications\").innerHTML.trim() != \"\") {\r\n               " +
" Ext.getDom(\"Notifications\").style.display = \"block\";\r\n            }\r\n\r\n        " +
"    var depList = window.Ext.getDom(\"ViewConfig_DependencyList\");\r\n            i" +
"f (depList.value) {\r\n                this.DependencyHandler = new window.Ext.Dou" +
"bleGis.DependencyHandler();\r\n                this.DependencyHandler.register(win" +
"dow.Ext.decode(depList.value), window.EntityForm);\r\n            }\r\n\r\n           " +
" var orgUnitLookup = Ext.getCmp(\"OrganizationUnit\");\r\n            var dropDown =" +
" Ext.getDom(\"IntegrationType\");\r\n\r\n            if (dropDown.value == \"AccountDet" +
"ailsTo1C\" || dropDown.value == \"LegalPersonsTo1C\") {\r\n                orgUnitLoo" +
"kup.extendedInfo = \"filterByMovedToErm=true\";\r\n            } else {\r\n           " +
"     orgUnitLookup.extendedInfo = \"\";\r\n            }\r\n\r\n            Ext.get(\"Can" +
"cel\").on(\"click\", function() { window.close(); });\r\n            Ext.get(\"OK\").on" +
"(\"click\", function() {\r\n                if (Ext.DoubleGis.FormValidator.validate" +
"(window.EntityForm)) {\r\n                    Ext.getDom(\"OK\").disabled = \"disable" +
"d\";\r\n                    Ext.getDom(\"Cancel\").disabled = \"disabled\";\r\n          " +
"          Ext.getDom(\"Notifications\").style.display = \"none\";\r\n                 " +
"   window.EntityForm.submit();\r\n                }\r\n            });\r\n\r\n          " +
"  Ext.get(\"IntegrationType\").on(\"change\", function() {\r\n                var orgU" +
"nitLookup = Ext.getCmp(\"OrganizationUnit\");\r\n                var dropDown = Ext." +
"getDom(\"IntegrationType\");\r\n\r\n                if (dropDown.value == \"AccountDeta" +
"ilsTo1C\" || dropDown.value == \"LegalPersonsTo1C\" || dropDown.value == \"AccountDe" +
"tailsToServiceBus\") {\r\n                    orgUnitLookup.extendedInfo = \"filterB" +
"yMovedToErm=true\";\r\n                } else {\r\n                    orgUnitLookup." +
"extendedInfo = \"\";\r\n                }\r\n            });\r\n\r\n            //Запрещае" +
"м выбрать поздравления и уведомления об оплате. Это временное требование. Убрать" +
", когда нужно будет\r\n            var sendingTypes = document.getElementById(\'Mai" +
"lSendingType\').getElementsByTagName(\'option\');\r\n            sendingTypes[1].disa" +
"bled = true;\r\n            sendingTypes[2].disabled = true;\r\n        });\r\n    </s" +
"cript>\r\n");

            
            #line 66 "..\..\Views\LocalMessage\Export.cshtml"
    
            
            #line default
            #line hidden
            
            #line 66 "..\..\Views\LocalMessage\Export.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {

            
            #line default
            #line hidden
WriteLiteral("        <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 69 "..\..\Views\LocalMessage\Export.cshtml"
       Write(Html.Hidden("ViewConfig_DependencyList", Model.ViewConfig.DependencyList));

            
            #line default
            #line hidden
WriteLiteral("\r\n            <div");

WriteLiteral(" style=\"display: none; height: 26px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(" class=\"Notifications\"");

WriteLiteral(" onmouseover=\" AddTooltip(Ext.getDom(\'Notifications\') ? Ext.getDom(\'Notifications" +
"\').innerHTML : \'\'); \"");

WriteLiteral(" onmouseout=\" RemoveTooltip(); \"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 71 "..\..\Views\LocalMessage\Export.cshtml"
           Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 74 "..\..\Views\LocalMessage\Export.cshtml"
           Write(Html.TemplateField(m => m.IntegrationType, FieldFlex.lone, null, LocalMessageController.IntegrationTypeExportResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 77 "..\..\Views\LocalMessage\Export.cshtml"
           Write(Html.TemplateField(m => m.OrganizationUnit, FieldFlex.lone, new LookupSettings { EntityName = EntityName.OrganizationUnit }));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 80 "..\..\Views\LocalMessage\Export.cshtml"
           Write(Html.TemplateField(m => m.MailSendingType, FieldFlex.lone, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 83 "..\..\Views\LocalMessage\Export.cshtml"
           Write(Html.TemplateField(m => m.PeriodStart, FieldFlex.lone, new CalendarSettings
                                                                            {
                                                                                Store = CalendarSettings.StoreMode.Relative,
                                                                                Display = CalendarSettings.DisplayMode.Month,
                                                                                MinDate = new DateTime(2010, 12, 1),
                                                                                MaxDate = DateTime.Now.GetNextMonthFirstDate()
                                                                            }));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 92 "..\..\Views\LocalMessage\Export.cshtml"
           Write(Html.TemplateField(m => m.PeriodStartFor1C, FieldFlex.lone, new CalendarSettings
                    {
                        Store = CalendarSettings.StoreMode.Relative,
                        MinDate = new DateTime(2010, 12, 1),
                        MaxDate = DateTime.Today
                    }));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 100 "..\..\Views\LocalMessage\Export.cshtml"
           Write(Html.TemplateField(m => m.IncludeRegionalAdvertisement, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 103 "..\..\Views\LocalMessage\Export.cshtml"
           Write(Html.TemplateField(m => m.CreateCsvFile, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n        </div>\r\n");

            
            #line 106 "..\..\Views\LocalMessage\Export.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
