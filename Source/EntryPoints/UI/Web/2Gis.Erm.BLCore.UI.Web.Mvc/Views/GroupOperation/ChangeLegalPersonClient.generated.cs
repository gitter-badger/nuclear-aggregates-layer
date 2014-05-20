﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34011
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
    
    #line 1 "..\..\Views\GroupOperation\ChangeLegalPersonClient.cshtml"
    using Platform.Common.Utils;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/GroupOperation/ChangeLegalPersonClient.cshtml")]
    public partial class ChangeLegalPersonClient : System.Web.Mvc.WebViewPage<Models.GroupOperation.ChangeClientViewModel>
    {
        public ChangeLegalPersonClient()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 5 "..\..\Views\GroupOperation\ChangeLegalPersonClient.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 9 "..\..\Views\GroupOperation\ChangeLegalPersonClient.cshtml"
            Write(BLResources.ChangeClient);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 10 "..\..\Views\GroupOperation\ChangeLegalPersonClient.cshtml"
                  Write(BLResources.ChangeClient);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 11 "..\..\Views\GroupOperation\ChangeLegalPersonClient.cshtml"
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

WriteAttribute("href", Tuple.Create(" href=\"", 488), Tuple.Create("\"", 551)
, Tuple.Create(Tuple.Create("", 495), Tuple.Create("/Content/Progress.css?", 495), true)
            
            #line 15 "..\..\Views\GroupOperation\ChangeLegalPersonClient.cshtml"
, Tuple.Create(Tuple.Create("", 517), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 517), false)
);

WriteLiteral(" />\r\n\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 570), Tuple.Create("\"", 643)
, Tuple.Create(Tuple.Create("", 576), Tuple.Create("/Scripts/Ext.Ajax.syncRequest.js?", 576), true)
            
            #line 17 "..\..\Views\GroupOperation\ChangeLegalPersonClient.cshtml"
, Tuple.Create(Tuple.Create("", 609), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 609), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 690), Tuple.Create("\"", 771)
, Tuple.Create(Tuple.Create("", 696), Tuple.Create("/Scripts/DoubleGis.UI.GroupOperations.js?", 696), true)
            
            #line 18 "..\..\Views\GroupOperation\ChangeLegalPersonClient.cshtml"
, Tuple.Create(Tuple.Create("", 737), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 737), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.namespace(\'Ext.DoubleGis.UI.Client\');\r\n        Ext.DoubleGis.UI.Cl" +
"ient.ChangeClientProcessor = Ext.extend(Ext.DoubleGis.UI.GroupProcessor, {\r\n    " +
"        constructor: function (config)\r\n            {\r\n                Ext.apply" +
"(config, {\r\n                    listeners: {\r\n                    }\r\n           " +
"     });\r\n                Ext.DoubleGis.UI.Client.ChangeClientProcessor.supercla" +
"ss.constructor.call(this, config);\r\n            },\r\n            IsUserSettingsVa" +
"lid: function ()\r\n            {\r\n                return true;\r\n            },\r\n " +
"           CreateParamsForControllerCall: function (entityId)\r\n            {\r\n  " +
"              var clientId = Ext.getDom(\"ClientId\").value;\r\n                retu" +
"rn { entityId: entityId, clientId: clientId };\r\n            },\r\n            Vali" +
"dateEntryProcessingSuccessStatus: function (message)\r\n            {\r\n           " +
"     var bypassValidationInfo = window.Ext.decode(message);\r\n                if " +
"(bypassValidationInfo.CanProceed && bypassValidationInfo.CanProceed == true)\r\n  " +
"              {\r\n                    var isOperationContinue = confirm(bypassVal" +
"idationInfo.Message);\r\n                    if (isOperationContinue)\r\n           " +
"         {\r\n                        var params = this.CreateParamsForControllerC" +
"all(bypassValidationInfo.EntityId);\r\n                        params.bypassValida" +
"tion = true;\r\n                        var url = this.EvaluateOperationUrl(null);" +
"\r\n                        this.ProcessSingleEntity(url, params);\r\n\r\n            " +
"            return this.SuccessStatus.ReprocessingRequired;\r\n                   " +
" }\r\n\r\n                    return this.SuccessStatus.Rejected;\r\n                }" +
"\r\n                return this.SuccessStatus.Approved;\r\n            }\r\n        })" +
";\r\n\r\n\r\n        Ext.onReady(function ()\r\n        {\r\n            var ids = !window" +
".dialogArguments ? [] : (window.dialogArguments.Values ? window.dialogArguments." +
"Values : window.dialogArguments);\r\n\r\n            var config = {\r\n               " +
" Entities: ids, // массив id сущностей\r\n                OperationName: \'");

            
            #line 69 "..\..\Views\GroupOperation\ChangeLegalPersonClient.cshtml"
                           Write(Model.OperationName);

            
            #line default
            #line hidden
WriteLiteral("\', // тип операции - Qualify, Assign, ChangeTerritory\r\n                // TODO {a" +
"ll, 18.12.2013}: возможно некоректное отображение диакритики\r\n                Cl" +
"oseButtonText: \'");

            
            #line 71 "..\..\Views\GroupOperation\ChangeLegalPersonClient.cshtml"
                             Write(BLResources.Close);

            
            #line default
            #line hidden
WriteLiteral(@"', // локализованная надпись для кнопки закрыть
                // TODO {all, 18.12.2013}: возможно некоректное отображение диакритики
                // TODO {all, 18.12.2013}: ресурс можно перенести в ClientResourceStorage
                NeedToSelectOneOrMoreItemsMsg: '");

            
            #line 74 "..\..\Views\GroupOperation\ChangeLegalPersonClient.cshtml"
                                           Write(BLResources.NeedToSelectOneOrMoreItems);

            
            #line default
            #line hidden
WriteLiteral(@"', // локализованная надпись о том что нужно выбрать один или несколько элементов
                // TODO {all, 18.12.2013}: возможно некоректное отображение диакритики
                // TODO {all, 18.12.2013}: ресурс можно перенести в ClientResourceStorage
                ResultMessageTitle: '");

            
            #line 77 "..\..\Views\GroupOperation\ChangeLegalPersonClient.cshtml"
                                Write(BLResources.GroupOperationResultsTitle);

            
            #line default
            #line hidden
WriteLiteral(@"', // локализованная надпись - заголовок для результатов операции
                // TODO {all, 18.12.2013}: возможно некоректное отображение диакритики
                // TODO {all, 18.12.2013}: ресурс можно перенести в ClientResourceStorage
                ResultMessageTemplate: '");

            
            #line 80 "..\..\Views\GroupOperation\ChangeLegalPersonClient.cshtml"
                                   Write(BLResources.GroupOperationResultsMessage);

            
            #line default
            #line hidden
WriteLiteral(@"' // локализованная надпись - шаблон строки для результатов операции
            };
            var changeClientProcessor = new Ext.DoubleGis.UI.Client.ChangeClientProcessor(config);
            if (!changeClientProcessor.CheckProcessingPossibility())
            {
                return;
            }

            changeClientProcessor.Process();
        });

    </script>
");

            
            #line 92 "..\..\Views\GroupOperation\ChangeLegalPersonClient.cshtml"
    
            
            #line default
            #line hidden
            
            #line 92 "..\..\Views\GroupOperation\ChangeLegalPersonClient.cshtml"
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

WriteLiteral(" style=\"display: none; height: 15px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(" class=\"Notifications\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 103 "..\..\Views\GroupOperation\ChangeLegalPersonClient.cshtml"
                   Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </div>\r\n                </td>\r\n            </tr>\r\n         " +
"   <tr>\r\n                <td");

WriteLiteral(" colspan=\"2\"");

WriteLiteral(">\r\n                    &nbsp;\r\n                </td>\r\n            </tr>\r\n        " +
"    <tr>\r\n                <td");

WriteLiteral(" valign=\"top\"");

WriteLiteral(">\r\n                </td>\r\n                <td");

WriteLiteral(" valign=\"top\"");

WriteLiteral(">\r\n                    <table");

WriteLiteral(" style=\"table-layout: fixed\"");

WriteLiteral(" cellspacing=\"0\"");

WriteLiteral(" cellpadding=\"0\"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(">\r\n                        <tbody>\r\n                            <tr>\r\n           " +
"                     <td>\r\n");

WriteLiteral("                                    ");

            
            #line 120 "..\..\Views\GroupOperation\ChangeLegalPersonClient.cshtml"
                               Write(Html.LookupFor(m => m.Client, new LookupSettings { EntityName = EntityName.Client}));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                            </tr>\r\n     " +
"                   </tbody>\r\n                    </table>\r\n                </td>" +
"\r\n            </tr>\r\n            <tr");

WriteLiteral(" style=\"display: none\"");

WriteLiteral(">\r\n                <td>\r\n                    <div");

WriteLiteral(" id=\"pbDiv\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" id=\"pbInner\"");

WriteLiteral(">\r\n                        </div>\r\n                    </div>\r\n");

WriteLiteral("                    ");

            
            #line 133 "..\..\Views\GroupOperation\ChangeLegalPersonClient.cshtml"
               Write(Html.HiddenFor(m => m.EntityTypeName));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </td>\r\n            </tr>\r\n        </tbody>\r\n    </table>\r\n");

            
            #line 138 "..\..\Views\GroupOperation\ChangeLegalPersonClient.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
