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
    
    #line 1 "..\..\Views\GroupOperation\ChangeDealClient.cshtml"
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/GroupOperation/ChangeDealClient.cshtml")]
    public partial class ChangeDealClient : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLCore.UI.Web.Mvc.Models.GroupOperation.ChangeClientViewModel>
    {
        public ChangeDealClient()
        {
        }
        public override void Execute()
        {
            
            #line 4 "..\..\Views\GroupOperation\ChangeDealClient.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 8 "..\..\Views\GroupOperation\ChangeDealClient.cshtml"
            Write(BLResources.ChangeClient);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 9 "..\..\Views\GroupOperation\ChangeDealClient.cshtml"
                  Write(BLResources.ChangeClient);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 10 "..\..\Views\GroupOperation\ChangeDealClient.cshtml"
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

WriteAttribute("href", Tuple.Create(" href=\"", 532), Tuple.Create("\"", 580)
, Tuple.Create(Tuple.Create("", 539), Tuple.Create("/Content/Progress.css?", 539), true)
            
            #line 14 "..\..\Views\GroupOperation\ChangeDealClient.cshtml"
, Tuple.Create(Tuple.Create("", 561), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 561), false)
);

WriteLiteral(" />\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 603), Tuple.Create("\"", 661)
, Tuple.Create(Tuple.Create("", 609), Tuple.Create("/Scripts/Ext.Ajax.syncRequest.js?", 609), true)
            
            #line 16 "..\..\Views\GroupOperation\ChangeDealClient.cshtml"
, Tuple.Create(Tuple.Create("", 642), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 642), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 708), Tuple.Create("\"", 774)
, Tuple.Create(Tuple.Create("", 714), Tuple.Create("/Scripts/DoubleGis.UI.GroupOperations.js?", 714), true)
            
            #line 17 "..\..\Views\GroupOperation\ChangeDealClient.cshtml"
, Tuple.Create(Tuple.Create("", 755), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 755), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.namespace(\'Ext.DoubleGis.UI.Deal\');\r\n\r\n        Ext.DoubleGis.UI.De" +
"al.ChangeClientProcessor = Ext.extend(Ext.DoubleGis.UI.GroupProcessor, {\r\n      " +
"      ClientId: -1,\r\n            ClientLookup: {},\r\n            constructor: fun" +
"ction (config) {\r\n                Ext.apply(config,\r\n                {\r\n        " +
"            listeners:\r\n                    {\r\n                        configspe" +
"cificcontrols: function () {\r\n                            this.ConfigCustomContr" +
"ols();\r\n                        },\r\n                        applyusersettings: f" +
"unction () {\r\n                            this.ApplyUserSettings();\r\n           " +
"             }\r\n                    }\r\n                });\r\n                Ext." +
"DoubleGis.UI.Deal.ChangeClientProcessor.superclass.constructor.call(this, config" +
");\r\n            },\r\n            ConfigCustomControls: function () {\r\n           " +
"     this.ClientLookup = Ext.getCmp(\"Client\");\r\n            },\r\n            IsUs" +
"erSettingsValid: function () {\r\n                this.ClientId = this.ClientLooku" +
"p.getValue() ? this.ClientLookup.getValue().id : this.ClientId;\r\n\r\n             " +
"   if (this.ClientId === -1) {\r\n                    Ext.MessageBox.show({\r\n     " +
"                   title: \'\',\r\n                        msg: Ext.LocalizedResourc" +
"es.NeedToSelectOneOrMoreItems,\r\n                        buttons: window.Ext.Mess" +
"ageBox.OK,\r\n                        width: 300,\r\n                        icon: w" +
"indow.Ext.MessageBox.ERROR\r\n                    });\r\n                    return " +
"false;\r\n                }\r\n\r\n                if (!this.Config || !this.Config.En" +
"tities || this.Config.Entities.length == 0) {\r\n                    Ext.MessageBo" +
"x.show({\r\n                        title: \'\',\r\n                        msg: Ext.L" +
"ocalizedResources.ChangeNeedToPickClient,\r\n                        buttons: wind" +
"ow.Ext.MessageBox.OK,\r\n                        width: 300,\r\n                    " +
"    icon: window.Ext.MessageBox.ERROR\r\n                    });\r\n                " +
"    return false;\r\n                }\r\n\r\n                return true;\r\n          " +
"  },\r\n            ApplyUserSettings: function () {\r\n                this.ClientL" +
"ookup.disable();\r\n            },\r\n            CreateParamsForControllerCall: fun" +
"ction (entityId) {\r\n                return { entityId: entityId, clientId: this." +
"ClientId };\r\n            }\r\n        });\r\n\r\n        Ext.onReady(function () {\r\n\r\n" +
"            var ids = !window.dialogArguments ? [] : (window.dialogArguments.Val" +
"ues ? window.dialogArguments.Values : window.dialogArguments);\r\n\r\n            va" +
"r config =\r\n            {\r\n                Entities: ids,\r\n                Opera" +
"tionName: \'");

            
            #line 85 "..\..\Views\GroupOperation\ChangeDealClient.cshtml"
                           Write(Model.OperationName);

            
            #line default
            #line hidden
WriteLiteral(@"',
                CloseButtonText: Ext.LocalizedResources.Close, // локализованная надпись для кнопки закрыть
                NeedToSelectOneOrMoreItemsMsg: Ext.LocalizedResources.NeedToSelectOneOrMoreItems, // локализованная надпись о том что нужно выбрать один или несколько элементов
                ResultMessageTitle: Ext.LocalizedResources.GroupOperationResultsTitle, // локализованная надпись - заголовок для результатов операции
                ResultMessageTemplate: Ext.LocalizedResources.GroupOperationResultsMessage // локализованная надпись - шаблон строки для результатов операции
            };

            var changeClientProcessor = new Ext.DoubleGis.UI.Deal.ChangeClientProcessor(config);
            if (!changeClientProcessor.CheckProcessingPossibility())
                return;

            changeClientProcessor.Process();
        });

</script>
");

            
            #line 100 "..\..\Views\GroupOperation\ChangeDealClient.cshtml"
    
            
            #line default
            #line hidden
            
            #line 100 "..\..\Views\GroupOperation\ChangeDealClient.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {

            
            #line default
            #line hidden
WriteLiteral("    <table");

WriteLiteral(" cellspacing=\"5\"");

WriteLiteral(" cellpadding=\"5\"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(" height=\"100%\"");

WriteLiteral(">\r\n        <colgroup>\r\n            <col");

WriteLiteral(" width=\"26\"");

WriteLiteral(" />\r\n            <col />\r\n        </colgroup>\r\n        <tbody>\r\n            <tr>\r" +
"\n                <td");

WriteLiteral(" colspan=\"2\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" style=\"height: 24px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(">");

            
            #line 110 "..\..\Views\GroupOperation\ChangeDealClient.cshtml"
                                                             Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                </td>\r\n            </tr>\r\n            <tr>\r\n             " +
"   <td>");

            
            #line 114 "..\..\Views\GroupOperation\ChangeDealClient.cshtml"
               Write(Html.LabelFor(m => m.Client));

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                <td>\r\n                    <table");

WriteLiteral(" style=\"table-layout: fixed\"");

WriteLiteral(" cellspacing=\"0\"");

WriteLiteral(" cellpadding=\"0\"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(">\r\n                        <tbody>\r\n                            <tr>\r\n           " +
"                     <td>\r\n");

WriteLiteral("                                    ");

            
            #line 120 "..\..\Views\GroupOperation\ChangeDealClient.cshtml"
                               Write(Html.LookupFor(m => m.Client, new LookupSettings { EntityName = EntityName.Client, ExtendedInfo = "ForReserve=false" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                            </tr>\r\n     " +
"                   </tbody>\r\n                    </table>\r\n                </td>" +
"\r\n            </tr>\r\n            <tr");

WriteLiteral(" style=\"display: none\"");

WriteLiteral(">\r\n                <td");

WriteLiteral(" colspan=\"2\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" id=\"pbDiv\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" id=\"pbInner\"");

WriteLiteral(">\r\n                        </div>\r\n                    </div>\r\n");

WriteLiteral("                    ");

            
            #line 133 "..\..\Views\GroupOperation\ChangeDealClient.cshtml"
               Write(Html.HiddenFor(m => m.EntityTypeName));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </td>\r\n            </tr>\r\n        </tbody>\r\n    </table>\r\n");

            
            #line 138 "..\..\Views\GroupOperation\ChangeDealClient.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
