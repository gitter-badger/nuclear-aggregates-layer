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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Price.cshtml")]
    public partial class Price : System.Web.Mvc.WebViewPage<PriceViewModel>
    {
        public Price()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\CreateOrUpdate\Price.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">
        window.InitPage = function ()
        {
            window.Card.on(""beforebuild"", function (cardObject)
            {
                this.Publish = function ()
                {
                    if (Ext.getDom(""IsPublished"").checked)
                    {
                        // TODO {all, 18.12.2013}: возможно некоректное отображение диакритики
                        // TODO {all, 18.12.2013}: alert можно заменить на ext'овый messagebox
                        // TODO {all, 18.12.2013}: ресурс можно перенести в ClientResourceStorage
                        alert('");

            
            #line 21 "..\..\Views\CreateOrUpdate\Price.cshtml"
                          Write(BLResources.PriceIsAlreadyPublished);

            
            #line default
            #line hidden
WriteLiteral(@"');
                        return;
                    }
                    var params = ""dialogWidth:"" + 500 + ""px; dialogHeight:"" + 150 + ""px; status:yes; scroll:no;resizable:no;"";
                    var url = '/Price/Publish';
                    var arguments = {
                        priceId: Ext.getDom(""Id"").value,
                        organizationUnitId: Ext.getDom(""OrganizationUnitId"").value,
                        beginDate: Ext.util.Format.reformatDateFromUserLocaleToInvariant(Ext.getDom(""BeginDate"").value),
                        publishDate: Ext.util.Format.reformatDateFromUserLocaleToInvariant(Ext.getDom(""PublishDate"").value)
                    };

                    this.Items.Toolbar.disable();
                    window.showModalDialog(url, arguments, params);
                    this.refresh(true);
                };

                this.Unpublish = function ()
                {
                    if (!Ext.getDom(""IsPublished"").checked)
                    {
                        alert('");

            
            #line 42 "..\..\Views\CreateOrUpdate\Price.cshtml"
                          Write(BLResources.CantUnpublishPriceWhenUnpublished);

            
            #line default
            #line hidden
WriteLiteral("\');\r\n                        return;\r\n                    }\r\n                    " +
"var params = \"dialogWidth:\" + 500 + \"px; dialogHeight:\" + 150 + \"px; status:yes;" +
" scroll:no;resizable:no;\";\r\n                    var url = \'/Price/Unpublish\';\r\n " +
"                   var arguments = {\r\n                        priceId: Ext.getDo" +
"m(\"Id\").value\r\n                    };\r\n\r\n                    this.Items.Toolbar." +
"disable();\r\n                    window.showModalDialog(url, arguments, params);\r" +
"\n                    window.Card.isDirty = false;\r\n                    this.refr" +
"esh(true);\r\n                };\r\n\r\n                this.Copy = function ()\r\n     " +
"           {\r\n                    var params = \"dialogWidth:\" + 500 + \"px; dialo" +
"gHeight:\" + 250 + \"px; status:yes; scroll:no;resizable:no;\";\r\n                  " +
"  var url = \'/Price/Copy\';\r\n                    var arguments = {\r\n             " +
"           priceId: Ext.getDom(\"Id\").value\r\n                    };\r\n\r\n          " +
"          var disabledItems = [];\r\n                    this.Items.Toolbar.items." +
"items.forEach(function (x) { if (x.disabled) { disabledItems.push(x); } });\r\n\r\n " +
"                   this.Items.Toolbar.disable();\r\n                    var nextAc" +
"tion = window.showModalDialog(url, arguments, params);\r\n                    this" +
".Items.Toolbar.enable();\r\n\r\n                    // Включились все кнопки, поэтом" +
"у деактивируем ранее деактивированные.\r\n                    disabledItems.forEac" +
"h(function (x) { x.disable(); });\r\n\r\n                    if (nextAction == \"Clos" +
"e\")\r\n                    {\r\n                        window.opener.Entity.refresh" +
"();\r\n                        window.close();\r\n                    }\r\n           " +
"         if (nextAction == \"Reload\")\r\n                    { this.refresh(true); " +
"}\r\n                };\r\n            });\r\n        };\r\n    </script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 3766), Tuple.Create("\"", 3802)
            
            #line 90 "..\..\Views\CreateOrUpdate\Price.cshtml"
, Tuple.Create(Tuple.Create("", 3774), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 3774), false)
);

WriteLiteral(">\r\n");

            
            #line 91 "..\..\Views\CreateOrUpdate\Price.cshtml"
        
            
            #line default
            #line hidden
            
            #line 91 "..\..\Views\CreateOrUpdate\Price.cshtml"
         if (Model != null)
        {
            
            
            #line default
            #line hidden
            
            #line 93 "..\..\Views\CreateOrUpdate\Price.cshtml"
       Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
            
            #line 93 "..\..\Views\CreateOrUpdate\Price.cshtml"
                                      
            
            
            #line default
            #line hidden
            
            #line 94 "..\..\Views\CreateOrUpdate\Price.cshtml"
       Write(Html.HiddenFor(m => m.CurrencyId));

            
            #line default
            #line hidden
            
            #line 94 "..\..\Views\CreateOrUpdate\Price.cshtml"
                                              
            
            
            #line default
            #line hidden
            
            #line 95 "..\..\Views\CreateOrUpdate\Price.cshtml"
       Write(Html.HiddenFor(m => m.Name));

            
            #line default
            #line hidden
            
            #line 95 "..\..\Views\CreateOrUpdate\Price.cshtml"
                                        
        }

            
            #line default
            #line hidden
WriteLiteral("        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 98 "..\..\Views\CreateOrUpdate\Price.cshtml"
       Write(Html.TemplateField(m => m.CreateDate, FieldFlex.twins, new DateTimeSettings{ReadOnly = true}));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 99 "..\..\Views\CreateOrUpdate\Price.cshtml"
       Write(Html.TemplateField(m => m.OrganizationUnit, FieldFlex.twins, new LookupSettings { EntityName = EntityName.OrganizationUnit, SearchFormFilterInfo = "IsDeleted=false&&IsActive=true"}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 102 "..\..\Views\CreateOrUpdate\Price.cshtml"
       Write(Html.TemplateField(m => m.PublishDate, FieldFlex.twins, new DateTimeSettings { MinDate = DateTime.Now, ShiftOffset = false}));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 103 "..\..\Views\CreateOrUpdate\Price.cshtml"
       Write(Html.TemplateField(m => m.IsPublished, FieldFlex.twins, new Dictionary<string, object> { { "disabled", "disabled" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 106 "..\..\Views\CreateOrUpdate\Price.cshtml"
       Write(Html.TemplateField(m => m.BeginDate, FieldFlex.twins, new DateTimeSettings{MinDate = DateTime.Now.AddDays(1), ShiftOffset = false, PeriodType = PeriodType.MonthlyLowerBound}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
