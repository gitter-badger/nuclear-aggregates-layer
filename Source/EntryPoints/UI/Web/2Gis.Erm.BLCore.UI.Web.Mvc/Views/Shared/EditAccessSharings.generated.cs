﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.0
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Views.Shared
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
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.Model.Entities.Enums;
    using DoubleGis.Erm.Platform.Model.Metadata.Enums;
    using DoubleGis.Erm.Platform.UI.Web.Mvc;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Security;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Shared/EditAccessSharings.cshtml")]
    public partial class EditAccessSharings : System.Web.Mvc.WebViewPage<AccessSharingModel>
    {
        public EditAccessSharings()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\Shared\EditAccessSharings.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 7 "..\..\Views\Shared\EditAccessSharings.cshtml"
            Write(BLResources.AccessSharingFormHeader);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 8 "..\..\Views\Shared\EditAccessSharings.cshtml"
                  Write(BLResources.AccessSharingFormHeader);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 9 "..\..\Views\Shared\EditAccessSharings.cshtml"
                    Write(BLResources.AccessSharingFormDescription);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

WriteLiteral("\r\n");

DefineSection("PageContent", () => {

WriteLiteral("\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 345), Tuple.Create("\"", 394)
, Tuple.Create(Tuple.Create("", 352), Tuple.Create("/Content/GridStyle.css?", 352), true)
            
            #line 13 "..\..\Views\Shared\EditAccessSharings.cshtml"
, Tuple.Create(Tuple.Create("", 375), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 375), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 442), Tuple.Create("\"", 503)
, Tuple.Create(Tuple.Create("", 449), Tuple.Create("/Content/EditAccessSharingList.css?", 449), true)
            
            #line 14 "..\..\Views\Shared\EditAccessSharings.cshtml"
     , Tuple.Create(Tuple.Create("", 484), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 484), false)
);

WriteLiteral(" />\r\n\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 522), Tuple.Create("\"", 580)
, Tuple.Create(Tuple.Create("", 528), Tuple.Create("/Scripts/Ext.grid.CheckColumn.js?", 528), true)
            
            #line 16 "..\..\Views\Shared\EditAccessSharings.cshtml"
, Tuple.Create(Tuple.Create("", 561), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 561), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 627), Tuple.Create("\"", 685)
, Tuple.Create(Tuple.Create("", 633), Tuple.Create("/Scripts/Ext.grid.CheckColumn.js?", 633), true)
            
            #line 17 "..\..\Views\Shared\EditAccessSharings.cshtml"
, Tuple.Create(Tuple.Create("", 666), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 666), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 732), Tuple.Create("\"", 813)
, Tuple.Create(Tuple.Create("", 738), Tuple.Create("/Scripts/Ext.DoubleGis.UI.Security.AccessSharingGrid.js?", 738), true)
            
            #line 18 "..\..\Views\Shared\EditAccessSharings.cshtml"
, Tuple.Create(Tuple.Create("", 794), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 794), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(@">
        body
        {
            font-family: Tahoma;
        }
        
        .controlPanel a
        {
            text-decoration: none;
            font-weight: normal;
            color: Black;
        }
        
        .controlpanel td.header
        {
            padding: 5px;
            font-size: 9px;
            font-weight: bold;
        }
        
        .controlPanel td.image
        {
            padding-left: 5px;
        }
        
        .controlPanel td.link
        {
            padding-right: 5px;
        }
    </style>
    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n\r\n        Ext.onReady(function ()\r\n        {\r\n            window.name = \"acces" +
"sSharingWindow\";\r\n            var data = Ext.decode(Ext.getDom(\'JsonData\').value" +
", true);\r\n            Ext.DoubleGis.UI.Security.AccessSharingInstance = new Ext." +
"DoubleGis.UI.Security.AccessSharing(data);\r\n\r\n            Ext.get(\"Cancel\").on(\"" +
"click\", function () { window.close(); });\r\n            Ext.get(\"OK\").on(\"click\"," +
" function ()\r\n            {\r\n                var sharingsData = Ext.DoubleGis.UI" +
".Security.AccessSharingInstance.GetData();\r\n                Ext.getDom(\'JsonData" +
"\').value = Ext.encode(sharingsData);\r\n                theForm.submit();\r\n       " +
"         return true;\r\n            });\r\n        });\r\n\r\n        function addNewCl" +
"ick()\r\n        {\r\n            var url = \'/Grid/Search/User\';\r\n            var re" +
"sult = window.showModalDialog(url, null, \'status:no; resizable:yes; dialogWidth:" +
"900px; dialogHeight:500px; resizable: yes; scroll: no; location:yes;\');\r\n       " +
"     if (result && result.items)\r\n            {\r\n                for (var i = 0;" +
" i < result.items.length; i++)\r\n                {\r\n                    var item " +
"= result.items[i];\r\n                    Ext.DoubleGis.UI.Security.AccessSharingI" +
"nstance.InsertNew(item.id, item.name);\r\n                }\r\n            }\r\n      " +
"  }\r\n\r\n        function removeSelectedClick()\r\n        {\r\n            Ext.Double" +
"Gis.UI.Security.AccessSharingInstance.RemoveSelected();\r\n        }\r\n\r\n        fu" +
"nction invertSelectedClick()\r\n        {\r\n            Ext.DoubleGis.UI.Security.A" +
"ccessSharingInstance.InvertSelectedRows();\r\n        }\r\n\r\n        function resetC" +
"lick()\r\n        {\r\n            Ext.DoubleGis.UI.Security.AccessSharingInstance.C" +
"lear();\r\n        }\r\n\r\n    </script>\r\n");

            
            #line 98 "..\..\Views\Shared\EditAccessSharings.cshtml"
    
            
            #line default
            #line hidden
            
            #line 98 "..\..\Views\Shared\EditAccessSharings.cshtml"
     using (Html.BeginForm("EditAccessSharings", Model.EntityTypeName.ToString(), FormMethod.Post, new Dictionary<string, object>{{"id", "theForm"}, {"target", "accessSharingWindow"} } ))
    {
    
            
            #line default
            #line hidden
            
            #line 100 "..\..\Views\Shared\EditAccessSharings.cshtml"
Write(Html.HiddenFor(m => m.EntityTypeName));

            
            #line default
            #line hidden
            
            #line 100 "..\..\Views\Shared\EditAccessSharings.cshtml"
                                          
    
            
            #line default
            #line hidden
            
            #line 101 "..\..\Views\Shared\EditAccessSharings.cshtml"
Write(Html.HiddenFor(m => m.EntityId));

            
            #line default
            #line hidden
            
            #line 101 "..\..\Views\Shared\EditAccessSharings.cshtml"
                                    
    
            
            #line default
            #line hidden
            
            #line 102 "..\..\Views\Shared\EditAccessSharings.cshtml"
Write(Html.HiddenFor(m => m.EntityOwnerId));

            
            #line default
            #line hidden
            
            #line 102 "..\..\Views\Shared\EditAccessSharings.cshtml"
                                         
    
            
            #line default
            #line hidden
            
            #line 103 "..\..\Views\Shared\EditAccessSharings.cshtml"
Write(Html.HiddenFor(m => m.EntityReplicationCode));

            
            #line default
            #line hidden
            
            #line 103 "..\..\Views\Shared\EditAccessSharings.cshtml"
                                                 
    
            
            #line default
            #line hidden
            
            #line 104 "..\..\Views\Shared\EditAccessSharings.cshtml"
Write(Html.HiddenFor(m => m.JsonData));

            
            #line default
            #line hidden
            
            #line 104 "..\..\Views\Shared\EditAccessSharings.cshtml"
                                    


            
            #line default
            #line hidden
WriteLiteral("    <!-- Control panel div -->\r\n");

WriteLiteral("    <div");

WriteLiteral(" style=\"width: 165px; height: 370px; float: left; table-layout: fixed; margin-lef" +
"t: 5px;\r\n        margin-right: 5px; background-color: White; border: 1px solid #" +
"6489D4\"");

WriteLiteral(">\r\n        <table");

WriteLiteral(" class=\"controlPanel\"");

WriteLiteral(" cellpadding=\"0\"");

WriteLiteral(" cellspacing=\"0\"");

WriteLiteral(">\r\n            <tr>\r\n                <td");

WriteLiteral(" class=\"header\"");

WriteLiteral(" colspan=\"2\"");

WriteLiteral(" style=\"color: White; background-color: #6693CF; height: 20px;\r\n                 " +
"   vertical-align: middle;\"");

WriteLiteral(">\r\n                    Общие задачи\r\n                </td>\r\n            </tr>\r\n  " +
"          <tr>\r\n                <td");

WriteLiteral(" style=\"width: 30px;\"");

WriteLiteral(" class=\"image\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" class=\"icon_16 icon_user\"");

WriteLiteral(" />\r\n                </td>\r\n                <td");

WriteLiteral(" style=\"width: 135px;\"");

WriteLiteral(" class=\"link\"");

WriteLiteral(">\r\n                    <a");

WriteLiteral(" href=\"#\"");

WriteLiteral(" onclick=\"addNewClick();\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 122 "..\..\Views\Shared\EditAccessSharings.cshtml"
                   Write(BLResources.AccessSharingAddUser);

            
            #line default
            #line hidden
WriteLiteral("</a>\r\n                </td>\r\n            </tr>\r\n            <tr>\r\n               " +
" <td");

WriteLiteral(" style=\"width: 30px;\"");

WriteLiteral(" class=\"image\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" class=\"icon_16 icon_delete\"");

WriteLiteral(" />\r\n                </td>\r\n                <td");

WriteLiteral(" style=\"width: 135px;\"");

WriteLiteral(" class=\"link\"");

WriteLiteral(">\r\n                    <a");

WriteLiteral(" href=\"#\"");

WriteLiteral(" onclick=\"removeSelectedClick();\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 131 "..\..\Views\Shared\EditAccessSharings.cshtml"
                   Write(BLResources.AccessSharingDeleteUser);

            
            #line default
            #line hidden
WriteLiteral("</a>\r\n                </td>\r\n            </tr>\r\n            <tr>\r\n               " +
" <td");

WriteLiteral(" style=\"width: 30px;\"");

WriteLiteral(" class=\"image\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" class=\"icon_16 icon_check\"");

WriteLiteral(" />\r\n                </td>\r\n                <td");

WriteLiteral(" style=\"width: 135px;\"");

WriteLiteral(" class=\"link\"");

WriteLiteral(">\r\n                    <a");

WriteLiteral(" href=\"#\"");

WriteLiteral(" onclick=\"invertSelectedClick();\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 140 "..\..\Views\Shared\EditAccessSharings.cshtml"
                   Write(BLResources.AccessSharingInvertSelectedRows);

            
            #line default
            #line hidden
WriteLiteral("</a>\r\n                </td>\r\n            </tr>\r\n            <tr>\r\n               " +
" <td");

WriteLiteral(" style=\"width: 30px;\"");

WriteLiteral(" class=\"image\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" class=\"icon_16 icon_clear\"");

WriteLiteral(" />\r\n                </td>\r\n                <td");

WriteLiteral(" style=\"width: 135px;\"");

WriteLiteral(" class=\"link\"");

WriteLiteral(">\r\n                    <a");

WriteLiteral(" href=\"#\"");

WriteLiteral(" onclick=\"resetClick();\"");

WriteLiteral(">\r\n");

WriteLiteral("                        ");

            
            #line 149 "..\..\Views\Shared\EditAccessSharings.cshtml"
                   Write(BLResources.AccessSharingReset);

            
            #line default
            #line hidden
WriteLiteral("</a>\r\n                </td>\r\n            </tr>\r\n        </table>\r\n    </div>\r\n");

WriteLiteral("    <!-- Grid div -->\r\n");

WriteLiteral("    <div");

WriteLiteral(" id=\"accessSharingGrid\"");

WriteLiteral(" style=\"width: 615px; float: left; margin-right: 5px;\r\n        border: 1px solid " +
"#999999\"");

WriteLiteral(">\r\n    </div>\r\n");

WriteLiteral("    <div");

WriteLiteral(" style=\"clear: both; border-bottom: 1px solid #FFFFFF; border-top: 1px solid #A7C" +
"DF0;\r\n        width: 100%; height: 0px; line-height: 0px; margin-top: 5px;\"");

WriteLiteral(">\r\n    </div>\r\n");

            
            #line 161 "..\..\Views\Shared\EditAccessSharings.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
