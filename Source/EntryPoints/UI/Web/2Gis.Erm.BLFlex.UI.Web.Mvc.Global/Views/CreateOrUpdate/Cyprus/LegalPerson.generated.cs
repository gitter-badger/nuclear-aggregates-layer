﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18449
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Views.CreateOrUpdate.Cyprus
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Cyprus/LegalPerson.cshtml")]
    public partial class LegalPerson : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Cyprus.CyprusLegalPersonViewModel>
    {
        public LegalPerson()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
  
    Layout = "../../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        window.InitPage = function() {\r\n            window.Card.on(\"beforebuil" +
"d\", function(card) {\r\n                Ext.apply(this, {\r\n                    Cha" +
"ngeLegalPersonClient: function() {\r\n                        var params = \"dialog" +
"Width:\" + 500 + \"px; dialogHeight:\" + 250 + \"px; status:yes; scroll:no;resizable" +
":no;\";\r\n                        var url = \'/GroupOperation/ChangeClient/LegalPer" +
"son\';\r\n                        window.showModalDialog(url, [Ext.getDom(\"Id\").val" +
"ue], params);\r\n                        this.refresh();\r\n                    },\r\n" +
"\r\n                    ChangeLegalPersonRequisites: function() {\r\n               " +
"         var params = \"dialogWidth:\" + 700 + \"px; dialogHeight:\" + 330 + \"px; st" +
"atus:yes; scroll:no;resizable:no;\";\r\n                        var url = \'/Cyprus/" +
"LegalPerson/ChangeLegalPersonRequisites/\' + Ext.getDom(\"Id\").value;\r\n           " +
"             var result = window.showModalDialog(url, null, params);\r\n          " +
"              if (result == \'OK\') {\r\n                            this.refresh();" +
"\r\n                        }\r\n                    }\r\n                });\r\n\r\n     " +
"       });\r\n            window.Card.on(\"afterbuild\", function(card) {\r\n         " +
"       if (window.Ext.getDom(\"ViewConfig_Id\").value && window.Ext.getDom(\"ViewCo" +
"nfig_Id\").value != \"0\") {\r\n                    this.Items.TabPanel.add(\r\n       " +
"                 {\r\n                            xtype: \"actionshistorytab\",\r\n   " +
"                         pCardInfo:\r\n                            {\r\n            " +
"                    pTypeName: this.Settings.EntityName,\r\n                      " +
"          pId: window.Ext.getDom(\"ViewConfig_Id\").value\r\n                       " +
"     }\r\n                        });\r\n                }\r\n            });\r\n\r\n     " +
"       Ext.apply(this, {\r\n\r\n                buildProfilesList: function () {\r\n  " +
"                  if (this.form.Id.value != 0) {\r\n                        var cn" +
"t = Ext.getCmp(\'ContentTab_holder\');\r\n                        var tp = Ext.getCm" +
"p(\'TabWrapper\');\r\n\r\n                        tp.anchor = \"100%, 60%\";\r\n          " +
"              delete tp.anchorSpec;\r\n                        cnt.add(new Ext.Pan" +
"el({\r\n                            id: \'profileFrame_holder\',\r\n                  " +
"          anchor: \'100%, 40%\',\r\n                            html: \'<iframe id=\"p" +
"rofileFrame_frame\"></iframe>\'\r\n                        }));\r\n                   " +
"     cnt.doLayout();\r\n                        var mask = new window.Ext.LoadMask" +
"(window.Ext.get(\"profileFrame_holder\"));\r\n                        mask.show();\r\n" +
"                        var iframe = Ext.get(\'profileFrame_frame\');\r\n\r\n         " +
"               iframe.dom.src = \'/Grid/View/LegalPersonProfile/LegalPerson/{0}/{" +
"1}?extendedInfo=filterToParent%3Dtrue\'.replace(/\\{0\\}/g, this.form.Id.value).rep" +
"lace(/\\{1\\}/g, this.ReadOnly ? \'Inactive\' : \'Active\');\r\n                        " +
"iframe.on(\'load\', function (evt, el) {\r\n                            el.height = " +
"Ext.get(el.parentElement).getComputedHeight();\r\n                            el.w" +
"idth = Ext.get(el.parentElement).getComputedWidth();\r\n                          " +
"  el.style.height = \"100%\";\r\n                            el.style.width = \"100%\"" +
";\r\n                            el.contentWindow.Ext.onReady(function () {\r\n     " +
"                           el.contentWindow.IsBottomOrderPositionDataList = true" +
";\r\n                            });\r\n                            this.hide();\r\n  " +
"                      }, mask);\r\n                        cnt.doLayout();\r\n\r\n    " +
"                }\r\n                }\r\n            });\r\n\r\n            this.on(\"af" +
"terbuild\", this.buildProfilesList, this);\r\n            this.on(\"afterrelatedlist" +
"ready\", function(card, details) {\r\n                var dataListName = details.da" +
"taList.currentSettings.Name;\r\n\r\n                if (dataListName === \'LegalPerso" +
"nProfile\') {\r\n                    var dataListWindow = details.dataList.ContentC" +
"ontainer.container.dom.document.parentWindow;\r\n                    if (dataListW" +
"indow.IsBottomOrderPositionDataList) {\r\n                        dataListWindow.E" +
"xt.getDom(\'Toolbar\').style.display = \'none\';\r\n                        details.da" +
"taList.Items.Grid.getBottomToolbar().hide();\r\n                        details.da" +
"taList.ContentContainer.doLayout();\r\n                    }\r\n                }\r\n " +
"           }, this);\r\n        };\r\n    </script>\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(">\r\n        div.label-wrapper {\r\n            width: 150px !important;\r\n        }\r\n" +
"    </style>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 105 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 106 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
Write(Html.HiddenFor(m => m.HasProfiles));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 4833), Tuple.Create("\"", 4869)
            
            #line 108 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
, Tuple.Create(Tuple.Create("", 4841), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 4841), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 110 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.LegalName, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 111 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.LegalPersonType, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 114 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.Client, FieldFlex.lone, new LookupSettings { EntityName = EntityName.Client, ReadOnly = !Model.IsNew }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 117 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.Inn, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 118 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.BusinessmanInn, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 119 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.VAT, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 122 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.PassportNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 123 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.PassportIssuedBy, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 126 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.CardNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 127 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.RegistrationAddress, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 130 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.LegalAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 133 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.Comment, FieldFlex.lone, new Dictionary<string, object> { { "rows", 3 } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
