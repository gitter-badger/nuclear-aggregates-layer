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
    
    #line 1 "..\..\Views\GroupOperation\ChangeClientTerritory.cshtml"
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/GroupOperation/ChangeClientTerritory.cshtml")]
    public partial class ChangeClientTerritory : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLCore.UI.Web.Mvc.Models.GroupOperation.ChangeTerritoryViewModel>
    {
        public ChangeClientTerritory()
        {
        }
        public override void Execute()
        {
            
            #line 4 "..\..\Views\GroupOperation\ChangeClientTerritory.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 8 "..\..\Views\GroupOperation\ChangeClientTerritory.cshtml"
            Write(BLResources.ChangeTerritory);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 9 "..\..\Views\GroupOperation\ChangeClientTerritory.cshtml"
                  Write(BLResources.ChangeTerritory);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 10 "..\..\Views\GroupOperation\ChangeClientTerritory.cshtml"
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

WriteAttribute("href", Tuple.Create(" href=\"", 541), Tuple.Create("\"", 604)
, Tuple.Create(Tuple.Create("", 548), Tuple.Create("/Content/Progress.css?", 548), true)
            
            #line 14 "..\..\Views\GroupOperation\ChangeClientTerritory.cshtml"
, Tuple.Create(Tuple.Create("", 570), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 570), false)
);

WriteLiteral(" />\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 627), Tuple.Create("\"", 700)
, Tuple.Create(Tuple.Create("", 633), Tuple.Create("/Scripts/Ext.Ajax.syncRequest.js?", 633), true)
            
            #line 16 "..\..\Views\GroupOperation\ChangeClientTerritory.cshtml"
, Tuple.Create(Tuple.Create("", 666), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 666), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 747), Tuple.Create("\"", 828)
, Tuple.Create(Tuple.Create("", 753), Tuple.Create("/Scripts/DoubleGis.UI.GroupOperations.js?", 753), true)
            
            #line 17 "..\..\Views\GroupOperation\ChangeClientTerritory.cshtml"
, Tuple.Create(Tuple.Create("", 794), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 794), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.namespace(\'Ext.DoubleGis.UI.Client\');\r\n        Ext.DoubleGis.UI.Cl" +
"ient.ChangeTerritoryProcessor = Ext.extend(Ext.DoubleGis.UI.GroupProcessor, {\r\n " +
"           TerritoryId: -1,\r\n            TerritoryLookup: {},\r\n            const" +
"ructor: function (config)\r\n            {\r\n                Ext.apply(config, {\r\n " +
"                   listeners: {\r\n                        configspecificcontrols:" +
" function ()\r\n                        {\r\n                            this.Config" +
"CustomControls();\r\n                        },\r\n                        applyuser" +
"settings: function ()\r\n                        {\r\n                            th" +
"is.ApplyUserSettings();\r\n                        }\r\n                    }\r\n     " +
"           });\r\n                Ext.DoubleGis.UI.Client.ChangeTerritoryProcessor" +
".superclass.constructor.call(this, config);\r\n            },\r\n            ConfigC" +
"ustomControls: function ()\r\n            {\r\n                this.TerritoryLookup " +
"= Ext.getCmp(\"Territory\");\r\n            },\r\n            IsUserSettingsValid: fun" +
"ction ()\r\n            {\r\n                this.TerritoryId = this.TerritoryLookup" +
".getValue() ? this.TerritoryLookup.getValue().id : this.TerritoryId;\r\n          " +
"      if (this.TerritoryId === -1)\r\n                {\r\n                    Ext.M" +
"essageBox.show({\r\n                        title: \'\',\r\n                        ms" +
"g: Ext.LocalizedResources.NeedToSelectOneOrMoreItems,\r\n                        b" +
"uttons: window.Ext.MessageBox.OK,\r\n                        width: 300,\r\n        " +
"                icon: window.Ext.MessageBox.ERROR\r\n                    });\r\n    " +
"                return false;\r\n                }\r\n\r\n                return true;" +
"\r\n            },\r\n            ApplyUserSettings: function ()\r\n            {\r\n   " +
"             this.TerritoryLookup.disable();\r\n            },\r\n            Create" +
"ParamsForControllerCall: function (entityId)\r\n            {\r\n                ret" +
"urn { entityId: entityId, territoryId: this.TerritoryId };\r\n            }\r\n     " +
"   });\r\n        Ext.onReady(function ()\r\n        {\r\n            var ids = !windo" +
"w.dialogArguments ? [] : (window.dialogArguments.Values ? window.dialogArguments" +
".Values : window.dialogArguments);\r\n\r\n            var config = {\r\n              " +
"  Entities: ids, // массив id сущностей\r\n                OperationName: \'");

            
            #line 76 "..\..\Views\GroupOperation\ChangeClientTerritory.cshtml"
                           Write(Model.OperationName);

            
            #line default
            #line hidden
WriteLiteral(@"', // тип операции - Qualify, Assign, ChangeTerritory
                CloseButtonText: Ext.LocalizedResources.Close, // локализованная надпись для кнопки закрыть
                NeedToSelectOneOrMoreItemsMsg: Ext.LocalizedResources.NeedToSelectOneOrMoreItems, // локализованная надпись о том что нужно выбрать один или несколько элементов
                ResultMessageTitle: Ext.LocalizedResources.GroupOperationResultsTitle, // локализованная надпись - заголовок для результатов операции
                ResultMessageTemplate: Ext.LocalizedResources.GroupOperationResultsMessage // локализованная надпись - шаблон строки для результатов операции
            };
            var changeTerritoryProcessor = new Ext.DoubleGis.UI.Client.ChangeTerritoryProcessor(config);
            if (!changeTerritoryProcessor.CheckProcessingPossibility())
            {
                return;
            }

            changeTerritoryProcessor.Process();
        });
    </script>
");

            
            #line 91 "..\..\Views\GroupOperation\ChangeClientTerritory.cshtml"
    
            
            #line default
            #line hidden
            
            #line 91 "..\..\Views\GroupOperation\ChangeClientTerritory.cshtml"
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

            
            #line 101 "..\..\Views\GroupOperation\ChangeClientTerritory.cshtml"
                                                             Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                </td>\r\n            </tr>\r\n            <tr>\r\n             " +
"   <td");

WriteLiteral(" valign=\"top\"");

WriteLiteral(">\r\n\r\n                </td>\r\n                <td");

WriteLiteral(" valign=\"top\"");

WriteLiteral(">\r\n                    <label");

WriteLiteral(" style=\"font-weight: bold\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 110 "..\..\Views\GroupOperation\ChangeClientTerritory.cshtml"
                   Write(BLResources.CrmTerritory);

            
            #line default
            #line hidden
WriteLiteral("</label>\r\n                    <br />\r\n                    <div");

WriteLiteral(" style=\"padding-bottom: 10px; color: #444444; padding-top: 5px\"");

WriteLiteral(">\r\n                        <label>");

            
            #line 113 "..\..\Views\GroupOperation\ChangeClientTerritory.cshtml"
                          Write(BLResources.ClientTerritoryChangeLegend);

            
            #line default
            #line hidden
WriteLiteral("</label>\r\n                    </div>\r\n                    <table");

WriteLiteral(" style=\"table-layout: fixed\"");

WriteLiteral(" cellspacing=\"0\"");

WriteLiteral(" cellpadding=\"0\"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(">\r\n                        <tbody>\r\n                            <tr>\r\n           " +
"                     <td>\r\n");

WriteLiteral("                                    ");

            
            #line 119 "..\..\Views\GroupOperation\ChangeClientTerritory.cshtml"
                               Write(Html.LookupFor(m => m.Territory, new LookupSettings { EntityName = EntityName.Territory}));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </td>\r\n                            </tr>\r\n     " +
"                   </tbody>\r\n                    </table>\r\n                </td>" +
"\r\n            </tr>\r\n            <tr");

WriteLiteral(" style=\"display:none;\"");

WriteLiteral(">\r\n                <td");

WriteLiteral(" colspan=\"2\"");

WriteLiteral(">\r\n");

WriteLiteral("                    ");

            
            #line 128 "..\..\Views\GroupOperation\ChangeClientTerritory.cshtml"
               Write(Html.HiddenFor(m => m.EntityTypeName));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </td>\r\n            </tr>\r\n        </tbody>\r\n        <tbody>\r\n  " +
"          <tr>\r\n                <td");

WriteLiteral(" colspan=\"2\"");

WriteLiteral(" style=\"padding-left: 10px;\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" id=\"pbDiv\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" id=\"pbInner\"");

WriteLiteral(">\r\n                        </div>\r\n                    </div>\r\n                </" +
"td>\r\n            </tr>\r\n        </tbody>\r\n    </table>\r\n");

            
            #line 143 "..\..\Views\GroupOperation\ChangeClientTerritory.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
