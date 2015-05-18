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

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Areas.MultiCulture.Views.ClientsMerging
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Areas/MultiCulture/Views/ClientsMerging/Merge.cshtml")]
    public partial class Merge : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.MultiCultureMergeClientsViewModel>
    {
        public Merge()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Areas\MultiCulture\Views\ClientsMerging\Merge.cshtml"
  
    Layout = "../../../../Views/Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 7 "..\..\Areas\MultiCulture\Views\ClientsMerging\Merge.cshtml"
            Write(BLResources.MergeRecords);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 8 "..\..\Areas\MultiCulture\Views\ClientsMerging\Merge.cshtml"
                  Write(BLResources.MergeRecords);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 9 "..\..\Areas\MultiCulture\Views\ClientsMerging\Merge.cshtml"
                    Write(BLResources.MergeRecordsLegend);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

WriteLiteral("\r\n");

DefineSection("PageContent", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 358), Tuple.Create("\"", 408)
, Tuple.Create(Tuple.Create("", 364), Tuple.Create("/Scripts/MergeClients.js?", 364), true)
            
            #line 13 "..\..\Areas\MultiCulture\Views\ClientsMerging\Merge.cshtml"
, Tuple.Create(Tuple.Create("", 389), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 389), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 455), Tuple.Create("\"", 531)
, Tuple.Create(Tuple.Create("", 461), Tuple.Create("/Scripts/MultiCulture/MergeClients.MultiCulture.js?", 461), true)
            
            #line 14 "..\..\Areas\MultiCulture\Views\ClientsMerging\Merge.cshtml"
, Tuple.Create(Tuple.Create("", 512), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 512), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(@">
        #header td, #dataRegion td {
            border-bottom: #dddddd 1px solid;
            background-color: #ffffff;
        }

        #dataRegion table td {
            padding: 1px;
            border-bottom: #dddddd 1px solid;
            background-color: #ffffff;
            text-indent: 5px;
        }

        #dataRegion td.datacell.selected {
            border-bottom: #333333 1px solid;
            background-color: #a7cdf0;
            border-top: #ffffff 1px solid;
        }

        #dataRegion td.datacell {
            cursor: pointer;
            white-space: nowrap;
        }

        #dataRegion tr.section-header td {
            background-color: #f0f0f0 !important;
            border-bottom: #000000 1px solid !important;
            cursor: pointer;
        }
    </style>
");

            
            #line 46 "..\..\Areas\MultiCulture\Views\ClientsMerging\Merge.cshtml"
    
            
            #line default
            #line hidden
            
            #line 46 "..\..\Areas\MultiCulture\Views\ClientsMerging\Merge.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {

            
            #line default
            #line hidden
WriteLiteral("        <div");

WriteLiteral(" style=\"padding: 5px; overflow: auto; vertical-align: top;\"");

WriteLiteral(">\r\n            <table");

WriteLiteral(" style=\"width: 100%; height: 100%; border-collapse: collapse;\"");

WriteLiteral(">\r\n                <colgroup>\r\n                    <col");

WriteLiteral(" width=\"165\"");

WriteLiteral(" />\r\n                    <col");

WriteLiteral(" width=\"20\"");

WriteLiteral(" />\r\n                    <col />\r\n                </colgroup>\r\n                <t" +
"body>\r\n                    <tr");

WriteLiteral(" height=\"20\"");

WriteLiteral(">\r\n                        <td");

WriteLiteral(" colspan=\"3\"");

WriteLiteral(">\r\n                            <table");

WriteLiteral(" style=\"border-collapse: collapse; border-bottom: medium none; border-left: #889d" +
"c2 1px solid;\r\n                            border-top: #889dc2 1px solid; border" +
"-right: #889dc2 1px solid; width: 100%;\r\n                            height: 100" +
"%;\"");

WriteLiteral(">\r\n                                <colgroup>\r\n                                  " +
"  <col");

WriteLiteral(" width=\"16\"");

WriteLiteral(" />\r\n                                    <col />\r\n                               " +
" </colgroup>\r\n                                <tbody>\r\n                         " +
"           <tr>\r\n                                        <td");

WriteLiteral(" style=\"background-color: #a7cdf0; font-weight: bold;\"");

WriteLiteral("></td>\r\n                                        <td");

WriteLiteral(" style=\"background-color: #a7cdf0; font-weight: bold;\"");

WriteLiteral(">\r\n");

WriteLiteral("                                            ");

            
            #line 69 "..\..\Areas\MultiCulture\Views\ClientsMerging\Merge.cshtml"
                                       Write(BLResources.MergeRecordsSelectMainRecord);

            
            #line default
            #line hidden
WriteLiteral("\r\n                                        </td>\r\n                                " +
"    </tr>\r\n                                </tbody>\r\n                           " +
" </table>\r\n                        </td>\r\n                    </tr>\r\n           " +
"         <tr");

WriteLiteral(" height=\"40\"");

WriteLiteral(">\r\n                        <td");

WriteLiteral(" colspan=\"3\"");

WriteLiteral(">\r\n                            <table");

WriteLiteral(" id=\"header\"");

WriteLiteral(" style=\"border-collapse: collapse; width: 100%; height: 100%; table-layout: fixed" +
";\r\n                            border-left: #889dc2 1px solid; border-top: #889d" +
"c2 1px solid; border-right: #889dc2 1px solid;\"");

WriteLiteral(">\r\n                                <colgroup>\r\n                                  " +
"  <col");

WriteLiteral(" width=\"165\"");

WriteLiteral(" />\r\n                                    <col");

WriteLiteral(" width=\"20\"");

WriteLiteral(" />\r\n                                    <col />\r\n                               " +
"     <col");

WriteLiteral(" width=\"20\"");

WriteLiteral(" />\r\n                                    <col");

WriteLiteral(" width=\"20\"");

WriteLiteral(" />\r\n                                    <col />\r\n                               " +
"     <col");

WriteLiteral(" width=\"20\"");

WriteLiteral(" />\r\n                                </colgroup>\r\n                               " +
" <tbody>\r\n                                    <tr");

WriteLiteral(" height=\"40\"");

WriteLiteral(">\r\n                                        <td");

WriteLiteral(" style=\"font-weight: bold; text-indent: 5px;\"");

WriteLiteral(">\r\n");

WriteLiteral("                                            ");

            
            #line 92 "..\..\Areas\MultiCulture\Views\ClientsMerging\Merge.cshtml"
                                       Write(BLResources.MainRecord);

            
            #line default
            #line hidden
WriteLiteral("\r\n                                        </td>\r\n                                " +
"        <td");

WriteLiteral(" style=\"text-indent: 5px;\"");

WriteLiteral(">\r\n                                            <input");

WriteLiteral(" type=\"radio\"");

WriteLiteral(" name=\"client\"");

WriteLiteral(" id=\"client_1\"");

WriteLiteral(" class=\"rad left\"");

WriteLiteral(" checked=\"checked\"");

WriteLiteral(" />\r\n                                        </td>\r\n                             " +
"           <td>\r\n");

WriteLiteral("                                            ");

            
            #line 98 "..\..\Areas\MultiCulture\Views\ClientsMerging\Merge.cshtml"
                                       Write(Html.LookupFor(m => m.Client1, new LookupSettings { EntityName = EntityType.Instance.Client(), ExtendedInfo = "restrictForMergeId={Client2Id}", ReadOnly = Model.DisableMasterClient }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                            ");

            
            #line 99 "..\..\Areas\MultiCulture\Views\ClientsMerging\Merge.cshtml"
                                       Write(Html.ValidationMessageFor(m=>m.Client1));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                        </td>\r\n                                " +
"        <td />\r\n                                        <td");

WriteLiteral(" style=\"text-indent: 5px;\"");

WriteLiteral(">\r\n                                            <input");

WriteLiteral(" type=\"radio\"");

WriteLiteral(" name=\"client\"");

WriteLiteral(" id=\"client_2\"");

WriteLiteral(" class=\"rad right\"");

WriteLiteral(" />\r\n                                        </td>\r\n                             " +
"           <td>\r\n");

WriteLiteral("                                            ");

            
            #line 106 "..\..\Areas\MultiCulture\Views\ClientsMerging\Merge.cshtml"
                                       Write(Html.LookupFor(m => m.Client2, new LookupSettings { EntityName = EntityType.Instance.Client(), ExtendedInfo = "restrictForMergeId={Client1Id}" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                                            ");

            
            #line 107 "..\..\Areas\MultiCulture\Views\ClientsMerging\Merge.cshtml"
                                       Write(Html.ValidationMessageFor(m=>m.Client2));

            
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
                        <td");

WriteLiteral(" colspan=\"3\"");

WriteLiteral(" id=\"dataRegion\"");

WriteLiteral(" />\r\n                    </tr>\r\n                    <tr>\r\n                       " +
" <td");

WriteLiteral(" style=\"vertical-align: bottom\"");

WriteLiteral(">\r\n");

WriteLiteral("                            ");

            
            #line 119 "..\..\Areas\MultiCulture\Views\ClientsMerging\Merge.cshtml"
                       Write(Html.CheckBoxFor(m => m.AssignAllObjects));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                            ");

            
            #line 120 "..\..\Areas\MultiCulture\Views\ClientsMerging\Merge.cshtml"
                       Write(Html.LabelFor(m => m.AssignAllObjects, BLResources.AssignInAllHierarchy, new { style = "font-weight: bold" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                        </td>\r\n                    </tr>\r\n                    <" +
"tr");

WriteLiteral(" height=\"5px;\"");

WriteLiteral(">\r\n                        <td />\r\n                    </tr>\r\n                   " +
" <tr");

WriteLiteral(" height=\"10px;\"");

WriteLiteral(">\r\n                        <td />\r\n                    </tr>\r\n                   " +
" <tr");

WriteLiteral(" height=\"25px;\"");

WriteLiteral(">\r\n                        <td");

WriteLiteral(" colspan=\"3\"");

WriteLiteral(">\r\n                            <div");

WriteLiteral(" style=\"visibility: hidden;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(" class=\"Notifications\"");

WriteLiteral(">\r\n                            </div>\r\n");

WriteLiteral("                            ");

            
            #line 133 "..\..\Areas\MultiCulture\Views\ClientsMerging\Merge.cshtml"
                       Write(Html.Hidden("Id"));

            
            #line default
            #line hidden
WriteLiteral(";\r\n");

WriteLiteral("                            ");

            
            #line 134 "..\..\Areas\MultiCulture\Views\ClientsMerging\Merge.cshtml"
                       Write(Html.Hidden("Timestamp"));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                            ");

            
            #line 135 "..\..\Areas\MultiCulture\Views\ClientsMerging\Merge.cshtml"
                       Write(Html.Hidden("AppendedClient"));

            
            #line default
            #line hidden
WriteLiteral("\r\n                        </td>\r\n                    </tr>\r\n                </tbo" +
"dy>\r\n            </table>\r\n        </div>\r\n");

WriteLiteral("        <div");

WriteLiteral(" style=\"display: none\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" id=\"MessageType\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 143 "..\..\Areas\MultiCulture\Views\ClientsMerging\Merge.cshtml"
           Write(Model.MessageType);

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n            <div");

WriteLiteral(" id=\"Message\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 146 "..\..\Areas\MultiCulture\Views\ClientsMerging\Merge.cshtml"
           Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n        </div>\r\n");

            
            #line 149 "..\..\Areas\MultiCulture\Views\ClientsMerging\Merge.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
