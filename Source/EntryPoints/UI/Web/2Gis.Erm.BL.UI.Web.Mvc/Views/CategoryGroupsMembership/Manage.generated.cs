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

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Views.CategoryGroupsMembership
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
    using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CategoryGroupsMembership/Manage.cshtml")]
    public partial class Manage : System.Web.Mvc.WebViewPage<CategoryGroupMembershipViewModel>
    {
        public Manage()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\CategoryGroupsMembership\Manage.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 134), Tuple.Create("\"", 235)
, Tuple.Create(Tuple.Create("", 140), Tuple.Create("/Scripts/Ext.DoubleGis.UI.CategoryGroupsMembershipControl.js?", 140), true)
            
            #line 9 "..\..\Views\CategoryGroupsMembership\Manage.cshtml"
, Tuple.Create(Tuple.Create("", 201), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 201), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        window.InitPage = function() {\r\n\r\n            Ext.apply(this, {\r\n     " +
"           ViewCategoryGroups: function () {\r\n\r\n                    var params =" +
" \"dialogWidth:\" + 800 + \"px; dialogHeight:\" + 600 + \"px; status:yes; scroll:yes;" +
"resizable:yes;\";\r\n                    var url = \'/Grid/View/CategoryGroup\';\r\n\r\n " +
"                   window.showModalDialog(url, null, params);\r\n                 " +
"   this.refresh();\r\n                },\r\n                CreateControl: function(" +
") {\r\n                    var id = Ext.get(\"OrganizationUnitId\").dom.value;\r\n\r\n  " +
"                  var config =\r\n                    {\r\n                        d" +
"iv: \'CategoryGroupsForOrganizationUnitDiv\',\r\n\r\n                        api:\r\n   " +
"                     {\r\n                            read: { url: \'/CategoryGroup" +
"sMembership/GetCategoryGroupsMembership\', method: \'GET\' },\r\n                    " +
"        update: { url: \'/CategoryGroupsMembership/SetCategoryGroupsMembership\', " +
"method: \'POST\' }\r\n                        },\r\n                        baseParams" +
": { organizationUnitId: id }\r\n                    };\r\n\r\n                    Ext." +
"DoubleGis.UI.CategoryGroupsMembershipControlInstance = new Ext.DoubleGis.UI.Cate" +
"goryGroupsMembershipControl(config);\r\n                }\r\n            });\r\n\r\n    " +
"        window.Card.on(\"beforebuild\", function() {\r\n                this.generic" +
"Save = function(submitMode) {\r\n                    var card = this;\r\n\r\n         " +
"           var onSuccess = function() {\r\n                        card.submitMode" +
" = submitMode;\r\n                        if (card.normalizeForm() !== false) {\r\n " +
"                           card.postForm();\r\n                        }\r\n        " +
"            };\r\n\r\n                    var onFailure = function () {\r\n           " +
"             // TODO {all, 18.12.2013}: alert можно заменить на ext\'овый message" +
"box\r\n                        alert(Ext.LocalizedResources.SaveError);\r\n         " +
"               card.Items.Toolbar.enable();\r\n                    };\r\n\r\n         " +
"           if (Ext.DoubleGis.UI.CategoryGroupsMembershipControlInstance) {\r\n    " +
"                    Ext.DoubleGis.UI.CategoryGroupsMembershipControlInstance.Sav" +
"e(onSuccess, onFailure);\r\n                    } else {\r\n                        " +
"onSuccess();\r\n                    }\r\n                };\r\n            });\r\n\r\n    " +
"        window.Card.on(\'beforepost\', function(card) {\r\n                card.gene" +
"ricSave(card.submitMode);\r\n                return false;\r\n            });\r\n\r\n   " +
"         window.Card.on(\'afterbuild\', function (card) {\r\n                card.Cr" +
"eateControl();\r\n            });\r\n        }\r\n    </script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 2993), Tuple.Create("\"", 3029)
            
            #line 82 "..\..\Views\CategoryGroupsMembership\Manage.cshtml"
, Tuple.Create(Tuple.Create("", 3001), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 3001), false)
);

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 83 "..\..\Views\CategoryGroupsMembership\Manage.cshtml"
   Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 84 "..\..\Views\CategoryGroupsMembership\Manage.cshtml"
   Write(Html.HiddenFor(m => m.OrganizationUnitId));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" id=\"CategoryGroupsForOrganizationUnitDiv\"");

WriteLiteral("></div>\r\n    </div>\r\n    ");

});

        }
    }
}
#pragma warning restore 1591
