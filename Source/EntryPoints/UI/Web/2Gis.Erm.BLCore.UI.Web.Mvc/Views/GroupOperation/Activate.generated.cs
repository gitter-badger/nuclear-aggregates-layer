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
    
    #line 1 "..\..\Views\GroupOperation\Activate.cshtml"
    using DoubleGis.Erm.BLCore.UI.Metadata.Confirmations;
    
    #line default
    #line hidden
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
    using DoubleGis.Erm.Platform.Common;
    
    #line 2 "..\..\Views\GroupOperation\Activate.cshtml"
    using DoubleGis.Erm.Platform.Common.Utils;
    
    #line default
    #line hidden
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.Model.Entities.Enums;
    
    #line 3 "..\..\Views\GroupOperation\Activate.cshtml"
    using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;
    
    #line default
    #line hidden
    
    #line 4 "..\..\Views\GroupOperation\Activate.cshtml"
    using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
    
    #line default
    #line hidden
    using DoubleGis.Erm.Platform.Model.Metadata.Enums;
    using DoubleGis.Erm.Platform.UI.Web.Mvc;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Security;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/GroupOperation/Activate.cshtml")]
    public partial class Activate : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLCore.UI.Web.Mvc.Models.GroupOperation.GroupOperationViewModel>
    {
        public Activate()
        {
        }
        public override void Execute()
        {
            
            #line 7 "..\..\Views\GroupOperation\Activate.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 11 "..\..\Views\GroupOperation\Activate.cshtml"
            Write(BLResources.ActivateConfirmation);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 12 "..\..\Views\GroupOperation\Activate.cshtml"
                  Write(BLResources.ActivateConfirmation);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 13 "..\..\Views\GroupOperation\Activate.cshtml"
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

WriteAttribute("href", Tuple.Create(" href=\"", 749), Tuple.Create("\"", 812)
, Tuple.Create(Tuple.Create("", 756), Tuple.Create("/Content/Progress.css?", 756), true)
            
            #line 17 "..\..\Views\GroupOperation\Activate.cshtml"
, Tuple.Create(Tuple.Create("", 778), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 778), false)
);

WriteLiteral(" />\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 835), Tuple.Create("\"", 908)
, Tuple.Create(Tuple.Create("", 841), Tuple.Create("/Scripts/Ext.Ajax.syncRequest.js?", 841), true)
            
            #line 19 "..\..\Views\GroupOperation\Activate.cshtml"
, Tuple.Create(Tuple.Create("", 874), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 874), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 955), Tuple.Create("\"", 1036)
, Tuple.Create(Tuple.Create("", 961), Tuple.Create("/Scripts/DoubleGis.UI.GroupOperations.js?", 961), true)
            
            #line 20 "..\..\Views\GroupOperation\Activate.cshtml"
, Tuple.Create(Tuple.Create("", 1002), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1002), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1083), Tuple.Create("\"", 1143)
, Tuple.Create(Tuple.Create("", 1089), Tuple.Create("/Scripts/Tooltip.js?", 1089), true)
            
            #line 21 "..\..\Views\GroupOperation\Activate.cshtml"
, Tuple.Create(Tuple.Create("", 1109), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1109), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">
        Ext.namespace('Ext.DoubleGis.UI.Activate');
        Ext.DoubleGis.UI.Activate.ActivateProcessor = Ext.extend(Ext.DoubleGis.UI.GroupProcessor, {
            constructor: function (config) {
                Ext.DoubleGis.UI.Activate.ActivateProcessor.superclass.constructor.call(this, config);
            },
            IsUserSettingsValid: function () {
                return true;
            },
            CreateParamsForControllerCall: function (entityId) {
                return { entityId: entityId };
            }
        });
        Ext.onReady(function () {
            Ext.getDom('PageContentCell').style[""vertical-align""] = ""top"";

            var ids = !window.dialogArguments ? [] : (window.dialogArguments.Values ? window.dialogArguments.Values : window.dialogArguments);

            var config = {
                Entities: ids, // массив id сущностей
                OperationName: '");

            
            #line 43 "..\..\Views\GroupOperation\Activate.cshtml"
                           Write(Model.OperationName);

            
            #line default
            #line hidden
WriteLiteral(@"', // тип операции - Qualify, Assign, ChangeTerritory
                CloseButtonText: Ext.LocalizedResources.Close, // локализованная надпись для кнопки закрыть
                NeedToSelectOneOrMoreItemsMsg: Ext.LocalizedResources.NeedToSelectOneOrMoreItems, // локализованная надпись о том что нужно выбрать один или несколько элементов
                ResultMessageTitle: Ext.LocalizedResources.GroupOperationResultsTitle, // локализованная надпись - заголовок для результатов операции
                ResultMessageTemplate: Ext.LocalizedResources.GroupOperationResultsMessage // локализованная надпись - шаблон строки для результатов операции
            };
            var activateProcessor = new Ext.DoubleGis.UI.Activate.ActivateProcessor(config);
            if (!activateProcessor.CheckProcessingPossibility()){
                return;
            }
            activateProcessor.Process();
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

WriteAttribute("onmouseover", Tuple.Create(" \r\n                        onmouseover=\"", 3285), Tuple.Create("\"", 3352)
, Tuple.Create(Tuple.Create("", 3325), Tuple.Create("AddTooltip(", 3325), true)
            
            #line 60 "..\..\Views\GroupOperation\Activate.cshtml"
, Tuple.Create(Tuple.Create("", 3336), Tuple.Create<System.Object, System.Int32>(Model.Message
            
            #line default
            #line hidden
, 3336), false)
, Tuple.Create(Tuple.Create("", 3350), Tuple.Create(");", 3350), true)
);

WriteLiteral(" \r\n                        onmouseout=\"RemoveTooltip();\"");

WriteLiteral(">\r\n");

WriteLiteral("                    ");

            
            #line 62 "..\..\Views\GroupOperation\Activate.cshtml"
               Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n                </div>\r\n            </td>\r\n        </tr>\r\n        <tr>\r\n       " +
"     <td>\r\n                <span");

WriteLiteral(" id=\"bodyMessage\"");

WriteLiteral(" style=\"display: none\"");

WriteLiteral("></span><span>\r\n");

WriteLiteral("                    ");

            
            #line 69 "..\..\Views\GroupOperation\Activate.cshtml"
               Write(ConfirmationManager.GetConfirmation(new StrictOperationIdentity(ActivateIdentity.Instance, new EntitySet(Model.EntityTypeName))));

            
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

            
            #line 82 "..\..\Views\GroupOperation\Activate.cshtml"
Write(Html.HiddenFor(x => x.EntityTypeName));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

});

        }
    }
}
#pragma warning restore 1591
