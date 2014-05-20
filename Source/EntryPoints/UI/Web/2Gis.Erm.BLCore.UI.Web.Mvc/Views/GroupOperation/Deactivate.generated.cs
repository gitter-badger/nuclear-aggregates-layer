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
    
    #line 1 "..\..\Views\GroupOperation\Deactivate.cshtml"
    using DoubleGis.Erm.BLCore.UI.Metadata.Confirmations;
    
    #line default
    #line hidden
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
    using DoubleGis.Erm.Platform.Common;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.Model.Entities.Enums;
    using DoubleGis.Erm.Platform.Model.Metadata.Enums;
    using DoubleGis.Erm.Platform.UI.Web.Mvc;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    
    #line 2 "..\..\Views\GroupOperation\Deactivate.cshtml"
    using Platform.Common.Utils;
    
    #line default
    #line hidden
    
    #line 3 "..\..\Views\GroupOperation\Deactivate.cshtml"
    using Platform.Model.Identities.Operations.Identity;
    
    #line default
    #line hidden
    
    #line 4 "..\..\Views\GroupOperation\Deactivate.cshtml"
    using Platform.Model.Identities.Operations.Identity.Generic;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/GroupOperation/Deactivate.cshtml")]
    public partial class Deactivate : System.Web.Mvc.WebViewPage<Models.GroupOperation.GroupOperationViewModel>
    {
        public Deactivate()
        {
        }
        public override void Execute()
        {
            
            #line 7 "..\..\Views\GroupOperation\Deactivate.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 11 "..\..\Views\GroupOperation\Deactivate.cshtml"
            Write(BLResources.DeactivateConfirmation);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 12 "..\..\Views\GroupOperation\Deactivate.cshtml"
                  Write(BLResources.DeactivateConfirmation);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 13 "..\..\Views\GroupOperation\Deactivate.cshtml"
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

WriteAttribute("href", Tuple.Create(" href=\"", 679), Tuple.Create("\"", 742)
, Tuple.Create(Tuple.Create("", 686), Tuple.Create("/Content/Progress.css?", 686), true)
            
            #line 17 "..\..\Views\GroupOperation\Deactivate.cshtml"
, Tuple.Create(Tuple.Create("", 708), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 708), false)
);

WriteLiteral(" />\r\n\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 761), Tuple.Create("\"", 834)
, Tuple.Create(Tuple.Create("", 767), Tuple.Create("/Scripts/Ext.Ajax.syncRequest.js?", 767), true)
            
            #line 19 "..\..\Views\GroupOperation\Deactivate.cshtml"
, Tuple.Create(Tuple.Create("", 800), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 800), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 881), Tuple.Create("\"", 962)
, Tuple.Create(Tuple.Create("", 887), Tuple.Create("/Scripts/DoubleGis.UI.GroupOperations.js?", 887), true)
            
            #line 20 "..\..\Views\GroupOperation\Deactivate.cshtml"
, Tuple.Create(Tuple.Create("", 928), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 928), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1009), Tuple.Create("\"", 1069)
, Tuple.Create(Tuple.Create("", 1015), Tuple.Create("/Scripts/Tooltip.js?", 1015), true)
            
            #line 21 "..\..\Views\GroupOperation\Deactivate.cshtml"
, Tuple.Create(Tuple.Create("", 1035), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1035), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.namespace(\'Ext.DoubleGis.UI.Activate\');\r\n        Ext.DoubleGis.UI." +
"Activate.DeactivateProcessor = Ext.extend(Ext.DoubleGis.UI.GroupProcessor, {\r\n  " +
"          constructor: function (config) {\r\n                Ext.DoubleGis.UI.Act" +
"ivate.DeactivateProcessor.superclass.constructor.call(this, config);\r\n          " +
"  },\r\n            IsUserSettingsValid: function () {\r\n                return tru" +
"e;\r\n            },\r\n            CreateParamsForControllerCall: function (entityI" +
"d) {\r\n                return { entityId: entityId };\r\n            },\r\n          " +
"  ValidateEntryProcessingSuccessStatus: function (message) {\r\n                va" +
"r bypassValidationInfo = window.Ext.decode(message);\r\n                if (bypass" +
"ValidationInfo.ConfirmationMessage) {\r\n                    var isOperationContin" +
"ue = confirm(bypassValidationInfo.ConfirmationMessage);\r\n                    if " +
"(isOperationContinue) {\r\n                        var params = this.CreateParamsF" +
"orControllerCall(bypassValidationInfo.Id);\r\n                        params.bypas" +
"sValidation = true;\r\n                        var url = this.EvaluateOperationUrl" +
"();\r\n                        this.ProcessSingleEntity(url, params);\r\n\r\n         " +
"               return this.SuccessStatus.ReprocessingRequired;\r\n                " +
"    }\r\n\r\n                    return this.SuccessStatus.Rejected;\r\n              " +
"  }\r\n                return this.SuccessStatus.Approved;\r\n            }\r\n       " +
" });\r\n        Ext.onReady(function () {\r\n            Ext.getDom(\'PageContentCell" +
"\').style[\"vertical-align\"] = \"top\";\r\n\r\n            var ids = !window.dialogArgum" +
"ents ? [] : (window.dialogArguments.Values ? window.dialogArguments.Values : win" +
"dow.dialogArguments);\r\n\r\n            var config = {\r\n                Entities: i" +
"ds, // массив id сущностей\r\n                OperationName: \'");

            
            #line 60 "..\..\Views\GroupOperation\Deactivate.cshtml"
                           Write(Model.OperationName);

            
            #line default
            #line hidden
WriteLiteral("\', // тип операции - Qualify, Assign, ChangeTerritory\r\n                // TODO {a" +
"ll, 18.12.2013}: возможно некоректное отображение диакритики\r\n                Cl" +
"oseButtonText: \'");

            
            #line 62 "..\..\Views\GroupOperation\Deactivate.cshtml"
                             Write(BLResources.Close);

            
            #line default
            #line hidden
WriteLiteral(@"', // локализованная надпись для кнопки закрыть
                // TODO {all, 18.12.2013}: возможно некоректное отображение диакритики
                // TODO {all, 18.12.2013}: ресурс можно перенести в ClientResourceStorage
                NeedToSelectOneOrMoreItemsMsg: '");

            
            #line 65 "..\..\Views\GroupOperation\Deactivate.cshtml"
                                           Write(BLResources.NeedToSelectOneOrMoreItems);

            
            #line default
            #line hidden
WriteLiteral(@"', // локализованная надпись о том что нужно выбрать один или несколько элементов
                // TODO {all, 18.12.2013}: возможно некоректное отображение диакритики
                // TODO {all, 18.12.2013}: ресурс можно перенести в ClientResourceStorage
                ResultMessageTitle: '");

            
            #line 68 "..\..\Views\GroupOperation\Deactivate.cshtml"
                                Write(BLResources.GroupOperationResultsTitle);

            
            #line default
            #line hidden
WriteLiteral(@"', // локализованная надпись - заголовок для результатов операции
                // TODO {all, 18.12.2013}: возможно некоректное отображение диакритики
                // TODO {all, 18.12.2013}: ресурс можно перенести в ClientResourceStorage
                ResultMessageTemplate: '");

            
            #line 71 "..\..\Views\GroupOperation\Deactivate.cshtml"
                                   Write(BLResources.GroupOperationResultsMessage);

            
            #line default
            #line hidden
WriteLiteral(@"' // локализованная надпись - шаблон строки для результатов операции
            };
            var deactivateProcessor = new Ext.DoubleGis.UI.Activate.DeactivateProcessor(config);
            if (!deactivateProcessor.CheckProcessingPossibility()){
                return;
            }
            deactivateProcessor.Process();
        });
    </script>
    <table");

WriteLiteral(" cellspacing=\"5\"");

WriteLiteral(" cellpadding=\"5\"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(" height=\"100px\"");

WriteLiteral(">\r\n        <tr>\r\n            <td");

WriteLiteral(" colspan=\"2\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" style=\"height: 30px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteAttribute("onmouseover", Tuple.Create(" \r\n                        onmouseover=\"", 4700), Tuple.Create("\"", 4767)
, Tuple.Create(Tuple.Create("", 4740), Tuple.Create("AddTooltip(", 4740), true)
            
            #line 84 "..\..\Views\GroupOperation\Deactivate.cshtml"
, Tuple.Create(Tuple.Create("", 4751), Tuple.Create<System.Object, System.Int32>(Model.Message
            
            #line default
            #line hidden
, 4751), false)
, Tuple.Create(Tuple.Create("", 4765), Tuple.Create(");", 4765), true)
);

WriteLiteral(" \r\n                        onmouseout=\"RemoveTooltip();\"");

WriteLiteral(">\r\n");

WriteLiteral("                    ");

            
            #line 86 "..\..\Views\GroupOperation\Deactivate.cshtml"
               Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n                </div>\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n       " +
"     <td>\r\n                <span");

WriteLiteral(" id=\"bodyMessage\"");

WriteLiteral(" style=\"display: none\"");

WriteLiteral("></span>\r\n                <span>\r\n");

WriteLiteral("                    ");

            
            #line 94 "..\..\Views\GroupOperation\Deactivate.cshtml"
               Write(ConfirmationManager.GetConfirmation(new StrictOperationIdentity(DeactivateIdentity.Instance, new EntitySet(Model.EntityTypeName))));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </span>\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n      " +
"      <td>\r\n                <div");

WriteLiteral(" id=\"pbDiv\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" id=\"pbInner\"");

WriteLiteral(">\r\n                    </div>\r\n                </div>\r\n            </td>\r\n       " +
" </tr>\r\n    </table>\r\n");

WriteLiteral("    ");

            
            #line 107 "..\..\Views\GroupOperation\Deactivate.cshtml"
Write(Html.HiddenFor(x => x.EntityTypeName));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

});

        }
    }
}
#pragma warning restore 1591
