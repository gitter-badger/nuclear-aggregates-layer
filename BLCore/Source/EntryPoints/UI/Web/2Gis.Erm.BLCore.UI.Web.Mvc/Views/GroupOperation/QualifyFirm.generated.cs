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
    using System.Web.Mvc;
    using System.Web.Mvc.Html;

    using DoubleGis.Erm.BLCore.Resources.Server.Properties;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models.GroupOperation;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    using NuClear.Model.Common.Entities;

    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/GroupOperation/QualifyFirm.cshtml")]
    public partial class QualifyFirm : System.Web.Mvc.WebViewPage<QualifyFirmViewModel>
    {
        public QualifyFirm()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 7 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
            Write(BLResources.Qualify);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 8 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                  Write(BLResources.QualifyFirm);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 9 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
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

WriteAttribute("href", Tuple.Create(" href=\"", 427), Tuple.Create("\"", 475)
, Tuple.Create(Tuple.Create("", 434), Tuple.Create("/Content/Progress.css?", 434), true)
            
            #line 13 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
, Tuple.Create(Tuple.Create("", 456), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 456), false)
);

WriteLiteral(" />\r\n\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 494), Tuple.Create("\"", 552)
, Tuple.Create(Tuple.Create("", 500), Tuple.Create("/Scripts/Ext.Ajax.syncRequest.js?", 500), true)
            
            #line 15 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
, Tuple.Create(Tuple.Create("", 533), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 533), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 599), Tuple.Create("\"", 665)
, Tuple.Create(Tuple.Create("", 605), Tuple.Create("/Scripts/DoubleGis.UI.GroupOperations.js?", 605), true)
            
            #line 16 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
, Tuple.Create(Tuple.Create("", 646), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 646), false)
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
"on (responseText) {\r\n                            this.EntityProcessingSuccess(re" +
"sponseText);\r\n                        },\r\n                        entityprocessi" +
"ngfail: function (message) {\r\n                            this.EntityProcessingF" +
"ail(message);\r\n                        },\r\n                        processingfin" +
"ished: function() {\r\n                            this.ProcessingFinished();\r\n   " +
"                     }\r\n                    }\r\n                });\r\n            " +
"    Ext.DoubleGis.UI.Firm.QualifyProcessor.superclass.constructor.call(this, con" +
"fig);\r\n            },\r\n            ConfigCustomControls: function() {\r\n         " +
"       // show error messages\r\n                if (Ext.getDom(\"Notifications\").i" +
"nnerHTML.trim() == \"OK\") {\r\n                    window.close();\r\n               " +
"     return;\r\n                } else if (Ext.getDom(\"Notifications\").innerHTML.t" +
"rim() != \"\" && Ext.getDom(\"Notifications\").innerHTML.trim() != \"OK\") {\r\n        " +
"            Ext.get(\"Notifications\").addClass(\"Notifications\");\r\n               " +
" }\r\n\r\n                var onRadioClick = this.RadioClick.createDelegate(this);\r\n" +
"                Ext.get(\"rdoNewAccount\").on(\"click\", onRadioClick);\r\n           " +
"     Ext.get(\"rdoOther\").on(\"click\", onRadioClick);\r\n                Ext.get(\"rd" +
"oAssignToMe\").on(\"click\", onRadioClick);\r\n                Ext.get(\"rdoAssignToUs" +
"er\").on(\"click\", onRadioClick);\r\n\r\n                this.UserCodeLookup = Ext.get" +
"Cmp(\"UserCode\");\r\n                this.ClientCodeLookup = Ext.getCmp(\"ClientCode" +
"\");\r\n\r\n                // filter client lookup\r\n                this.UserCodeLoo" +
"kup.on(\"change\", function() {\r\n                    if (this.UserCodeLookup.getVa" +
"lue())\r\n                        this.ClientCodeLookup.extendedInfo = \"userId=\" +" +
" this.UserCodeLookup.getValue().id;\r\n                    else\r\n                 " +
"       this.ClientCodeLookup.extendedInfo = \"\";\r\n\r\n                    this.Clie" +
"ntCodeLookup.clearValue();\r\n                }, this);\r\n\r\n                // set " +
"checkboxes initial values\r\n                Ext.getDom(\"rdoOther\").checked = fals" +
"e;\r\n                Ext.getDom(\"rdoAssignToUser\").checked = false;\r\n            " +
"},\r\n            RadioClick: function() {\r\n                if (Ext.getDom(\"rdoAss" +
"ignToMe\").checked) {\r\n                    this.UserCodeLookup.disable();\r\n      " +
"          } else if (Ext.getDom(\"rdoAssignToUser\").checked) {\r\n                 " +
"   this.UserCodeLookup.enable();\r\n                }\r\n\r\n                if (Ext.g" +
"etDom(\"rdoNewAccount\").checked) {\r\n                    this.ClientCodeLookup.dis" +
"able();\r\n                } else if (Ext.getDom(\"rdoOther\").checked) {\r\n         " +
"           this.ClientCodeLookup.enable();\r\n                }\r\n\r\n               " +
" if (Ext.getDom(\"rdoAssignToMe\").checked) {\r\n                    this.ClientCode" +
"Lookup.extendedInfo = \"filterToCurrentUser=true\";\r\n                    this.User" +
"CodeLookup.disable();\r\n                } else if (Ext.getDom(\"rdoAssignToUser\")." +
"checked) {\r\n                    if (this.UserCodeLookup.getValue())\r\n           " +
"             this.ClientCodeLookup.extendedInfo = \"userId=\" + this.UserCodeLooku" +
"p.getValue().id;\r\n                    else\r\n                        this.ClientC" +
"odeLookup.extendedInfo = \"\";\r\n\r\n                    this.UserCodeLookup.enable()" +
";\r\n                }\r\n            },\r\n            IsUserSettingsValid: function(" +
") {\r\n                // warning if client to set\r\n                if (Ext.getDom" +
"(\"rdoOther\").checked) {\r\n                    if (Ext.getDom(\"ClientCode\").value " +
"== \"\") {\r\n                        Ext.MessageBox.show({\r\n                       " +
"     title: \'\',\r\n                            msg: Ext.LocalizedResources.Qualify" +
"MustPickClient,\r\n                            buttons: window.Ext.MessageBox.OK,\r" +
"\n                            width: 300,\r\n                            icon: wind" +
"ow.Ext.MessageBox.ERROR\r\n                        });\r\n                        re" +
"turn false;\r\n                    }\r\n                }\r\n\r\n                // warn" +
"ing if owner to set\r\n                if (Ext.getDom(\"rdoAssignToUser\").checked) " +
"{\r\n                    if (Ext.getDom(\"UserCode\").value == \"\") {\r\n              " +
"          Ext.MessageBox.show({\r\n                            title: \'\',\r\n       " +
"                     msg: Ext.LocalizedResources.QualifyMustPickUser,\r\n         " +
"                   buttons: window.Ext.MessageBox.OK,\r\n                         " +
"   width: 300,\r\n                            icon: window.Ext.MessageBox.ERROR\r\n " +
"                       });\r\n                        return false;\r\n             " +
"       }\r\n                }\r\n\r\n                return true;\r\n            },\r\n   " +
"         ApplyUserSettings: function() {\r\n                this.IsCreateNewClient" +
"s = window.Ext.getDom(\"rdoNewAccount\").checked;\r\n                this.UserId = E" +
"xt.getDom(\"rdoAssignToUser\").checked ? this.UserCodeLookup.getValue().id : \"\";\r\n" +
"                this.ClientId = Ext.getDom(\"rdoOther\").checked ? this.ClientCode" +
"Lookup.getValue().id : \"\";\r\n\r\n                Ext.getDom(\"rdoAssignToMe\").disabl" +
"ed = \"disabled\";\r\n                Ext.getDom(\"rdoAssignToUser\").disabled = \"disa" +
"bled\";\r\n                Ext.getDom(\"rdoNewAccount\").disabled = \"disabled\";\r\n    " +
"            Ext.getDom(\"rdoOther\").disabled = \"disabled\";\r\n                this." +
"UserCodeLookup.disable();\r\n                this.ClientCodeLookup.disable();\r\n   " +
"         },\r\n            CreateParamsForControllerCall: function(entityId) {\r\n  " +
"              return { entityId: entityId, userId: this.UserId, clientId: this.C" +
"lientId };\r\n            },\r\n            EntityProcessingSuccess: function(respon" +
"seText) {\r\n                var response = Ext.decode(responseText);\r\n           " +
"     if (this.IsCreateNewClients) {\r\n                    this.ClientCardsToOpen." +
"push(response.RelatedEntityId); // открываем карточки всех вновь созданных клиен" +
"тов\r\n                } else if (this.ClientCardsToOpen.length == 0) {\r\n         " +
"           this.ClientCardsToOpen.push(response.RelatedEntityId); // открываем к" +
"арточку клиента - к которому привязали фирмы\r\n                }\r\n            },\r" +
"\n            EntityProcessingFail: function (message) {\r\n            },\r\n       " +
"     ProcessingFinished: function() {\r\n                if (!this.IsSingleEntityP" +
"rocessing || this.ClientCardsToOpen.length != 1) {\r\n                    return;\r" +
"\n                }\r\n\r\n                Ext.DoubleGis.Global.Helpers.OpenEntity(\'C" +
"lient\', this.ClientCardsToOpen[0]);\r\n            }\r\n        });\r\n        Ext.onR" +
"eady(function() {\r\n\r\n            var ids = !window.dialogArguments ? [] : (windo" +
"w.dialogArguments.Values ? window.dialogArguments.Values : window.dialogArgument" +
"s);\r\n\r\n            var config = {\r\n                Entities: ids, // массив id с" +
"ущностей\r\n                OperationName: \'");

            
            #line 176 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                           Write(Model.OperationName);

            
            #line default
            #line hidden
WriteLiteral(@"', // тип операции - Qualify, Assign, ChangeTerritory
                CloseButtonText: Ext.LocalizedResources.Close, // локализованная надпись для кнопки закрыть
                NeedToSelectOneOrMoreItemsMsg: Ext.LocalizedResources.NeedToSelectOneOrMoreItems, // локализованная надпись о том что нужно выбрать один или несколько элементов
                ResultMessageTitle: Ext.LocalizedResources.GroupOperationResultsTitle, // локализованная надпись - заголовок для результатов операции
                ResultMessageTemplate: Ext.LocalizedResources.GroupOperationResultsMessage // локализованная надпись - шаблон строки для результатов операции
            };
            var qualifyProcessor = new Ext.DoubleGis.UI.Firm.QualifyProcessor(config);
            if (!qualifyProcessor.CheckProcessingPossibility()) {
                return;
            }

            qualifyProcessor.Process();
        });
    </script>

");

            
            #line 191 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
    
            
            #line default
            #line hidden
            
            #line 191 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
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

            
            #line 201 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
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

            
            #line 209 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                                                                        Write(BLResources.QualifyCreateNewClient);

            
            #line default
            #line hidden
WriteLiteral("</label>\r\n                        <br />\r\n                        <div");

WriteLiteral(" style=\"color: #444444; padding-top: 5px\"");

WriteLiteral(">");

            
            #line 211 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
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

            
            #line 225 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                       Write(BLResources.QualifyPickClient);

            
            #line default
            #line hidden
WriteLiteral("</label>\r\n                        <br />\r\n                        <div");

WriteLiteral(" style=\"padding-bottom: 10px; color: #444444; padding-top: 5px\"");

WriteLiteral(">\r\n                            <label");

WriteLiteral(" for=\"rdoOther\"");

WriteLiteral(">\r\n");

WriteLiteral("                                ");

            
            #line 229 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
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

            
            #line 234 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                                   Write(Html.LookupFor(m => m.ClientCode, new LookupSettings { Disabled = true, EntityName = EntityType.Instance.Client(), ExtendedInfo = "filterToCurrentUser=true" }));

            
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

            
            #line 247 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                       Write(BLResources.QualifyAssignToMe);

            
            #line default
            #line hidden
WriteLiteral("</label>\r\n                        <br />\r\n                        <div");

WriteLiteral(" style=\"color: #444444; padding-top: 5px\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 250 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
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

            
            #line 264 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                       Write(BLResources.QualifyAssignToOtherUser);

            
            #line default
            #line hidden
WriteLiteral("</label>\r\n                        <br />\r\n                        <div");

WriteLiteral(" style=\"padding-bottom: 10px; color: #444444; padding-top: 5px\"");

WriteLiteral(">\r\n                            <label");

WriteLiteral(" for=\"rdoAssignToUser\"");

WriteLiteral(">\r\n");

WriteLiteral("                                ");

            
            #line 268 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
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

            
            #line 273 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                                   Write(Html.LookupFor(k => k.UserCode, new LookupSettings { Disabled = true, EntityName = EntityType.Instance.User(), ExtendedInfo = "'hideReserveUser=true'" }));

            
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

            
            #line 282 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
                   Write(Html.Hidden("EntityType", Model.EntityTypeName.Description));

            
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

            
            #line 297 "..\..\Views\GroupOperation\QualifyFirm.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
