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
    using DoubleGis.Erm.BLCore.UI.Metadata.Confirmations;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models.GroupOperation;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
    using DoubleGis.Erm.Platform.Common;
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/GroupOperation/DeactivateUser.cshtml")]
    public partial class DeactivateUser : System.Web.Mvc.WebViewPage<OwnerGroupOperationViewModel>
    {
        public DeactivateUser()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\GroupOperation\DeactivateUser.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 7 "..\..\Views\GroupOperation\DeactivateUser.cshtml"
            Write(BLResources.DeactivateConfirmation);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 8 "..\..\Views\GroupOperation\DeactivateUser.cshtml"
                  Write(BLResources.DeactivateConfirmation);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 9 "..\..\Views\GroupOperation\DeactivateUser.cshtml"
                    Write(string.Format(BLResources.GroupOperationTopBarMessage, Model.EntityTypeName.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture)));

            
            #line default
            #line hidden
WriteLiteral(" ");

});

WriteLiteral("\r\n");

DefineSection("PageContent", () => {

WriteLiteral("\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 436), Tuple.Create("\"", 563)
, Tuple.Create(Tuple.Create("", 442), Tuple.Create("/Scripts/", 442), true)
            
            #line 14 "..\..\Views\GroupOperation\DeactivateUser.cshtml"
, Tuple.Create(Tuple.Create("", 451), Tuple.Create<System.Object, System.Int32>("Ext.LocalizedResources." + ViewData.GetUserLocaleInfo().TwoLetterISOLanguageName + ".js"
            
            #line default
            #line hidden
, 451), false)
, Tuple.Create(Tuple.Create("", 543), Tuple.Create("?", 543), true)
            
            #line 14 "..\..\Views\GroupOperation\DeactivateUser.cshtml"
                                        , Tuple.Create(Tuple.Create("", 544), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 544), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    \r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 647), Tuple.Create("\"", 695)
, Tuple.Create(Tuple.Create("", 654), Tuple.Create("/Content/Progress.css?", 654), true)
            
            #line 16 "..\..\Views\GroupOperation\DeactivateUser.cshtml"
, Tuple.Create(Tuple.Create("", 676), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 676), false)
);

WriteLiteral(" />\r\n\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 714), Tuple.Create("\"", 772)
, Tuple.Create(Tuple.Create("", 720), Tuple.Create("/Scripts/Ext.Ajax.syncRequest.js?", 720), true)
            
            #line 18 "..\..\Views\GroupOperation\DeactivateUser.cshtml"
, Tuple.Create(Tuple.Create("", 753), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 753), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 819), Tuple.Create("\"", 885)
, Tuple.Create(Tuple.Create("", 825), Tuple.Create("/Scripts/DoubleGis.UI.GroupOperations.js?", 825), true)
            
            #line 19 "..\..\Views\GroupOperation\DeactivateUser.cshtml"
, Tuple.Create(Tuple.Create("", 866), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 866), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 932), Tuple.Create("\"", 977)
, Tuple.Create(Tuple.Create("", 938), Tuple.Create("/Scripts/Tooltip.js?", 938), true)
            
            #line 20 "..\..\Views\GroupOperation\DeactivateUser.cshtml"
, Tuple.Create(Tuple.Create("", 958), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 958), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.namespace(\'Ext.DoubleGis.UI.Deactivate\');\r\n        Ext.DoubleGis.U" +
"I.Deactivate.DeactivateUserProcessor = Ext.extend(Ext.DoubleGis.UI.GroupProcesso" +
"r, {\r\n            FailedEntitiesMessages: [],\r\n            OwnerCode: 0,\r\n      " +
"      IsPartialAssign: false,\r\n            UserCodeLookup: {},\r\n            cons" +
"tructor: function (config) {\r\n                Ext.apply(config, {\r\n             " +
"       listeners: {\r\n                        configspecificcontrols: function ()" +
" {\r\n                            this.ConfigCustomControls();\r\n                  " +
"      },\r\n                        applyusersettings: function () {\r\n            " +
"                this.ApplyUserSettings();\r\n                        }\r\n          " +
"          }\r\n                });\r\n                Ext.DoubleGis.UI.Deactivate.De" +
"activateUserProcessor.superclass.constructor.call(this, config);\r\n            }," +
"\r\n            ConfigCustomControls: function () {\r\n                var onRadioCl" +
"ick = this.RadioClick.createDelegate(this);\r\n                Ext.get(\"rdoAssignT" +
"oMe\").on(\"click\", onRadioClick);\r\n                Ext.get(\"rdoAssignToUser\").on(" +
"\"click\", onRadioClick);\r\n                this.UserCodeLookup = Ext.getCmp(\"UserC" +
"ode\");\r\n            },\r\n            RadioClick: function () {\r\n                i" +
"f (Ext.getDom(\"rdoAssignToMe\").checked) {\r\n                    this.UserCodeLook" +
"up.disable();\r\n                }\r\n                else if (Ext.getDom(\"rdoAssign" +
"ToUser\").checked) {\r\n                    this.UserCodeLookup.enable();\r\n        " +
"        }\r\n            },\r\n            IsUserSettingsValid: function () {\r\n     " +
"           if (Ext.getDom(\"rdoAssignToUser\").checked) {\r\n                    if " +
"(Ext.getDom(\"UserCode\").value == \"\") {\r\n                        Ext.MessageBox.s" +
"how({\r\n                            title: \'\',\r\n                            msg: " +
"Ext.LocalizedResources.AssignMustPickUser,\r\n                            buttons:" +
" window.Ext.MessageBox.OK,\r\n                            width: 300,\r\n           " +
"                 icon: window.Ext.MessageBox.ERROR\r\n                        });\r" +
"\n                        return false;\r\n                    }\r\n                }" +
"\r\n\r\n                return true;\r\n            },\r\n            ApplyUserSettings:" +
" function () {\r\n                this.OwnerCode = Ext.getDom(\"rdoAssignToUser\").c" +
"hecked ? this.UserCodeLookup.getValue().id : \"\";\r\n\r\n                Ext.getDom(\"" +
"rdoAssignToMe\").disabled = \"disabled\";\r\n                Ext.getDom(\"rdoAssignToU" +
"ser\").disabled = \"disabled\";\r\n                this.UserCodeLookup.disable();\r\n  " +
"          },\r\n            CreateParamsForControllerCall: function (entityId) {\r\n" +
"                return { entityId: entityId, ownerCode: this.OwnerCode };\r\n     " +
"       }\r\n        });\r\n\r\n        Ext.onReady(function () {\r\n            var iWid" +
"th = 500;\r\n            var iHeight = 300;\r\n            window.dialogWidth = iWid" +
"th + \"px\";\r\n            window.dialogHeight = iHeight + \"px\";\r\n\r\n            Ext" +
".getDom(\'PageContentCell\').style[\"vertical-align\"] = \"top\";\r\n\r\n            var i" +
"ds = !window.dialogArguments ? [] : (window.dialogArguments.Values ? window.dial" +
"ogArguments.Values : window.dialogArguments);\r\n\r\n            var config = {\r\n   " +
"             Entities: ids, // массив id сущностей\r\n                OperationNam" +
"e: \'");

            
            #line 96 "..\..\Views\GroupOperation\DeactivateUser.cshtml"
                           Write(Model.OperationName);

            
            #line default
            #line hidden
WriteLiteral(@"', // тип операции - Qualify, Assign, ChangeTerritory
                CloseButtonText: Ext.LocalizedResources.Close, // локализованная надпись для кнопки закрыть
                NeedToSelectOneOrMoreItemsMsg: Ext.LocalizedResources.NeedToSelectOneOrMoreItems, // локализованная надпись о том что нужно выбрать один или несколько элементов                
                ResultMessageTitle: Ext.LocalizedResources.GroupOperationResultsTitle, // локализованная надпись - заголовок для результатов операции
                ResultMessageTemplate: Ext.LocalizedResources.GroupOperationResultsMessage // локализованная надпись - шаблон строки для результатов операции
            };
            var deactivateUserProcessor = new Ext.DoubleGis.UI.Deactivate.DeactivateUserProcessor(config);
            if (!deactivateUserProcessor.CheckProcessingPossibility()) {
                return;
            }
            deactivateUserProcessor.Process();
        });
    </script>
");

            
            #line 109 "..\..\Views\GroupOperation\DeactivateUser.cshtml"
    
            
            #line default
            #line hidden
            
            #line 109 "..\..\Views\GroupOperation\DeactivateUser.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {

            
            #line default
            #line hidden
WriteLiteral("        <table");

WriteLiteral(" cellspacing=\"5\"");

WriteLiteral(" cellpadding=\"5\"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(" height=\"100%\"");

WriteLiteral(" style=\"position: fixed\"");

WriteLiteral(">\r\n            <colgroup>\r\n                <col");

WriteLiteral(" width=\"26\"");

WriteLiteral(" />\r\n                <col />\r\n            </colgroup>\r\n            <tbody>\r\n     " +
"           <tr>\r\n                    <td");

WriteLiteral(" colspan=\"2\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" style=\"height: 30px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteAttribute("onmouseover", Tuple.Create(" \r\n                             onmouseover=\"", 5826), Tuple.Create("\"", 5898)
, Tuple.Create(Tuple.Create("", 5871), Tuple.Create("AddTooltip(", 5871), true)
            
            #line 120 "..\..\Views\GroupOperation\DeactivateUser.cshtml"
, Tuple.Create(Tuple.Create("", 5882), Tuple.Create<System.Object, System.Int32>(Model.Message
            
            #line default
            #line hidden
, 5882), false)
, Tuple.Create(Tuple.Create("", 5896), Tuple.Create(");", 5896), true)
);

WriteLiteral(" \r\n                             onmouseout=\"RemoveTooltip();\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 122 "..\..\Views\GroupOperation\DeactivateUser.cshtml"
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

            
            #line 132 "..\..\Views\GroupOperation\DeactivateUser.cshtml"
                       Write(BLResources.AssignAssignToMe);

            
            #line default
            #line hidden
WriteLiteral("</label>\r\n                        <br />\r\n                        <div");

WriteLiteral(" style=\"color: #444444; padding-top: 5px\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 135 "..\..\Views\GroupOperation\DeactivateUser.cshtml"
                       Write(BLResources.DeactivateUserAssignToMeLegend);

            
            #line default
            #line hidden
WriteLiteral("\r\n                        </div>\r\n                    </td>\r\n                </tr" +
">\r\n                <tr>\r\n                    <td");

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

WriteLiteral(">");

            
            #line 149 "..\..\Views\GroupOperation\DeactivateUser.cshtml"
                                                                          Write(BLResources.AssignAssignToOther);

            
            #line default
            #line hidden
WriteLiteral("</label>\r\n                        <br />\r\n                        <div");

WriteLiteral(" style=\"padding-bottom: 10px; color: #444444; padding-top: 5px\"");

WriteLiteral(">\r\n                            <label");

WriteLiteral(" for=\"rdoAssignToUser\"");

WriteLiteral(">");

            
            #line 152 "..\..\Views\GroupOperation\DeactivateUser.cshtml"
                                                    Write(BLResources.DeactivateUserAssignToOtherLegend);

            
            #line default
            #line hidden
WriteLiteral("</label>\r\n                        </div>\r\n                        <table");

WriteLiteral(" style=\"table-layout: fixed\"");

WriteLiteral(" cellspacing=\"0\"");

WriteLiteral(" cellpadding=\"0\"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(">\r\n                            <tbody>\r\n                                <tr>\r\n   " +
"                                 <td>\r\n");

WriteLiteral("                                        ");

            
            #line 158 "..\..\Views\GroupOperation\DeactivateUser.cshtml"
                                   Write(Html.LookupFor(k => k.UserCode, new LookupSettings { Disabled = true, EntityName = EntityType.Instance.User(), ExtendedInfo = "'hideReserveUser=true'"}));

            
            #line default
            #line hidden
WriteLiteral(@"
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div");

WriteLiteral(" id=\"pbDiv\"");

WriteLiteral(">\r\n                            <div");

WriteLiteral(" id=\"pbInner\"");

WriteLiteral(">\r\n                            </div>\r\n                        </div>\r\n          " +
"          </td>\r\n                </tr>\r\n            </tbody>\r\n        </table>\r\n" +
"");

            
            #line 175 "..\..\Views\GroupOperation\DeactivateUser.cshtml"
        
            
            #line default
            #line hidden
            
            #line 175 "..\..\Views\GroupOperation\DeactivateUser.cshtml"
   Write(Html.Hidden("EntityType", Model.EntityTypeName.Description));

            
            #line default
            #line hidden
            
            #line 175 "..\..\Views\GroupOperation\DeactivateUser.cshtml"
                                                                    
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
