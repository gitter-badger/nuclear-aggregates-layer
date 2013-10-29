﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18331
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.BLFlex.Web.Mvc.Global.Areas.Russia.Views.LegalPerson
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
    using DoubleGis.Erm.BL.Resources.Server.Properties;
    using DoubleGis.Erm.Core;
    using DoubleGis.Erm.Core.Enums;
    
    #line 1 "..\..\Areas\Russia\Views\LegalPerson\Merge.cshtml"
    using DoubleGis.Erm.Platform.Model.Entities;
    
    #line default
    #line hidden
    using DoubleGis.Erm.UI.Web.Mvc;
    using DoubleGis.Erm.UI.Web.Mvc.Controllers;
    using DoubleGis.Erm.UI.Web.Mvc.Models;
    using DoubleGis.Erm.UI.Web.Mvc.Utils;
    
    #line 2 "..\..\Areas\Russia\Views\LegalPerson\Merge.cshtml"
    using Platform.Common;
    
    #line default
    #line hidden
    
    #line 3 "..\..\Areas\Russia\Views\LegalPerson\Merge.cshtml"
    using Platform.Web.Mvc.Utils;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Areas/Russia/Views/LegalPerson/Merge.cshtml")]
    public partial class Merge : System.Web.Mvc.WebViewPage<MergeLegalPersonsViewModel>
    {
        public Merge()
        {
        }
        public override void Execute()
        {
            
            #line 6 "..\..\Areas\Russia\Views\LegalPerson\Merge.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 10 "..\..\Areas\Russia\Views\LegalPerson\Merge.cshtml"
            Write(BLResources.MergeRecords);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 11 "..\..\Areas\Russia\Views\LegalPerson\Merge.cshtml"
                  Write(BLResources.MergeRecords);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 12 "..\..\Areas\Russia\Views\LegalPerson\Merge.cshtml"
                    Write(BLResources.MergeLegalPersonsLegend);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

WriteLiteral("\r\n");

DefineSection("PageContent", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 396), Tuple.Create("\"", 466)
, Tuple.Create(Tuple.Create("", 402), Tuple.Create("/Scripts/MergeLegalPersons.js?", 402), true)
            
            #line 16 "..\..\Areas\Russia\Views\LegalPerson\Merge.cshtml"
, Tuple.Create(Tuple.Create("", 432), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 432), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(@">
        #header td, #dataRegion td
        {
            border-bottom: #dddddd 1px solid;
            background-color: #ffffff;
        }
        #dataRegion table td
        {
            padding: 1px;
            border-bottom: #dddddd 1px solid;
            background-color: #ffffff;
            text-indent: 5px;
        }
        #dataRegion td.datacell.selected
        {
            border-bottom: #333333 1px solid;
            background-color: #a7cdf0;
            border-top: #ffffff 1px solid;
        }
        #dataRegion td.datacell
        {
            cursor: pointer;
            white-space: nowrap;
        }
        #dataRegion tr.section-header td
        {
            background-color: #f0f0f0 !important;
            border-bottom: #000000 1px solid !important;
            cursor: pointer;
        }
    </style>
");

            
            #line 48 "..\..\Areas\Russia\Views\LegalPerson\Merge.cshtml"
    
            
            #line default
            #line hidden
            
            #line 48 "..\..\Areas\Russia\Views\LegalPerson\Merge.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" style=\"padding: 5px; overflow: auto; vertical-align: top;\"");

WriteLiteral(">\r\n        <table");

WriteLiteral(" style=\"width: 100%; height: 100%; border-collapse: collapse;\"");

WriteLiteral(">\r\n            <colgroup>\r\n                <col");

WriteLiteral(" width=\"165\"");

WriteLiteral(" />\r\n                <col");

WriteLiteral(" width=\"20\"");

WriteLiteral(" />\r\n                <col />\r\n            </colgroup>\r\n            <tbody>\r\n     " +
"           <tr");

WriteLiteral(" height=\"20\"");

WriteLiteral(">\r\n                    <td");

WriteLiteral(" colspan=\"3\"");

WriteLiteral(">\r\n                        <table");

WriteLiteral(" style=\"border-collapse: collapse; border-bottom: medium none; border-left: #889d" +
"c2 1px solid;\r\n                            border-top: #889dc2 1px solid; border" +
"-right: #889dc2 1px solid; width: 100%;\r\n                            height: 100" +
"%;\"");

WriteLiteral(">\r\n                            <colgroup>\r\n                                <col");

WriteLiteral(" width=\"16\"");

WriteLiteral(" />\r\n                                <col />\r\n                            </colgr" +
"oup>\r\n                            <tbody>\r\n                                <tr>\r" +
"\n                                    <td");

WriteLiteral(" style=\"background-color: #a7cdf0; font-weight: bold;\"");

WriteLiteral(">\r\n                                    </td>\r\n                                   " +
" <td");

WriteLiteral(" style=\"background-color: #a7cdf0; font-weight: bold;\"");

WriteLiteral(">\r\n");

WriteLiteral("                                        ");

            
            #line 72 "..\..\Areas\Russia\Views\LegalPerson\Merge.cshtml"
                                   Write(BLResources.MergeRecordsSelectMainRecord);

            
            #line default
            #line hidden
WriteLiteral("\r\n                                    </td>\r\n                                </tr" +
">\r\n                            </tbody>\r\n                        </table>\r\n     " +
"               </td>\r\n                </tr>\r\n                <tr");

WriteLiteral(" height=\"40\"");

WriteLiteral(">\r\n                    <td");

WriteLiteral(" colspan=\"3\"");

WriteLiteral(">\r\n                        <table");

WriteLiteral(" id=\"header\"");

WriteLiteral(" style=\"border-collapse: collapse; width: 100%; height: 100%; table-layout: fixed" +
";\r\n                            border-left: #889dc2 1px solid; border-top: #889d" +
"c2 1px solid; border-right: #889dc2 1px solid;\"");

WriteLiteral(">\r\n                            <colgroup>\r\n                                <col");

WriteLiteral(" width=\"165\"");

WriteLiteral(" />\r\n                                <col");

WriteLiteral(" width=\"20\"");

WriteLiteral(" />\r\n                                <col />\r\n                                <co" +
"l");

WriteLiteral(" width=\"20\"");

WriteLiteral(" />\r\n                                <col");

WriteLiteral(" width=\"20\"");

WriteLiteral(" />\r\n                                <col />\r\n                                <co" +
"l");

WriteLiteral(" width=\"20\"");

WriteLiteral(" />\r\n                            </colgroup>\r\n                            <tbody>" +
"\r\n                                <tr");

WriteLiteral(" height=\"40\"");

WriteLiteral(">\r\n                                    <td");

WriteLiteral(" style=\"font-weight: bold; text-indent: 5px;\"");

WriteLiteral(">\r\n");

WriteLiteral("                                        ");

            
            #line 95 "..\..\Areas\Russia\Views\LegalPerson\Merge.cshtml"
                                   Write(BLResources.MainRecord);

            
            #line default
            #line hidden
WriteLiteral("\r\n                                    </td>\r\n                                    " +
"<td");

WriteLiteral(" style=\"text-indent: 5px;\"");

WriteLiteral(">\r\n                                        <input");

WriteLiteral(" type=\"radio\"");

WriteLiteral(" name=\"legalPerson\"");

WriteLiteral(" id=\"legalPerson_1\"");

WriteLiteral(" class=\"rad left\"");

WriteLiteral(" checked=\"checked\"");

WriteLiteral(" />\r\n                                    </td>\r\n                                 " +
"   <td>\r\n");

WriteLiteral("                                        ");

            
            #line 101 "..\..\Areas\Russia\Views\LegalPerson\Merge.cshtml"
                                   Write(Html.LookupFor(m => m.LegalPerson1, new LookupSettings { EntityName = EntityName.LegalPerson, ExtendedInfo = "restrictForMergeId={LegalPerson2Id}" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                        ");

            
            #line 102 "..\..\Areas\Russia\Views\LegalPerson\Merge.cshtml"
                                   Write(Html.ValidationMessageFor(m => m.LegalPerson1));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                    </td>\r\n                                    " +
"<td />\r\n                                    <td");

WriteLiteral(" style=\"text-indent: 5px;\"");

WriteLiteral(">\r\n                                        <input");

WriteLiteral(" type=\"radio\"");

WriteLiteral(" name=\"legalPerson\"");

WriteLiteral(" id=\"legalPerson_2\"");

WriteLiteral(" class=\"rad right\"");

WriteLiteral(" />\r\n                                    </td>\r\n                                 " +
"   <td>\r\n");

WriteLiteral("                                        ");

            
            #line 109 "..\..\Areas\Russia\Views\LegalPerson\Merge.cshtml"
                                   Write(Html.LookupFor(m => m.LegalPerson2, new LookupSettings { EntityName = EntityName.LegalPerson, ExtendedInfo = "restrictForMergeId={LegalPerson1Id}" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                        ");

            
            #line 110 "..\..\Areas\Russia\Views\LegalPerson\Merge.cshtml"
                                   Write(Html.ValidationMessageFor(m => m.LegalPerson2));

            
            #line default
            #line hidden
WriteLiteral(@"
                                    </td>
                                </tr>
                                <tr>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td");

WriteLiteral(" colspan=\"3\"");

WriteLiteral(" id=\"dataRegion\"");

WriteLiteral(" />\r\n                </tr>\r\n                <tr");

WriteLiteral(" height=\"5px;\"");

WriteLiteral(">\r\n                    <td />\r\n                </tr>\r\n                <tr");

WriteLiteral(" height=\"10px;\"");

WriteLiteral(">\r\n                    <td />\r\n                </tr>\r\n                <tr");

WriteLiteral(" height=\"25px;\"");

WriteLiteral(">\r\n                    <td");

WriteLiteral(" colspan=\"3\"");

WriteLiteral(">\r\n                        <div");

WriteLiteral(" style=\"visibility: hidden;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(" class=\"Notifications\"");

WriteLiteral(">\r\n                        </div>\r\n");

WriteLiteral("                        ");

            
            #line 132 "..\..\Areas\Russia\Views\LegalPerson\Merge.cshtml"
                   Write(Html.Hidden("AppendedLegalPersonId"));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                        ");

            
            #line 133 "..\..\Areas\Russia\Views\LegalPerson\Merge.cshtml"
                   Write(Html.Hidden("MainLegalPersonId"));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </td>\r\n                </tr>\r\n            </tbody>\r\n       " +
" </table>\r\n    </div>\r\n");

WriteLiteral("    <div");

WriteLiteral(" style=\"display: none\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" id=\"MessageType\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 141 "..\..\Areas\Russia\Views\LegalPerson\Merge.cshtml"
       Write(Model.MessageType);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n        <div");

WriteLiteral(" id=\"Message\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 143 "..\..\Areas\Russia\Views\LegalPerson\Merge.cshtml"
       Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n    </div>\r\n");

            
            #line 145 "..\..\Areas\Russia\Views\LegalPerson\Merge.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
