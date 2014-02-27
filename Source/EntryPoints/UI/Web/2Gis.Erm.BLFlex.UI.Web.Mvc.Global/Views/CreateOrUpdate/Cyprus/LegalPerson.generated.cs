﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Views.CreateOrUpdate.Cyprus
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc.Html;
#line 1 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
    using BLCore.UI.Web.Mvc.Utils;
    
    #line default
    #line hidden
    using DoubleGis.Erm.BLCore.Resources.Server.Properties;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Cyprus/LegalPerson.cshtml")]
    public partial class LegalPerson : System.Web.Mvc.WebViewPage<Models.Cyprus.CyprusLegalPersonViewModel>
    {
        public LegalPerson()
        {
        }
        public override void Execute()
        {
            
            #line 4 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
  
    Layout = "../../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">
        window.InitPage = function() {
            window.Card.on(""beforebuild"", function(card) {
                Ext.apply(this, {
                    ChangeLegalPersonClient: function() {
                        var params = ""dialogWidth:"" + 500 + ""px; dialogHeight:"" + 250 + ""px; status:yes; scroll:no;resizable:no;"";
                        var url = '/GroupOperation/ChangeClient/LegalPerson';
                        window.showModalDialog(url, [Ext.getDom(""Id"").value], params);
                        this.refresh();
                    },

                    ChangeLegalPersonRequisites: function() {
                        var params = ""dialogWidth:"" + 700 + ""px; dialogHeight:"" + 330 + ""px; status:yes; scroll:no;resizable:no;"";
                        var url = '/Cyprus/LegalPerson/ChangeLegalPersonRequisites/' + Ext.getDom(""Id"").value;
                        var result = window.showModalDialog(url, null, params);
                        if (result == 'OK') {
                            var isInSyncWith1C = window.Ext.getDom(""IsInSyncWith1C"").value;
                            // TODO {all, 18.12.2013}: возможно некоректное отображение диакритики
                            // TODO {all, 18.12.2013}: ресурс можно перенести в ClientResourceStorage
                            var msg = isInSyncWith1C.toLowerCase() == 'true' ? '");

            
            #line 29 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
                                                                           Write(BLResources.OnLegalPersonRequisitesChangedMessageSync1C);

            
            #line default
            #line hidden
WriteLiteral("\' : \'");

            
            #line 29 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
                                                                                                                                        Write(BLResources.OnLegalPersonRequisitesChangedMessageNotSync1C);

            
            #line default
            #line hidden
WriteLiteral("\';\r\n                            window.Ext.MessageBox.show({\r\n                   " +
"             title: \'\',\r\n                                msg: msg,\r\n            " +
"                    buttons: window.Ext.MessageBox.OK,\r\n                        " +
"        fn: function() { this.refresh(true); },\r\n                               " +
" scope: this,\r\n                                width: 500,\r\n                    " +
"            icon: window.Ext.MessageBox.INFO\r\n                            });\r\n " +
"                       }\r\n                    }\r\n                });\r\n\r\n        " +
"    });\r\n            window.Card.on(\"afterbuild\", function(card) {\r\n            " +
"    if (window.Ext.getDom(\"ViewConfig_Id\").value && window.Ext.getDom(\"ViewConfi" +
"g_Id\").value != \"0\") {\r\n                    this.Items.TabPanel.add(\r\n          " +
"              {\r\n                            xtype: \"actionshistorytab\",\r\n      " +
"                      pCardInfo:\r\n                            {\r\n               " +
"                 pTypeName: this.Settings.EntityName,\r\n                         " +
"       pId: window.Ext.getDom(\"ViewConfig_Id\").value\r\n                          " +
"  }\r\n                        });\r\n                }\r\n            });\r\n\r\n        " +
"    Ext.apply(this, {\r\n\r\n                buildProfilesList: function () {\r\n     " +
"               if (this.form.Id.value != 0) {\r\n                        var cnt =" +
" Ext.getCmp(\'ContentTab_holder\');\r\n                        var tp = Ext.getCmp(\'" +
"TabWrapper\');\r\n\r\n                        tp.anchor = \"100%, 60%\";\r\n             " +
"           delete tp.anchorSpec;\r\n                        cnt.add(new Ext.Panel(" +
"{\r\n                            id: \'profileFrame_holder\',\r\n                     " +
"       anchor: \'100%, 40%\',\r\n                            html: \'<iframe id=\"prof" +
"ileFrame_frame\"></iframe>\'\r\n                        }));\r\n                      " +
"  cnt.doLayout();\r\n                        var mask = new window.Ext.LoadMask(wi" +
"ndow.Ext.get(\"profileFrame_holder\"));\r\n                        mask.show();\r\n   " +
"                     var iframe = Ext.get(\'profileFrame_frame\');\r\n\r\n            " +
"            iframe.dom.src = \'/Grid/View/LegalPersonProfile/LegalPerson/{0}/{1}?" +
"extendedInfo=filterToParent%3Dtrue\'.replace(/\\{0\\}/g, this.form.Id.value).replac" +
"e(/\\{1\\}/g, this.ReadOnly ? \'Inactive\' : \'Active\');\r\n                        ifr" +
"ame.on(\'load\', function (evt, el) {\r\n                            el.height = Ext" +
".get(el.parentElement).getComputedHeight();\r\n                            el.widt" +
"h = Ext.get(el.parentElement).getComputedWidth();\r\n                            e" +
"l.style.height = \"100%\";\r\n                            el.style.width = \"100%\";\r\n" +
"                            el.contentWindow.Ext.onReady(function () {\r\n        " +
"                        el.contentWindow.IsBottomOrderPositionDataList = true;\r\n" +
"                            });\r\n                            this.hide();\r\n     " +
"                   }, mask);\r\n                        cnt.doLayout();\r\n\r\n       " +
"             }\r\n                }\r\n            });\r\n\r\n            this.on(\"after" +
"build\", this.buildProfilesList, this);\r\n            this.on(\"afterrelatedlistrea" +
"dy\", function(card, details) {\r\n                var dataListName = details.dataL" +
"ist.currentSettings.Name;\r\n\r\n                if (dataListName === \'LegalPersonPr" +
"ofile\') {\r\n                    var dataListWindow = details.dataList.ContentCont" +
"ainer.container.dom.document.parentWindow;\r\n                    if (dataListWind" +
"ow.IsBottomOrderPositionDataList) {\r\n                        dataListWindow.Ext." +
"getDom(\'Toolbar\').style.display = \'none\';\r\n                        details.dataL" +
"ist.Items.Grid.getBottomToolbar().hide();\r\n                        details.dataL" +
"ist.ContentContainer.doLayout();\r\n                    }\r\n                }\r\n    " +
"        }, this);\r\n        };\r\n    </script>\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(">\r\n        div.label-wrapper {\r\n            width: 150px !important;\r\n        }\r\n" +
"    </style>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 118 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 119 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
Write(Html.HiddenFor(m => m.ReplicationCode));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 120 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
Write(Html.HiddenFor(m => m.IsInSyncWith1C));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 121 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
Write(Html.HiddenFor(m => m.HasProfiles));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 5846), Tuple.Create("\"", 5882)
            
            #line 123 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
, Tuple.Create(Tuple.Create("", 5854), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 5854), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 125 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.LegalName, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 126 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.LegalPersonType, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 129 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.Client, FieldFlex.lone, new LookupSettings { EntityName = EntityName.Client, ReadOnly = !Model.IsNew }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 132 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.Inn, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 133 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.BusinessmanInn, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 134 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.VAT, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 137 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.PassportNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 138 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.PassportIssuedBy, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 141 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.CardNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 142 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.RegistrationAddress, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 145 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.LegalAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 148 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.Comment, FieldFlex.lone, new Dictionary<string, object> { { "rows", 3 } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
