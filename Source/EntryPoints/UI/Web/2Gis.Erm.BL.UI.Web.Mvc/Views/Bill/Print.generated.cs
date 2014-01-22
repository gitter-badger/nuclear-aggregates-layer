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

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Views.Bill
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Bill/Print.cshtml")]
    public partial class Print : System.Web.Mvc.WebViewPage<Models.PrintOrderViewModel>
    {
        public Print()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 4 "..\..\Views\Bill\Print.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 8 "..\..\Views\Bill\Print.cshtml"
            Write(BLResources.ChooseLegalPersonProfileDialogTitle);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 9 "..\..\Views\Bill\Print.cshtml"
                  Write(BLResources.ChooseLegalPersonProfileDialogTitle);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 10 "..\..\Views\Bill\Print.cshtml"
                    Write(BLResources.ChooseLegalPersonProfile);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

WriteLiteral("\r\n");

DefineSection("PageContent", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 344), Tuple.Create("\"", 404)
, Tuple.Create(Tuple.Create("", 350), Tuple.Create("/Scripts/Tooltip.js?", 350), true)
            
            #line 14 "..\..\Views\Bill\Print.cshtml"
, Tuple.Create(Tuple.Create("", 370), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 370), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 451), Tuple.Create("\"", 532)
, Tuple.Create(Tuple.Create("", 457), Tuple.Create("/Scripts/DoubleGis.UI.GroupOperations.js?", 457), true)
            
            #line 15 "..\..\Views\Bill\Print.cshtml"
, Tuple.Create(Tuple.Create("", 498), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 498), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    \r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.onReady(function() {\r\n\r\n            Ext.each(Ext.CardLookupSetting" +
"s, function(item, i) {\r\n                new window.Ext.ux.LookupField(item);\r\n  " +
"          }, this);\r\n\r\n            Ext.get(\"Cancel\").on(\"click\", function() { wi" +
"ndow.close(); });\r\n            Ext.get(\"OK\").on(\"click\", function() {\r\n         " +
"       if (Ext.DoubleGis.FormValidator.validate(EntityForm)) {\r\n\r\n              " +
"      var billId = Ext.getDom(\'OrderId\').value;\r\n                    var profile" +
"Id = Ext.getCmp(\"LegalPersonProfile\").getValue().id;\r\n\r\n                    url " +
"= \'/Bill/PrintBill/\' + billId + \'?profileId=\' + profileId + \'&__dc=\' + Ext.util." +
"Format.cacheBuster();\r\n\r\n                    Ext.getDom(\"OK\").disabled = true;\r\n" +
"                    Ext.getDom(\"Cancel\").disabled = true;\r\n\r\n                   " +
" var iframe;\r\n                    iframe = document.getElementById(\"hiddenDownlo" +
"ader\");\r\n                    if (iframe === null) {\r\n                        ifr" +
"ame = document.createElement(\'iframe\');\r\n                        iframe.id = \"hi" +
"ddenDownloader\";\r\n                        iframe.style.visibility = \'hidden\';\r\n\r" +
"\n                        var iframeEl = new Ext.Element(iframe);\r\n              " +
"          iframeEl.on(\"load\", function() {\r\n                            var ifra" +
"meContent = iframe.contentWindow.document.documentElement.innerText;\r\n          " +
"                  if (iframeContent != \"\") {\r\n                                al" +
"ert(iframeContent);\r\n                            }\r\n                        });\r" +
"\n                        document.body.appendChild(iframe);\r\n                   " +
" }\r\n\r\n                    iframe.src = url;\r\n                    Ext.getDom(\"OK\"" +
").disabled = false;\r\n                    Ext.getDom(\"Cancel\").disabled = false;\r" +
"\n                }\r\n            });\r\n        });\r\n    </script>\r\n    \r\n");

            
            #line 61 "..\..\Views\Bill\Print.cshtml"
    
            
            #line default
            #line hidden
            
            #line 61 "..\..\Views\Bill\Print.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> {{"id", "EntityForm"}}))
    {

            
            #line default
            #line hidden
WriteLiteral("        <table");

WriteLiteral(" cellspacing=\"5\"");

WriteLiteral(" cellpadding=\"5\"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(" height=\"100%\"");

WriteLiteral(" style=\"position: fixed\"");

WriteLiteral(">\r\n            <colgroup>\r\n                <col");

WriteLiteral(" width=\"26\"");

WriteLiteral(" />\r\n                <col />\r\n            </colgroup>\r\n            <tbody>\r\n     " +
"           <tr>\r\n                    <td");

WriteLiteral(" colspan=\"2\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" style=\"height: 30px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteAttribute("onmouseover", Tuple.Create(" \r\n                             onmouseover=\"", 2926), Tuple.Create("\"", 2998)
, Tuple.Create(Tuple.Create("", 2971), Tuple.Create("AddTooltip(", 2971), true)
            
            #line 72 "..\..\Views\Bill\Print.cshtml"
, Tuple.Create(Tuple.Create("", 2982), Tuple.Create<System.Object, System.Int32>(Model.Message
            
            #line default
            #line hidden
, 2982), false)
, Tuple.Create(Tuple.Create("", 2996), Tuple.Create(");", 2996), true)
);

WriteLiteral(" \r\n                             onmouseout=\"RemoveTooltip();\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 74 "..\..\Views\Bill\Print.cshtml"
                       Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n                        </div>\r\n                    </td>\r\n                </tr" +
">\r\n                <tr>\r\n                    <td>\r\n                        <labe" +
"l");

WriteLiteral(" style=\"font-weight: bold\"");

WriteLiteral(" for=\"rdoAssignToUser\"");

WriteLiteral(">");

            
            #line 80 "..\..\Views\Bill\Print.cshtml"
                                                                          Write(BLResources.PrintWithLegalPersonProfile);

            
            #line default
            #line hidden
WriteLiteral("</label>\r\n                        <table");

WriteLiteral(" style=\"table-layout: fixed\"");

WriteLiteral(" cellspacing=\"0\"");

WriteLiteral(" cellpadding=\"0\"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(">\r\n                            <tbody>\r\n                                <tr>\r\n   " +
"                                 <td>\r\n");

WriteLiteral("                                        ");

            
            #line 85 "..\..\Views\Bill\Print.cshtml"
                                   Write(Html.TemplateField(m => m.LegalPersonProfile, FieldFlex.lone, new LookupSettings {EntityName = EntityName.LegalPersonProfile, SearchFormFilterInfo = "LegalPersonId={ViewConfig_Id} && IsActive=true && IsDeleted=false"}));

            
            #line default
            #line hidden
WriteLiteral(@"
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div");

WriteLiteral(" id=\"pbDiv\"");

WriteLiteral(">\r\n                            <div");

WriteLiteral(" id=\"pbInner\"");

WriteLiteral(">\r\n                            </div>\r\n                        </div>\r\n          " +
"          </td>\r\n                </tr>\r\n            </tbody>\r\n        </table>\r\n" +
"");

            
            #line 102 "..\..\Views\Bill\Print.cshtml"
        
            
            #line default
            #line hidden
            
            #line 102 "..\..\Views\Bill\Print.cshtml"
   Write(Html.HiddenFor(m => m.OrderId));

            
            #line default
            #line hidden
            
            #line 102 "..\..\Views\Bill\Print.cshtml"
                                       

        
            
            #line default
            #line hidden
            
            #line 104 "..\..\Views\Bill\Print.cshtml"
   Write(Html.Hidden("ViewConfig_Id", Model.LegalPersonId));

            
            #line default
            #line hidden
            
            #line 104 "..\..\Views\Bill\Print.cshtml"
                                                          
        
            
            #line default
            #line hidden
            
            #line 105 "..\..\Views\Bill\Print.cshtml"
   Write(Html.Hidden("ViewConfig_EntityName", EntityName.LegalPerson.ToString()));

            
            #line default
            #line hidden
            
            #line 105 "..\..\Views\Bill\Print.cshtml"
                                                                                
    }

            
            #line default
            #line hidden
});

WriteLiteral("\r\n");

        }
    }
}
#pragma warning restore 1591
