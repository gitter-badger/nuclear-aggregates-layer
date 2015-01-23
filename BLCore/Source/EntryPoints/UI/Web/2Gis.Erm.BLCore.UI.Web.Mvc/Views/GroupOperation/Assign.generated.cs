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

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Views.GroupOperation
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
    
    #line 1 "..\..\Views\GroupOperation\Assign.cshtml"
    using DoubleGis.Erm.Platform.Common.Utils;
    
    #line default
    #line hidden
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.Model.Entities.Enums;
    using DoubleGis.Erm.Platform.Model.Metadata.Enums;
    using DoubleGis.Erm.Platform.UI.Web.Mvc;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Security;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    using NuClear.Model.Common.Entities;
    using NuClear.Model.Common.Operations.Identity;
    using NuClear.Model.Common.Operations.Identity.Generic;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/GroupOperation/Assign.cshtml")]
    public partial class Assign : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLCore.UI.Web.Mvc.Models.GroupOperation.AssignViewModel>
    {
        public Assign()
        {
        }
        public override void Execute()
        {
            
            #line 4 "..\..\Views\GroupOperation\Assign.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 8 "..\..\Views\GroupOperation\Assign.cshtml"
            Write(BLResources.GroupOperationConfirm);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 9 "..\..\Views\GroupOperation\Assign.cshtml"
                  Write(string.Format(BLResources.AssignAssignMessage, Model.EntityTypeName.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture)));

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 10 "..\..\Views\GroupOperation\Assign.cshtml"
                    Write(string.Format(BLResources.GroupOperationTopBarMessage, Model.EntityTypeName.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture)));

            
            #line default
            #line hidden
WriteLiteral(" ");

});

WriteLiteral("\r\n");

DefineSection("PageContent", () => {

WriteLiteral("\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 651), Tuple.Create("\"", 699)
, Tuple.Create(Tuple.Create("", 658), Tuple.Create("/Content/Progress.css?", 658), true)
            
            #line 14 "..\..\Views\GroupOperation\Assign.cshtml"
, Tuple.Create(Tuple.Create("", 680), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 680), false)
);

WriteLiteral(" />\r\n\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 718), Tuple.Create("\"", 776)
, Tuple.Create(Tuple.Create("", 724), Tuple.Create("/Scripts/Ext.Ajax.syncRequest.js?", 724), true)
            
            #line 16 "..\..\Views\GroupOperation\Assign.cshtml"
, Tuple.Create(Tuple.Create("", 757), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 757), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 823), Tuple.Create("\"", 889)
, Tuple.Create(Tuple.Create("", 829), Tuple.Create("/Scripts/DoubleGis.UI.GroupOperations.js?", 829), true)
            
            #line 17 "..\..\Views\GroupOperation\Assign.cshtml"
, Tuple.Create(Tuple.Create("", 870), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 870), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 936), Tuple.Create("\"", 981)
, Tuple.Create(Tuple.Create("", 942), Tuple.Create("/Scripts/Tooltip.js?", 942), true)
            
            #line 18 "..\..\Views\GroupOperation\Assign.cshtml"
, Tuple.Create(Tuple.Create("", 962), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 962), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    \r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.namespace(\'Ext.DoubleGis.UI.Assign\');\r\n        Ext.DoubleGis.UI.As" +
"sign.AssignProcessor = Ext.extend(Ext.DoubleGis.UI.GroupProcessor, {\r\n          " +
"  FailedEntitiesMessages: [],\r\n            OwnerCode: 0,\r\n            IsPartialA" +
"ssign: false,\r\n            UserCodeLookup: {},\r\n            bypassValidationInfo" +
": false, // информация о том, есть ли привилегия \"Обработка лицевых счетов с зад" +
"олжностью\"\r\n            BypassValidationView: {},   // представление для Предлож" +
"енния продолжить операцию, если есть задолженность по лицевым счетам\r\n          " +
"  EntitiesToProcess: {},\r\n            constructor: function (config) {\r\n        " +
"        Ext.apply(config, {\r\n                    listeners: {\r\n                 " +
"       configspecificcontrols: function () {\r\n                            this.C" +
"onfigCustomControls();\r\n                        },\r\n                        appl" +
"yusersettings: function () {\r\n                            this.ApplyUserSettings" +
"();\r\n                        },\r\n                        processingfinished: fun" +
"ction () {\r\n                            this.ProcessingFinished();\r\n            " +
"            },\r\n                        entityprocessingfail: function (msg) {\r\n" +
"                            this.EntityAssignFailed(msg);\r\n                     " +
"   }\r\n                    }\r\n                });\r\n                Ext.DoubleGis." +
"UI.Assign.AssignProcessor.superclass.constructor.call(this, config);\r\n          " +
"      if (config.EntitiesToProcess) {\r\n                \tvar entitiesToProcess = " +
"{};\r\n                \tExt.each(config.EntitiesToProcess, function (x) {\r\n       " +
"         \t\tentitiesToProcess[x.entityId] = x.entityName;\r\n                \t});\r\n" +
"                \tthis.EntitiesToProcess = entitiesToProcess;\r\n                }\r" +
"\n            },\r\n            ConfigCustomControls: function () {\r\n              " +
"  var onRadioClick = this.RadioClick.createDelegate(this);\r\n                Ext." +
"get(\"rdoAssignToMe\").on(\"click\", onRadioClick);\r\n                Ext.get(\"rdoAss" +
"ignToUser\").on(\"click\", onRadioClick);\r\n                this.UserCodeLookup = Ex" +
"t.getCmp(\"UserCode\");\r\n            },\r\n            RadioClick: function () {\r\n  " +
"              if (Ext.getDom(\"rdoAssignToMe\").checked) {\r\n                    th" +
"is.UserCodeLookup.disable();\r\n                }\r\n                else if (Ext.ge" +
"tDom(\"rdoAssignToUser\").checked) {\r\n                    this.UserCodeLookup.enab" +
"le();\r\n                }\r\n            },\r\n            IsUserSettingsValid: funct" +
"ion () {\r\n                if (Ext.getDom(\"rdoAssignToUser\").checked) {\r\n        " +
"            if (Ext.getDom(\"UserCode\").value == \"\") {\r\n                        E" +
"xt.MessageBox.show({\r\n                            title: \'\',\r\n                  " +
"          msg: Ext.LocalizedResources.AssignMustPickUser,\r\n                     " +
"       buttons: window.Ext.MessageBox.OK,\r\n                            width: 30" +
"0,\r\n                            icon: window.Ext.MessageBox.ERROR\r\n             " +
"           });\r\n                        return false;\r\n                    }\r\n  " +
"              }\r\n\r\n                return true;\r\n            },\r\n            App" +
"lyUserSettings: function () {\r\n                this.OwnerCode = Ext.getDom(\"rdoA" +
"ssignToUser\").checked ? this.UserCodeLookup.getValue().id : \"\";\r\n\r\n             " +
"   var ctx = Ext.getDom(\"rdoCascadeAssign\");\r\n                this.IsPartialAssi" +
"gn = ctx && !ctx.checked;\r\n\r\n                Ext.getDom(\"rdoAssignToMe\").disable" +
"d = \"disabled\";\r\n                Ext.getDom(\"rdoAssignToUser\").disabled = \"disab" +
"led\";\r\n                this.UserCodeLookup.disable();\r\n            },\r\n         " +
"   ResolveEntityName: function (entityId) {\r\n            \tif (this.EntitiesToPro" +
"cess.hasOwnProperty(entityId))\r\n            \t\treturn this.EntitiesToProcess[enti" +
"tyId];\r\n            \telse \r\n            \t\treturn this.superclass().ResolveEntity" +
"Name.call(this, entityId);\r\n            },\r\n            CreateParamsForControlle" +
"rCall: function (entityId) {\r\n                return { entityId: entityId, owner" +
"Code: this.OwnerCode, isPartialAssign: this.IsPartialAssign, bypassValidation: n" +
"ull };\r\n            },\r\n            ValidateEntryProcessingSuccessStatus: functi" +
"on (message) {\r\n                var bypassValidationInfo = window.Ext.decode(mes" +
"sage);\r\n                if (bypassValidationInfo.CanProceed && bypassValidationI" +
"nfo.CanProceed == true) {\r\n                    var isOperationContinue = confirm" +
"(bypassValidationInfo.Message);\r\n                    if (isOperationContinue) {\r" +
"\n                        var params = this.CreateParamsForControllerCall(bypassV" +
"alidationInfo.EntityId);\r\n                        params.bypassValidation = true" +
";\r\n                        var url = this.EvaluateOperationUrl();\r\n             " +
"           this.ProcessSingleEntity(url, params);\r\n\r\n                        ret" +
"urn this.SuccessStatus.ReprocessingRequired;\r\n                    }\r\n\r\n         " +
"           return this.SuccessStatus.Rejected;\r\n                }\r\n             " +
"   return this.SuccessStatus.Approved;\r\n            },\r\n            ProcessingFi" +
"nished: function () {\r\n                // innerHTML элемента Notifications присв" +
"аивается по окончании операции (см. файл GroupOperations.js),\r\n                /" +
"/ динамически засовываем туда линк.\r\n                if (this.SuccessProcessed <" +
" this.EntitiesCount && !this.IsSingleEntityProcessing) {\r\n\r\n                    " +
"this.FinishOperation(this.FailedEntitiesMessages.join(\'\\r\\n\'));\r\n\r\n             " +
"       var notifications = Ext.getDom(\'Notifications\');\r\n                    var" +
" errorsLinkNode = document.createElement(\"a\");\r\n                    errorsLinkNo" +
"de.id = \'ErrorsLink\';\r\n                    errorsLinkNode.href = \'#\';\r\n         " +
"           errorsLinkNode.appendChild(document.createTextNode(Ext.LocalizedResou" +
"rces.DisplayErrorsList));\r\n                    notifications.appendChild(errorsL" +
"inkNode);\r\n                    Ext.getDom(\'ErrorsLink\').onclick = function () {\r" +
"\n                        Ext.getDom(\'ErrorsForm\').submit();\r\n                   " +
" };\r\n                    Ext.getDom(\'ErrorsLink\').onclick();\r\n                }\r" +
"\n                else {\r\n                    this.FinishOperation();\r\n          " +
"      }\r\n            },\r\n            EntityAssignFailed: function (msg) {\r\n     " +
"           this.FailedEntitiesMessages[this.FailedEntitiesMessages.length] = msg" +
";\r\n            },\r\n            FinishOperation: function (msg) {\r\n              " +
"  var finishOperationResponse = window.Ext.Ajax.syncRequest({\r\n                 " +
"   method: \'POST\',\r\n                    url: \'/Operation/CreateOperationWithErro" +
"rLog\',\r\n                    params: { operationId: Ext.getDom(\"operationId\").val" +
"ue, log: msg, contentType: \'text/csv\', logFileName: \'Assign_errors.csv\' }\r\n     " +
"           });\r\n                if ((finishOperationResponse.conn.status >= 200 " +
"&& finishOperationResponse.conn.status < 300) || (Ext.isIE && finishOperationRes" +
"ponse.conn.status == 1223)) {\r\n                }\r\n                else {\r\n      " +
"              alert(finishOperationResponse.conn.responseText);\r\n               " +
"     return;\r\n                }\r\n            }\r\n        });\r\n\r\n        Ext.onRea" +
"dy(function () {\r\n            var dialogArguments = !window.dialogArguments ? []" +
" : (window.dialogArguments.Values ? window.dialogArguments.Values : window.dialo" +
"gArguments);\r\n            var ids = dialogArguments;\r\n\r\n            var isExtend" +
"edMode = (Ext.isArray(dialogArguments) && dialogArguments.length > 0 && Ext.isOb" +
"ject(dialogArguments[0]) && dialogArguments[0].hasOwnProperty(\'entityId\'));\r\n   " +
"         if (isExtendedMode) {\r\n            \tids = [];\r\n            \tExt.each(di" +
"alogArguments, function (x) { ids.push(x.entityId); });\r\n            }\r\n\r\n      " +
"      //window.Tooltip = new Ext.DoubleGis.UI.Tooltip(document);\r\n            Ex" +
"t.getDom(\'DivErrors\').style.visibility = \'hidden\';\r\n            Ext.getDom(\'Page" +
"ContentCell\').style[\"vertical-align\"] = \"top\";\r\n            var config = {\r\n    " +
"        \tEntitiesToProcess: isExtendedMode ? dialogArguments : null,\r\n          " +
"      Entities: ids, // массив id сущностей\r\n                OperationName: \'");

            
            #line 178 "..\..\Views\GroupOperation\Assign.cshtml"
                           Write(Model.OperationName);

            
            #line default
            #line hidden
WriteLiteral(@"', // тип операции - Qualify, Assign, ChangeTerritory
                CloseButtonText: Ext.LocalizedResources.Close, // локализованная надпись для кнопки закрыть
                NeedToSelectOneOrMoreItemsMsg: Ext.LocalizedResources.NeedToSelectOneOrMoreItems, // локализованная надпись о том что нужно выбрать один или несколько элементов
                ResultMessageTitle: Ext.LocalizedResources.GroupOperationResultsTitle, // локализованная надпись - заголовок для результатов операции
                ResultMessageTemplate: Ext.LocalizedResources.GroupOperationResultsMessage // локализованная надпись - шаблон строки для результатов операции
            };
            var assignProcessor = new Ext.DoubleGis.UI.Assign.AssignProcessor(config);
            if (!assignProcessor.CheckProcessingPossibility())
            {
                return;
            }
            assignProcessor.Process();
        });
    </script>
    
    <div");

WriteLiteral(" style=\"height: 8px; padding-left: 5px;padding-top: 4px;position: fixed;\"");

WriteLiteral(" id=\"DivErrors\"");

WriteLiteral(">\r\n");

            
            #line 194 "..\..\Views\GroupOperation\Assign.cshtml"
    
            
            #line default
            #line hidden
            
            #line 194 "..\..\Views\GroupOperation\Assign.cshtml"
     using (Html.BeginForm("GetOperationLog", "Operation", FormMethod.Post, new Dictionary<string, object> { { "target", "_blank" }, { "id", "ErrorsForm" } }))
    {

            
            #line default
            #line hidden
WriteLiteral("        <input");

WriteLiteral(" type=\"hidden\"");

WriteLiteral(" name=\"operationId\"");

WriteAttribute("value", Tuple.Create(" value=\"", 10418), Tuple.Create("\"", 10441)
            
            #line 196 "..\..\Views\GroupOperation\Assign.cshtml"
, Tuple.Create(Tuple.Create("", 10426), Tuple.Create<System.Object, System.Int32>(Guid.NewGuid()
            
            #line default
            #line hidden
, 10426), false)
);

WriteLiteral(" />\r\n");

            
            #line 197 "..\..\Views\GroupOperation\Assign.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("        \r\n   </div>\r\n\r\n");

            
            #line 201 "..\..\Views\GroupOperation\Assign.cshtml"
    
            
            #line default
            #line hidden
            
            #line 201 "..\..\Views\GroupOperation\Assign.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {

            
            #line default
            #line hidden
WriteLiteral("        <table");

WriteLiteral(" cellspacing=\"5\"");

WriteLiteral(" cellpadding=\"5\"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(" style=\"position: fixed;\"");

WriteLiteral(">\r\n            <colgroup>\r\n                <col");

WriteLiteral(" width=\"32\"");

WriteLiteral(" />\r\n                <col />\r\n            </colgroup>\r\n            <tbody>\r\n     " +
"           <tr>\r\n                    <td");

WriteLiteral(" colspan=\"2\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" style=\"height: 30px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(" \r\n                             onmouseover=\"AddTooltip(Ext.getDom(\'Notifications" +
"\').childNodes.length > 0 ? Ext.getDom(\'Notifications\').childNodes[0].data : \'\');" +
"\"");

WriteLiteral(" \r\n                             onmouseout=\"RemoveTooltip();\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 214 "..\..\Views\GroupOperation\Assign.cshtml"
                       Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n                        </div>\r\n                    </td>\r\n                </tr" +
">\r\n                <tr>\r\n                    <td");

WriteLiteral(" valign=\"top\"");

WriteLiteral(">\r\n                        <input");

WriteLiteral(" id=\"rdoAssignToMe\"");

WriteLiteral(" class=\"radio\"");

WriteLiteral(" checked=\"checked\"");

WriteLiteral(" type=\"radio\"");

WriteLiteral(" name=\"AssignedUser\"");

WriteLiteral(" />\r\n                    </td>\r\n                    <td");

WriteLiteral(" valign=\"top\"");

WriteLiteral(">\r\n                        <label");

WriteLiteral(" style=\"font-weight: bold\"");

WriteLiteral(" for=\"rdoAssignTome\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 224 "..\..\Views\GroupOperation\Assign.cshtml"
                       Write(BLResources.AssignAssignToMe);

            
            #line default
            #line hidden
WriteLiteral("</label>\r\n                        <br />\r\n                        <div");

WriteLiteral(" style=\"color: #444444; padding-top: 5px\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 227 "..\..\Views\GroupOperation\Assign.cshtml"
                       Write(String.Format(BLResources.AssignAssignToMeLegend, Model.EntityTypeName.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture)));

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                    </td>\r\n                </tr>\r\n                <tr>\r\n " +
"                   <td");

WriteLiteral(" colspan=\"2\"");

WriteLiteral(">\r\n                        &nbsp;\r\n                    </td>\r\n                </t" +
"r>\r\n                <tr>\r\n                    <td");

WriteLiteral(" valign=\"top\"");

WriteLiteral(">\r\n                        <input");

WriteLiteral(" id=\"rdoAssignToUser\"");

WriteLiteral(" class=\"radio\"");

WriteLiteral(" type=\"radio\"");

WriteLiteral(" name=\"AssignedUser\"");

WriteLiteral(" />\r\n                    </td>\r\n                    <td");

WriteLiteral(" valign=\"top\"");

WriteLiteral(">\r\n                        <label");

WriteLiteral(" style=\"font-weight: bold\"");

WriteLiteral(" for=\"rdoAssignToUser\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 241 "..\..\Views\GroupOperation\Assign.cshtml"
                       Write(BLResources.AssignAssignToOther);

            
            #line default
            #line hidden
WriteLiteral("</label>\r\n                        <br />\r\n                        <div");

WriteLiteral(" style=\"padding-bottom: 10px; color: #444444; padding-top: 5px\"");

WriteLiteral(">\r\n                            <label");

WriteLiteral(" for=\"rdoAssignToUser\"");

WriteLiteral(">\r\n");

WriteLiteral("                                ");

            
            #line 245 "..\..\Views\GroupOperation\Assign.cshtml"
                           Write(String.Format(BLResources.AssignAssignToOtherLegend, Model.EntityTypeName.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture)));

            
            #line default
            #line hidden
WriteLiteral("</label></div>\r\n                        <table");

WriteLiteral(" style=\"table-layout: fixed\"");

WriteLiteral(" cellspacing=\"0\"");

WriteLiteral(" cellpadding=\"0\"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(">\r\n                            <tbody>\r\n                                <tr>\r\n   " +
"                                 <td>\r\n");

WriteLiteral("                                        ");

            
            #line 250 "..\..\Views\GroupOperation\Assign.cshtml"
                                   Write(Html.LookupFor(k => k.UserCode, new LookupSettings { Disabled = true, EntityName = EntityName.User, ExtendedInfo = "'hideReserveUser=true'" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                    </td>\r\n                                </tr" +
">\r\n                            </tbody>\r\n                        </table>\r\n     " +
"               </td>\r\n                </tr>\r\n");

            
            #line 257 "..\..\Views\GroupOperation\Assign.cshtml"
                
            
            #line default
            #line hidden
            
            #line 257 "..\..\Views\GroupOperation\Assign.cshtml"
                 if (Model.PartialAssignSupported)
                {

            
            #line default
            #line hidden
WriteLiteral("                    <tr>\r\n                        <td");

WriteLiteral(" valign=\"top\"");

WriteLiteral(">\r\n                            <input");

WriteLiteral(" id=\"rdoCascadeAssign\"");

WriteLiteral(" class=\"checkbox\"");

WriteLiteral(" type=\"checkbox\"");

WriteLiteral(" name=\"IsPartialAssign\"");

WriteAttribute("disabled", Tuple.Create(" disabled=\"", 13799), Tuple.Create("\"", 13841)
            
            #line 261 "..\..\Views\GroupOperation\Assign.cshtml"
                                          , Tuple.Create(Tuple.Create("", 13810), Tuple.Create<System.Object, System.Int32>(Model.IsCascadeAssignForbidden
            
            #line default
            #line hidden
, 13810), false)
);

WriteLiteral(" />\r\n                        </td>\r\n                        <td >\r\n              " +
"              <label");

WriteLiteral(" style=\"font-weight: bold\"");

WriteLiteral(" for=\"rdoCascadeAssign\"");

WriteLiteral(">\r\n");

WriteLiteral("                                ");

            
            #line 265 "..\..\Views\GroupOperation\Assign.cshtml"
                           Write(BLResources.AssignInAllHierarchy);

            
            #line default
            #line hidden
WriteLiteral("\r\n                            </label>\r\n                        </td>\r\n          " +
"          </tr>\r\n");

            
            #line 269 "..\..\Views\GroupOperation\Assign.cshtml"

                }

            
            #line default
            #line hidden
WriteLiteral("                <tr>\r\n                    <td");

WriteLiteral(" colspan=\"2\"");

WriteLiteral(" style=\"padding-left: 10px;\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" id=\"pbDiv\"");

WriteLiteral(">\r\n                            <div");

WriteLiteral(" id=\"pbInner\"");

WriteLiteral(">\r\n                            </div>\r\n                        </div>\r\n          " +
"          </td>\r\n                </tr>\r\n            </tbody>\r\n        </table>\r\n" +
"");

            
            #line 281 "..\..\Views\GroupOperation\Assign.cshtml"
        
            
            #line default
            #line hidden
            
            #line 281 "..\..\Views\GroupOperation\Assign.cshtml"
   Write(Html.HiddenFor(m => m.EntityTypeName));

            
            #line default
            #line hidden
            
            #line 281 "..\..\Views\GroupOperation\Assign.cshtml"
                                              
        
            
            #line default
            #line hidden
            
            #line 282 "..\..\Views\GroupOperation\Assign.cshtml"
   Write(Html.HiddenFor(m => m.PartialAssignSupported));

            
            #line default
            #line hidden
            
            #line 282 "..\..\Views\GroupOperation\Assign.cshtml"
                                                      
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
