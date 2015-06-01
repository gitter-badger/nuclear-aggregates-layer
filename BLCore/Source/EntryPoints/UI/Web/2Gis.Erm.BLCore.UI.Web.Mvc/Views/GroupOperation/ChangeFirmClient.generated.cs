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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/GroupOperation/ChangeFirmClient.cshtml")]
    public partial class ChangeFirmClient : System.Web.Mvc.WebViewPage<ChangeClientViewModel>
    {
        public ChangeFirmClient()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\GroupOperation\ChangeFirmClient.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 7 "..\..\Views\GroupOperation\ChangeFirmClient.cshtml"
            Write(BLResources.ChangeClient);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 8 "..\..\Views\GroupOperation\ChangeFirmClient.cshtml"
                  Write(BLResources.ChangeClient);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 9 "..\..\Views\GroupOperation\ChangeFirmClient.cshtml"
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

WriteAttribute("href", Tuple.Create(" href=\"", 434), Tuple.Create("\"", 482)
, Tuple.Create(Tuple.Create("", 441), Tuple.Create("/Content/Progress.css?", 441), true)
            
            #line 13 "..\..\Views\GroupOperation\ChangeFirmClient.cshtml"
, Tuple.Create(Tuple.Create("", 463), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 463), false)
);

WriteLiteral(" />\r\n\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 501), Tuple.Create("\"", 559)
, Tuple.Create(Tuple.Create("", 507), Tuple.Create("/Scripts/Ext.Ajax.syncRequest.js?", 507), true)
            
            #line 15 "..\..\Views\GroupOperation\ChangeFirmClient.cshtml"
, Tuple.Create(Tuple.Create("", 540), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 540), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 606), Tuple.Create("\"", 672)
, Tuple.Create(Tuple.Create("", 612), Tuple.Create("/Scripts/DoubleGis.UI.GroupOperations.js?", 612), true)
            
            #line 16 "..\..\Views\GroupOperation\ChangeFirmClient.cshtml"
, Tuple.Create(Tuple.Create("", 653), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 653), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.namespace(\'Ext.DoubleGis.UI.Firm\');\r\n\r\n        Ext.DoubleGis.UI.Fi" +
"rm.ChangeClientProcessor = Ext.extend(Ext.DoubleGis.UI.GroupProcessor, {\r\n      " +
"      ClientId: -1,\r\n            ClientLookup: {},\r\n            constructor: fun" +
"ction (config)\r\n            {\r\n                Ext.apply(config,\r\n              " +
"  {\r\n                    listeners:\r\n                    {\r\n                    " +
"    configspecificcontrols: function ()\r\n                        {\r\n            " +
"                this.ConfigCustomControls();\r\n                        },\r\n      " +
"                  applyusersettings: function ()\r\n                        {\r\n   " +
"                         this.ApplyUserSettings();\r\n                        }\r\n " +
"                   }\r\n                });\r\n                Ext.DoubleGis.UI.Firm" +
".ChangeClientProcessor.superclass.constructor.call(this, config);\r\n            }" +
",\r\n            ConfigCustomControls: function ()\r\n            {\r\n               " +
" this.ClientLookup = Ext.getCmp(\"Client\");\r\n            },\r\n            IsUserSe" +
"ttingsValid: function ()\r\n            {\r\n                this.ClientId = this.Cl" +
"ientLookup.getValue() ? this.ClientLookup.getValue().id : this.ClientId;\r\n\r\n    " +
"            if (this.ClientId === -1)\r\n                {\r\n                    Ex" +
"t.MessageBox.show({\r\n                        title: \'\',\r\n                       " +
" msg: Ext.LocalizedResources.NeedToSelectOneOrMoreItems,\r\n                      " +
"  buttons: window.Ext.MessageBox.OK,\r\n                        width: 300,\r\n     " +
"                   icon: window.Ext.MessageBox.ERROR\r\n                    });\r\n " +
"                   return false;\r\n                }\r\n\r\n                if (!this" +
".Config || !this.Config.Entities || this.Config.Entities.length == 0)\r\n         " +
"       {\r\n                    Ext.MessageBox.show({\r\n                        tit" +
"le: \'\',\r\n                        msg: \'");

            
            #line 66 "..\..\Views\GroupOperation\ChangeFirmClient.cshtml"
                         Write(BLResources.NoFirmsSelected);

            
            #line default
            #line hidden
WriteLiteral(@"',
                        buttons: window.Ext.MessageBox.OK,
                        width: 300,
                        icon: window.Ext.MessageBox.ERROR
                    });
                    return false;
                }

                for (var i = 0; i < this.Config.Entities.length; i++)
                {
                    var firmId = this.Config.Entities[i];

                    var changeClientValidationResponse = window.Ext.Ajax.syncRequest({
                        method: 'POST',
                        url: Ext.BasicOperationsServiceRestUrl + 'ChangeClient.svc/Rest/Validate/Firm/' + firmId + ""/"" + this.ClientId
                    });

                    if ((changeClientValidationResponse.conn.status >= 200 && changeClientValidationResponse.conn.status < 300) ||
                        (Ext.isIE && changeClientValidationResponse.conn.status == 1223))
                    {
                        var validationResult = Ext.decode(changeClientValidationResponse.conn.responseText);

                        if (validationResult.Errors && validationResult.Errors.length != 0)
                        {
                            Ext.MessageBox.show({
                                title: '',
                                msg: '");

            
            #line 92 "..\..\Views\GroupOperation\ChangeFirmClient.cshtml"
                                 Write(BLResources.ChangeFirmClientIsNotPossibleDueToErrors);

            
            #line default
            #line hidden
WriteLiteral(@"' + '\r\n' + validationResult.Errors.join('\r\n'),
                                buttons: window.Ext.MessageBox.OK,
                                width: 300,
                                icon: window.Ext.MessageBox.ERROR
                            });

                            return false;
                        } else if (validationResult.Warnings && validationResult.Warnings.length != 0)
                        {
                            var dialogResult = confirm('");

            
            #line 101 "..\..\Views\GroupOperation\ChangeFirmClient.cshtml"
                                                   Write(BLResources.ChangeFirmClientIgnoreWarnings);

            
            #line default
            #line hidden
WriteLiteral(@"' + ' \r\n' +
                                validationResult.Warnings.join('\r\n') + '?');
                            if (dialogResult)
                                return true;
                            else
                                return false;
                        }
                    } else
                    {
                        alert(changeClientValidationResponse.conn.responseText);
                    }
                }

                return true;
            },
            ApplyUserSettings: function ()
            {
                this.ClientLookup.disable();
            },
            CreateParamsForControllerCall: function (entityId)
            {
                return { entityId: entityId, clientId: this.ClientId };
            }
        });

        Ext.onReady(function ()
        {
            var ids = !window.dialogArguments ? [] : (window.dialogArguments.Values ? window.dialogArguments.Values : window.dialogArguments);

            var config =
            {
                Entities: ids,
                OperationName: '");

            
            #line 133 "..\..\Views\GroupOperation\ChangeFirmClient.cshtml"
                           Write(Model.OperationName);

            
            #line default
            #line hidden
WriteLiteral(@"',
                CloseButtonText: Ext.LocalizedResources.Close, // локализованная надпись для кнопки закрыть
                NeedToSelectOneOrMoreItemsMsg: Ext.LocalizedResources.NeedToSelectOneOrMoreItems, // локализованная надпись о том что нужно выбрать один или несколько элементов
                ResultMessageTitle: Ext.LocalizedResources.GroupOperationResultsTitle, // локализованная надпись - заголовок для результатов операции
                ResultMessageTemplate: Ext.LocalizedResources.GroupOperationResultsMessage // локализованная надпись - шаблон строки для результатов операции
            };

            var changeClientProcessor = new Ext.DoubleGis.UI.Firm.ChangeClientProcessor(config);
            if (!changeClientProcessor.CheckProcessingPossibility())
                return;

            changeClientProcessor.Process();
        });

    </script>
");

            
            #line 148 "..\..\Views\GroupOperation\ChangeFirmClient.cshtml"
    
            
            #line default
            #line hidden
            
            #line 148 "..\..\Views\GroupOperation\ChangeFirmClient.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 151 "..\..\Views\GroupOperation\ChangeFirmClient.cshtml"
   Write(Html.Hidden("EntityType", Model.EntityTypeName.Description));

            
            #line default
            #line hidden
WriteLiteral("\r\n        \r\n        ");

WriteLiteral("\r\n        ");

WriteLiteral("\r\n        \r\n        <div");

WriteLiteral(" style=\"display: none; height: 15px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(" class=\"Notifications\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 157 "..\..\Views\GroupOperation\ChangeFirmClient.cshtml"
       Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 160 "..\..\Views\GroupOperation\ChangeFirmClient.cshtml"
       Write(Html.TemplateField(x => x.Client, FieldFlex.lone, new LookupSettings { EntityName = EntityType.Instance.Client(), ExtendedInfo = "ForReserve=false" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" id=\"pbDiv\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" id=\"pbInner\"");

WriteLiteral(">\r\n            </div>\r\n        </div>\r\n    </div>\r\n");

            
            #line 167 "..\..\Views\GroupOperation\ChangeFirmClient.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
