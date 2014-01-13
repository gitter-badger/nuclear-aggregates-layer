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
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
    using DoubleGis.Erm.Platform.Common;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.Model.Entities.Enums;
    using DoubleGis.Erm.Platform.Model.Metadata.Enums;
    using DoubleGis.Erm.Platform.UI.Web.Mvc;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    
    #line 1 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
    using Platform.Common.Utils;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/GroupOperation/QualifyFirm.cshtml")]
    public partial class QualifyFirm : System.Web.Mvc.WebViewPage<Models.GroupOperation.QualifyFirmViewModel>
    {
        public QualifyFirm()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 5 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 9 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
            Write(BLResources.Qualify);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 10 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                  Write(BLResources.QualifyFirm);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 11 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
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

WriteAttribute("href", Tuple.Create(" href=\"", 481), Tuple.Create("\"", 544)
, Tuple.Create(Tuple.Create("", 488), Tuple.Create("/Content/Progress.css?", 488), true)
            
            #line 15 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
, Tuple.Create(Tuple.Create("", 510), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 510), false)
);

WriteLiteral(" />\r\n\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 563), Tuple.Create("\"", 636)
, Tuple.Create(Tuple.Create("", 569), Tuple.Create("/Scripts/Ext.Ajax.syncRequest.js?", 569), true)
            
            #line 17 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
, Tuple.Create(Tuple.Create("", 602), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 602), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 683), Tuple.Create("\"", 764)
, Tuple.Create(Tuple.Create("", 689), Tuple.Create("/Scripts/DoubleGis.UI.GroupOperations.js?", 689), true)
            
            #line 18 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
, Tuple.Create(Tuple.Create("", 730), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 730), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.namespace(\'Ext.DoubleGis.UI.Firm\');\r\n        Ext.DoubleGis.UI.Firm" +
".QualifyProcessor = Ext.extend(Ext.DoubleGis.UI.GroupProcessor, {\r\n            U" +
"serId: 0,\r\n            ClientId: 0,\r\n            UserCodeLookup: {},\r\n          " +
"  ClientCodeLookup: {},\r\n            IsCreateNewClients: false,\r\n            Cli" +
"entCardsToOpen: [],\r\n            constructor: function(config) {\r\n              " +
"  Ext.apply(config, {\r\n                    listeners: {\r\n                       " +
" configspecificcontrols: function() {\r\n                            this.ConfigCu" +
"stomControls();\r\n                        },\r\n                        applyuserse" +
"ttings: function() {\r\n                            this.ApplyUserSettings();\r\n   " +
"                     },\r\n                        entityprocessingsuccess: functi" +
"on(entityId) {\r\n                            this.EntityProcessingSuccess(entityI" +
"d);\r\n                        },\r\n                        entityprocessingfail: f" +
"unction(entityId) {\r\n                            this.EntityProcessingFail(entit" +
"yId);\r\n                        },\r\n                        processingfinished: f" +
"unction() {\r\n                            this.ProcessingFinished();\r\n           " +
"             }\r\n                    }\r\n                });\r\n                Ext." +
"DoubleGis.UI.Firm.QualifyProcessor.superclass.constructor.call(this, config);\r\n " +
"           },\r\n            ConfigCustomControls: function() {\r\n                /" +
"/ show error messages\r\n                if (Ext.getDom(\"Notifications\").innerHTML" +
".trim() == \"OK\") {\r\n                    window.close();\r\n                    ret" +
"urn;\r\n                } else if (Ext.getDom(\"Notifications\").innerHTML.trim() !=" +
" \"\" && Ext.getDom(\"Notifications\").innerHTML.trim() != \"OK\") {\r\n                " +
"    Ext.get(\"Notifications\").addClass(\"Notifications\");\r\n                }\r\n\r\n  " +
"              var onRadioClick = this.RadioClick.createDelegate(this);\r\n        " +
"        Ext.get(\"rdoNewAccount\").on(\"click\", onRadioClick);\r\n                Ext" +
".get(\"rdoOther\").on(\"click\", onRadioClick);\r\n                Ext.get(\"rdoAssignT" +
"oMe\").on(\"click\", onRadioClick);\r\n                Ext.get(\"rdoAssignToUser\").on(" +
"\"click\", onRadioClick);\r\n\r\n                this.UserCodeLookup = Ext.getCmp(\"Use" +
"rCode\");\r\n                this.ClientCodeLookup = Ext.getCmp(\"ClientCode\");\r\n\r\n " +
"               // filter client lookup\r\n                this.UserCodeLookup.on(\"" +
"change\", function() {\r\n                    if (this.UserCodeLookup.getValue())\r\n" +
"                        this.ClientCodeLookup.extendedInfo = \"userId=\" + this.Us" +
"erCodeLookup.getValue().id;\r\n                    else\r\n                        t" +
"his.ClientCodeLookup.extendedInfo = \"\";\r\n\r\n                    this.ClientCodeLo" +
"okup.clearValue();\r\n                }, this);\r\n\r\n                // set checkbox" +
"es initial values\r\n                Ext.getDom(\"rdoOther\").checked = false;\r\n    " +
"            Ext.getDom(\"rdoAssignToUser\").checked = false;\r\n            },\r\n    " +
"        RadioClick: function() {\r\n                if (Ext.getDom(\"rdoAssignToMe\"" +
").checked) {\r\n                    this.UserCodeLookup.disable();\r\n              " +
"  } else if (Ext.getDom(\"rdoAssignToUser\").checked) {\r\n                    this." +
"UserCodeLookup.enable();\r\n                }\r\n\r\n                if (Ext.getDom(\"r" +
"doNewAccount\").checked) {\r\n                    this.ClientCodeLookup.disable();\r" +
"\n                } else if (Ext.getDom(\"rdoOther\").checked) {\r\n                 " +
"   this.ClientCodeLookup.enable();\r\n                }\r\n\r\n                if (Ext" +
".getDom(\"rdoAssignToMe\").checked) {\r\n                    this.ClientCodeLookup.e" +
"xtendedInfo = \"userId=0\";\r\n                    this.UserCodeLookup.disable();\r\n " +
"               } else if (Ext.getDom(\"rdoAssignToUser\").checked) {\r\n            " +
"        if (this.UserCodeLookup.getValue())\r\n                        this.Client" +
"CodeLookup.extendedInfo = \"userId=\" + this.UserCodeLookup.getValue().id;\r\n      " +
"              else\r\n                        this.ClientCodeLookup.extendedInfo =" +
" \"\";\r\n\r\n                    this.UserCodeLookup.enable();\r\n                }\r\n  " +
"          },\r\n            IsUserSettingsValid: function() {\r\n                // " +
"warning if client to set\r\n                if (Ext.getDom(\"rdoOther\").checked) {\r" +
"\n                    if (Ext.getDom(\"ClientCode\").value == \"\") {\r\n              " +
"          Ext.MessageBox.show({\r\n                            title: \'\',\r\n       " +
"                     // TODO {all, 18.12.2013}: возможно некоректное отображение" +
" диакритики\r\n                            // TODO {all, 18.12.2013}: ресурс можно" +
" перенести в ClientResourceStorage\r\n                            msg: \'");

            
            #line 116 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                             Write(BLResources.QualifyMustPickClient);

            
            #line default
            #line hidden
WriteLiteral(@"',
                            buttons: window.Ext.MessageBox.OK,
                            width: 300,
                            icon: window.Ext.MessageBox.ERROR
                        });
                        return false;
                    }
                }

                // warning if owner to set
                if (Ext.getDom(""rdoAssignToUser"").checked) {
                    if (Ext.getDom(""UserCode"").value == """") {
                        Ext.MessageBox.show({
                            title: '',
                            msg: '");

            
            #line 130 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                             Write(BLResources.QualifyMustPickUser);

            
            #line default
            #line hidden
WriteLiteral("\',\r\n                            buttons: window.Ext.MessageBox.OK,\r\n             " +
"               width: 300,\r\n                            icon: window.Ext.Message" +
"Box.ERROR\r\n                        });\r\n                        return false;\r\n " +
"                   }\r\n                }\r\n\r\n                return true;\r\n       " +
"     },\r\n            ApplyUserSettings: function() {\r\n                this.IsCre" +
"ateNewClients = window.Ext.getDom(\"rdoNewAccount\").checked;\r\n                thi" +
"s.UserId = Ext.getDom(\"rdoAssignToUser\").checked ? this.UserCodeLookup.getValue(" +
").id : \"\";\r\n                this.ClientId = Ext.getDom(\"rdoOther\").checked ? thi" +
"s.ClientCodeLookup.getValue().id : \"\";\r\n\r\n                Ext.getDom(\"rdoAssignT" +
"oMe\").disabled = \"disabled\";\r\n                Ext.getDom(\"rdoAssignToUser\").disa" +
"bled = \"disabled\";\r\n                Ext.getDom(\"rdoNewAccount\").disabled = \"disa" +
"bled\";\r\n                Ext.getDom(\"rdoOther\").disabled = \"disabled\";\r\n         " +
"       this.UserCodeLookup.disable();\r\n                this.ClientCodeLookup.dis" +
"able();\r\n            },\r\n            CreateParamsForControllerCall: function(ent" +
"ityId) {\r\n                return { entityId: entityId, userId: this.UserId, clie" +
"ntId: this.ClientId };\r\n            },\r\n            EntityProcessingSuccess: fun" +
"ction(responseText) {\r\n                var entityId = Ext.decode(responseText);\r" +
"\n                if (this.IsCreateNewClients) {\r\n                    this.Client" +
"CardsToOpen.push(entityId); // открываем карточки всех вновь созданных клиентов\r" +
"\n                } else if (this.ClientCardsToOpen.length == 0) {\r\n             " +
"       this.ClientCardsToOpen.push(entityId); // открываем карточку клиента - к " +
"которому привязали фирмы\r\n                }\r\n            },\r\n            EntityP" +
"rocessingFail: function(entityId) {\r\n            },\r\n            ProcessingFinis" +
"hed: function() {\r\n                for (var i = 0; i < this.ClientCardsToOpen.le" +
"ngth; i++) {\r\n                    Ext.DoubleGis.Global.Helpers.OpenEntity(\'Clien" +
"t\', this.ClientCardsToOpen[i]);\r\n                }\r\n            }\r\n        });\r\n" +
"        Ext.onReady(function() {\r\n            var ids;\r\n            if (window.d" +
"ialogArguments) {\r\n                ids = window.dialogArguments.Values ? window." +
"dialogArguments.Values : window.dialogArguments;\r\n            } else {\r\n        " +
"        var queryParameters = Ext.urlDecode(window.location.search.substring(1))" +
";\r\n                ids = queryParameters.CrmIds.split(\',\');\r\n            }\r\n\r\n  " +
"          var config = {\r\n                Entities: ids, // массив id сущностей\r" +
"\n                OperationName: \'");

            
            #line 183 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                           Write(Model.OperationName);

            
            #line default
            #line hidden
WriteLiteral("\', // тип операции - Qualify, Assign, ChangeTerritory\r\n                CloseButto" +
"nText: \'");

            
            #line 184 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                             Write(BLResources.Close);

            
            #line default
            #line hidden
WriteLiteral("\', // локализованная надпись для кнопки закрыть\r\n                NeedToSelectOneO" +
"rMoreItemsMsg: \'");

            
            #line 185 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                                           Write(BLResources.NeedToSelectOneOrMoreItems);

            
            #line default
            #line hidden
WriteLiteral("\', // локализованная надпись о том что нужно выбрать один или несколько элементов" +
"\r\n                ResultMessageTitle: \'");

            
            #line 186 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                                Write(BLResources.GroupOperationResultsTitle);

            
            #line default
            #line hidden
WriteLiteral("\', // локализованная надпись - заголовок для результатов операции\r\n              " +
"  ResultMessageTemplate: \'");

            
            #line 187 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                                   Write(BLResources.GroupOperationResultsMessage);

            
            #line default
            #line hidden
WriteLiteral(@"' // локализованная надпись - шаблон строки для результатов операции
            };
            var qualifyProcessor = new Ext.DoubleGis.UI.Firm.QualifyProcessor(config);
            if (!qualifyProcessor.CheckProcessingPossibility()) {
                return;
            }

            qualifyProcessor.Process();
        });
    </script>

");

            
            #line 198 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
    
            
            #line default
            #line hidden
            
            #line 198 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {

            
            #line default
            #line hidden
WriteLiteral("        <table");

WriteLiteral(" cellspacing=\"5\"");

WriteLiteral(" cellpadding=\"5\"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(" height=\"100%\"");

WriteLiteral(">\r\n            <colgroup>\r\n                <col");

WriteLiteral(" width=\"26\"");

WriteLiteral(" />\r\n                <col />\r\n            </colgroup>\r\n            <tbody");

WriteLiteral("  id=\"qualifysettings\"");

WriteLiteral(">\r\n                <tr>\r\n                    <td");

WriteLiteral(" colspan=\"2\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" style=\"height: 24px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(">");

            
            #line 208 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                                                                 Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                    </td>\r\n                </tr>\r\n                <tr>\r\n " +
"                   <td");

WriteLiteral(" valign=\"top\"");

WriteLiteral(">\r\n                        <input");

WriteLiteral(" id=\"rdoNewAccount\"");

WriteLiteral(" class=\"radio\"");

WriteLiteral(" checked=\"checked\"");

WriteLiteral(" type=\"radio\"");

WriteLiteral(" name=\"AssignedClient\"");

WriteLiteral(" />\r\n                    </td>\r\n                    <td");

WriteLiteral(" valign=\"top\"");

WriteLiteral(">\r\n                        <label");

WriteLiteral(" style=\"font-weight: bold\"");

WriteLiteral(" for=\"rdoNewAccount\"");

WriteLiteral(">");

            
            #line 216 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                                                                        Write(BLResources.QualifyCreateNewClient);

            
            #line default
            #line hidden
WriteLiteral("</label>\r\n                        <br />\r\n                        <div");

WriteLiteral(" style=\"color: #444444; padding-top: 5px\"");

WriteLiteral(">");

            
            #line 218 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                                                                 Write(BLResources.QualifyCreateNewClientLegend);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                    </td>\r\n                </tr>\r\n                <tr>\r\n " +
"                   <td");

WriteLiteral(" colspan=\"2\"");

WriteLiteral(">\r\n                        &nbsp;\r\n                    </td>\r\n                </t" +
"r>\r\n                <tr>\r\n                    <td");

WriteLiteral(" valign=\"top\"");

WriteLiteral(">\r\n                        <input");

WriteLiteral(" id=\"rdoOther\"");

WriteLiteral(" class=\"radio\"");

WriteLiteral(" type=\"radio\"");

WriteLiteral(" name=\"AssignedClient\"");

WriteLiteral(" />\r\n                    </td>\r\n                    <td");

WriteLiteral(" valign=\"top\"");

WriteLiteral(">\r\n                        <label");

WriteLiteral(" style=\"font-weight: bold\"");

WriteLiteral(" for=\"rdoOther\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 232 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                       Write(BLResources.QualifyPickClient);

            
            #line default
            #line hidden
WriteLiteral("</label>\r\n                        <br />\r\n                        <div");

WriteLiteral(" style=\"padding-bottom: 10px; color: #444444; padding-top: 5px\"");

WriteLiteral(">\r\n                            <label");

WriteLiteral(" for=\"rdoOther\"");

WriteLiteral(">\r\n");

WriteLiteral("                                ");

            
            #line 236 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                           Write(BLResources.QualifyPickClientLegend);

            
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

            
            #line 241 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                                   Write(Html.LookupFor(m => m.ClientCode, new LookupSettings { Disabled = true, EntityName = EntityName.Client, ExtendedInfo = "userId=0" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                    </td>\r\n                                </tr" +
">\r\n                            </tbody>\r\n                        </table>\r\n     " +
"               </td>\r\n                </tr>\r\n                <tr>\r\n             " +
"       <td");

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

            
            #line 254 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                       Write(BLResources.QualifyAssignToMe);

            
            #line default
            #line hidden
WriteLiteral("</label>\r\n                        <br />\r\n                        <div");

WriteLiteral(" style=\"color: #444444; padding-top: 5px\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 257 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                       Write(BLResources.QualifyAssignToMeLegend);

            
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

            
            #line 271 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                       Write(BLResources.QualifyAssignToOtherUser);

            
            #line default
            #line hidden
WriteLiteral("</label>\r\n                        <br />\r\n                        <div");

WriteLiteral(" style=\"padding-bottom: 10px; color: #444444; padding-top: 5px\"");

WriteLiteral(">\r\n                            <label");

WriteLiteral(" for=\"rdoAssignToUser\"");

WriteLiteral(">\r\n");

WriteLiteral("                                ");

            
            #line 275 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                           Write(BLResources.QualifyAssignToOtherUserLegend);

            
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

            
            #line 280 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                                   Write(Html.LookupFor(k => k.UserCode, new LookupSettings { Disabled = true, EntityName = EntityName.User, ExtendedInfo = "'hideReserveUser=true'" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                    </td>\r\n                                </tr" +
">\r\n                            </tbody>\r\n                        </table>\r\n     " +
"               </td>\r\n                </tr>\r\n                <tr");

WriteLiteral(" style=\"display: none;\"");

WriteLiteral(">\r\n                    <td");

WriteLiteral(" colspan=\"2\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 289 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                   Write(Html.HiddenFor(m => m.EntityTypeName));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </td>\r\n                </tr>\r\n            </tbody>\r\n       " +
"     <tbody>\r\n                <tr>\r\n                    <td");

WriteLiteral(" colspan=\"2\"");

WriteLiteral(" style=\"padding-left: 10px;\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" id=\"pbDiv\"");

WriteLiteral(">\r\n                            <div");

WriteLiteral(" id=\"pbInner\"");

WriteLiteral(">\r\n                            </div>\r\n                        </div>\r\n          " +
"          </td>\r\n                </tr>\r\n            </tbody>\r\n        </table>\r\n" +
"");

            
            #line 304 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
