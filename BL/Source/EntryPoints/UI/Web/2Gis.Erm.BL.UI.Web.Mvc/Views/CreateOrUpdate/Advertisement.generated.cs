﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ASP
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
    using NuClear.Model.Common.Entities;
    using NuClear.Model.Common.Operations.Identity;
    using NuClear.Model.Common.Operations.Identity.Generic;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Advertisement.cshtml")]
    public partial class _Views_CreateOrUpdate_Advertisement_cshtml : System.Web.Mvc.WebViewPage<AdvertisementViewModel>
    {
        public _Views_CreateOrUpdate_Advertisement_cshtml()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\CreateOrUpdate\Advertisement.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n<script");

WriteAttribute("src", Tuple.Create(" src=\"", 120), Tuple.Create("\"", 186)
, Tuple.Create(Tuple.Create("", 126), Tuple.Create("/Scripts/Ext.ux.AdvertisementBagPanel.js?", 126), true)
            
            #line 9 "..\..\Views\CreateOrUpdate\Advertisement.cshtml"
, Tuple.Create(Tuple.Create("", 167), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 167), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n<style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(@">
    div.x-ads-richtext strong, div.x-ads-richtext b
    {
        font-weight: bold;
    }
    div.x-ads-richtext ol
    {
        list-style-type: decimal;
    }
    div.x-ads-richtext ul
    {
        list-style-type: disc;
    }
    div.x-ads-richtext em, div.x-ads-richtext i
    {
        font-style: italic;
    }
</style>
<script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        window.InitPage = function() {\r\n        Ext.apply(this,\r\n            {" +
"\r\n                    showAdsElemBag: function() {\r\n                        if (" +
"window.Ext.getDom(\"Id\").value != \'0\') {\r\n                            if (!this.a" +
"dsElemBag) {\r\n                            var cnt = Ext.getCmp(\'ContentTab_holde" +
"r\');\r\n                            var tp = Ext.getCmp(\'TabWrapper\');\r\n\r\n        " +
"                    tp.anchor = \"100%, 40%\";\r\n                            delete" +
" tp.anchorSpec;\r\n\r\n                            window.Entity = this.adsElemBag =" +
" cnt.add(new Ext.ux.AdvertisementBagPanel({\r\n                                aut" +
"oScroll: true,\r\n                                anchor: \'100%, 60%\',\r\n          " +
"                      title: Ext.LocalizedResources.AdvertisementElements\r\n     " +
"                       }));\r\n                                window.Entity.store" +
".load({ params: { id: Ext.getDom(\"Id\").value } });\r\n                            " +
"cnt.doLayout();\r\n                            } else {\r\n                         " +
"       window.Entity.store.load({ params: { id: Ext.getDom(\"Id\").value } });\r\n  " +
"                      }\r\n                        }\r\n                },\r\n        " +
"            Preview: function() {\r\n                    window.open(\"/Advertiseme" +
"nt/Preview?advertisementId=\" + window.Ext.getDom(\"Id\").value, \"\", \"width=360, he" +
"ight=600, resizable=1\");\r\n                }\r\n                });\r\n            th" +
"is.on(\"afterbuild\", function() {\r\n                this.showAdsElemBag();\r\n      " +
"         }, this);\r\n        this.on(\"formbind\", this.showAdsElemBag, this);\r\n   " +
" };\r\n</script>    \r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n\r\n");

            
            #line 69 "..\..\Views\CreateOrUpdate\Advertisement.cshtml"
 if (Model != null)
{
    
            
            #line default
            #line hidden
            
            #line 71 "..\..\Views\CreateOrUpdate\Advertisement.cshtml"
Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
            
            #line 71 "..\..\Views\CreateOrUpdate\Advertisement.cshtml"
                              
    
            
            #line default
            #line hidden
            
            #line 72 "..\..\Views\CreateOrUpdate\Advertisement.cshtml"
Write(Html.HiddenFor(m => m.IsReadOnlyTemplate));

            
            #line default
            #line hidden
            
            #line 72 "..\..\Views\CreateOrUpdate\Advertisement.cshtml"
                                              
}

            
            #line default
            #line hidden
            
            #line 74 "..\..\Views\CreateOrUpdate\Advertisement.cshtml"
Write(Html.HiddenFor(m=>m.IsDummy));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n<div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 2444), Tuple.Create("\"", 2480)
            
            #line 76 "..\..\Views\CreateOrUpdate\Advertisement.cshtml"
, Tuple.Create(Tuple.Create("", 2452), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 2452), false)
);

WriteLiteral(">\r\n    <div>\r\n        <div");

WriteLiteral(" style=\"display: none; height: 15px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(" class=\"Notifications\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 79 "..\..\Views\CreateOrUpdate\Advertisement.cshtml"
       Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 82 "..\..\Views\CreateOrUpdate\Advertisement.cshtml"
       Write(Html.TemplateField(m => m.Name, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 85 "..\..\Views\CreateOrUpdate\Advertisement.cshtml"
       Write(Html.TemplateField(m => m.Firm, FieldFlex.twins, new LookupSettings { EntityName = EntityType.Instance.Firm(), ReadOnly = Model.HasAssignedOrder }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 86 "..\..\Views\CreateOrUpdate\Advertisement.cshtml"
       Write(Html.TemplateField(m => m.AdvertisementTemplate, FieldFlex.twins, new LookupSettings { EntityName = EntityType.Instance.AdvertisementTemplate(), ReadOnly = Model.Id != 0, ExtendedInfo = "isPublished=true"}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 89 "..\..\Views\CreateOrUpdate\Advertisement.cshtml"
       Write(Html.TemplateField(m => m.Comment, FieldFlex.lone, new Dictionary<string, object>{{"rows", "2"}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n</div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
