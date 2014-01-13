﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34003
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Views.CreateOrUpdate
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Client.cshtml")]
    public partial class Client : System.Web.Mvc.WebViewPage<ClientViewModel>
    {
        public Client()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n\r\n");

            
            #line 4 "..\..\Views\CreateOrUpdate\Client.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        window.InitPage = function () {\r\n            window.Card.on(\"beforebui" +
"ld\", function () {\r\n                if (Ext.getDom(\"Id\").value != 0) {\r\n        " +
"            // change owner\r\n                    this.ChangeOwner = function () " +
"{\r\n                        var params = \"dialogWidth:450px; dialogHeight:300px; " +
"status:yes; scroll:no;resizable:no;\";\r\n                        var sUrl = \"/Grou" +
"pOperation/Assign/Client\";\r\n                        var result = window.showModa" +
"lDialog(sUrl, [Ext.getDom(\"Id\").value], params);\r\n                        if (re" +
"sult === true) {\r\n                            this.refresh(true);\r\n             " +
"           }\r\n                    };\r\n\r\n                    // change territory\r" +
"\n                    this.ChangeTerritory = function () {\r\n                     " +
"   var params = \"dialogWidth:450px; dialogHeight:230px; status:yes; scroll:no; r" +
"esizable:no; \";\r\n                        var sUrl = \"/GroupOperation/ChangeTerri" +
"tory/Client\";\r\n                        var result = window.showModalDialog(sUrl," +
" [Ext.getDom(\"Id\").value], params);\r\n                        if (result === true" +
") {\r\n                            this.refresh(true);\r\n                        }\r" +
"\n                    };\r\n\r\n                    // qualify\r\n                    t" +
"his.Qualify = function () {\r\n                        var params = \"dialogWidth:6" +
"50px; dialogHeight:300px; status:yes; scroll:no; resizable:no; \";\r\n             " +
"           var sUrl = \"/GroupOperation/Qualify/Client/\";\r\n                      " +
"  var result = window.showModalDialog(sUrl, [Ext.getDom(\"Id\").value], params);\r\n" +
"                        if (result === true) {\r\n                            this" +
".refresh(true);\r\n                        }\r\n                    };\r\n\r\n          " +
"          // disqualify\r\n                    this.Disqualify = function () {\r\n  " +
"                      var params = \"dialogWidth:650px; dialogHeight:230px; statu" +
"s:yes; scroll:no; resizable:no; \";\r\n                        var sUrl = \"/GroupOp" +
"eration/Disqualify/Client\";\r\n                        var result = window.showMod" +
"alDialog(sUrl, [Ext.getDom(\"Id\").value], params);\r\n                        if (r" +
"esult === true) {\r\n                            this.refresh(true);\r\n            " +
"            }\r\n                    };\r\n                    this.Merge = function" +
" () {\r\n                        var params = \"dialogWidth:\" + 800 + \"px; dialogHe" +
"ight:\" + 600 + \"px; status:yes; scroll:yes;resizable:yes;\";\r\n                   " +
"     var url = \'/Client/Merge?masterId={0}\';\r\n                        window.sho" +
"wModalDialog(String.format(url, Ext.getDom(\"Id\").value), null, params);\r\n       " +
"                 this.refresh(true);\r\n                    };\r\n                }\r" +
"\n            });\r\n\r\n            window.Card.on(\"afterbuild\", function (card) {\r\n" +
"                if (window.Ext.getDom(\"ViewConfig_Id\").value && window.Ext.getDo" +
"m(\"ViewConfig_Id\").value != \"0\") {\r\n                    this.Items.TabPanel.add(" +
"\r\n                        {\r\n                            xtype: \"actionshistoryt" +
"ab\",\r\n                            pCardInfo:\r\n                            {\r\n   " +
"                             pTypeName: this.Settings.EntityName,\r\n             " +
"                   pId: window.Ext.getDom(\"ViewConfig_Id\").value\r\n              " +
"              }\r\n                        });\r\n                }\r\n            });" +
"\r\n        };\r\n    </script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 81 "..\..\Views\CreateOrUpdate\Client.cshtml"
Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 82 "..\..\Views\CreateOrUpdate\Client.cshtml"
Write(Html.HiddenFor(m => m.ReplicationCode));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 83 "..\..\Views\CreateOrUpdate\Client.cshtml"
Write(Html.HiddenFor(m => m.CanEditIsAdvertisingAgency));

            
            #line default
            #line hidden
WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 3833), Tuple.Create("\"", 3869)
            
            #line 84 "..\..\Views\CreateOrUpdate\Client.cshtml"
, Tuple.Create(Tuple.Create("", 3841), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 3841), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 86 "..\..\Views\CreateOrUpdate\Client.cshtml"
       Write(Html.TemplateField(m => m.Name, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 89 "..\..\Views\CreateOrUpdate\Client.cshtml"
       Write(Html.TemplateField(m => m.MainAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 92 "..\..\Views\CreateOrUpdate\Client.cshtml"
       Write(Html.TemplateField(m => m.MainPhoneNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 93 "..\..\Views\CreateOrUpdate\Client.cshtml"
       Write(Html.TemplateField(m => m.Fax, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 96 "..\..\Views\CreateOrUpdate\Client.cshtml"
       Write(Html.TemplateField(m => m.AdditionalPhoneNumber1, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 97 "..\..\Views\CreateOrUpdate\Client.cshtml"
       Write(Html.TemplateField(m => m.Email, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 100 "..\..\Views\CreateOrUpdate\Client.cshtml"
       Write(Html.TemplateField(m => m.AdditionalPhoneNumber2, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 101 "..\..\Views\CreateOrUpdate\Client.cshtml"
       Write(Html.TemplateField(m => m.Website, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 104 "..\..\Views\CreateOrUpdate\Client.cshtml"
       Write(Html.TemplateField(m => m.IsAdvertisingAgency, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 107 "..\..\Views\CreateOrUpdate\Client.cshtml"
       Write(Html.TemplateField(m => m.Comment, FieldFlex.lone, new Dictionary<string, object> { { "rows", 5 } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n");

WriteLiteral("        ");

            
            #line 109 "..\..\Views\CreateOrUpdate\Client.cshtml"
   Write(Html.SectionHead("SectionHead2", BLResources.AdditionalTabTitle));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 111 "..\..\Views\CreateOrUpdate\Client.cshtml"
       Write(Html.TemplateField(m => m.InformationSource, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 112 "..\..\Views\CreateOrUpdate\Client.cshtml"
       Write(Html.TemplateField(m => m.LastQualifyTime, FieldFlex.twins, new DateTimeSettings { ReadOnly = true, ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 115 "..\..\Views\CreateOrUpdate\Client.cshtml"
       Write(Html.TemplateField(m => m.PromisingScore, FieldFlex.twins, new Dictionary<string, object> { { "readonly", "readonly" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 116 "..\..\Views\CreateOrUpdate\Client.cshtml"
       Write(Html.TemplateField(m => m.LastDisqualifyTime, FieldFlex.twins, new DateTimeSettings { ReadOnly = true, ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 119 "..\..\Views\CreateOrUpdate\Client.cshtml"
       Write(Html.TemplateField(m => m.MainFirm, FieldFlex.lone, new LookupSettings { EntityName = EntityName.Firm, SearchFormFilterInfo = "ClientId={Id}" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"AdministrationTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 5949), Tuple.Create("\"", 5992)
            
            #line 122 "..\..\Views\CreateOrUpdate\Client.cshtml"
, Tuple.Create(Tuple.Create("", 5957), Tuple.Create<System.Object, System.Int32>(BLResources.AdministrationTabTitle
            
            #line default
            #line hidden
, 5957), false)
);

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 123 "..\..\Views\CreateOrUpdate\Client.cshtml"
   Write(Html.SectionHead("adminHeader", BLResources.AdministrationTabTitle));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 124 "..\..\Views\CreateOrUpdate\Client.cshtml"
        
            
            #line default
            #line hidden
            
            #line 124 "..\..\Views\CreateOrUpdate\Client.cshtml"
         if (Model.IsCurated && Model.IsSecurityRoot)
        {

            
            #line default
            #line hidden
WriteLiteral("            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 127 "..\..\Views\CreateOrUpdate\Client.cshtml"
           Write(Html.TemplateField(m => m.Owner, FieldFlex.twins, new LookupSettings { EntityName = EntityName.User, Plugins = new[] { "new Ext.ux.LookupFieldOwner()" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                ");

            
            #line 128 "..\..\Views\CreateOrUpdate\Client.cshtml"
           Write(Html.TemplateField(m => m.Territory, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Territory, ReadOnly = Model != null && Model.Territory != null && !string.IsNullOrEmpty(Model.Territory.Value) }));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n");

            
            #line 130 "..\..\Views\CreateOrUpdate\Client.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 132 "..\..\Views\CreateOrUpdate\Client.cshtml"
       Write(Html.TemplateField(m => m.CreatedBy, FieldFlex.twins, new LookupSettings { EntityName = EntityName.User, ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 133 "..\..\Views\CreateOrUpdate\Client.cshtml"
       Write(Html.TemplateField(m => m.CreatedOn, FieldFlex.twins, new DateTimeSettings { ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 136 "..\..\Views\CreateOrUpdate\Client.cshtml"
       Write(Html.TemplateField(m => m.ModifiedBy, FieldFlex.twins, new LookupSettings { EntityName = EntityName.User, ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 137 "..\..\Views\CreateOrUpdate\Client.cshtml"
       Write(Html.TemplateField(m => m.ModifiedOn, FieldFlex.twins, new DateTimeSettings { ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
