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

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Views.Administration
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Administration/MessageQueueManagement.cshtml")]
    public partial class MessageQueueManagement : System.Web.Mvc.WebViewPage<Models.Administration.MessageQueueManagementViewModel>
    {
        public MessageQueueManagement()
        {
        }
        public override void Execute()
        {
WriteLiteral("<html");

WriteLiteral(" xmlns=\"http://www.w3.org/1999/xhtml\"");

WriteLiteral(">\r\n<head>\r\n    <title>");

            
            #line 5 "..\..\Views\Administration\MessageQueueManagement.cshtml"
      Write(BLResources.MessageQueueManagementPage_Header);

            
            #line default
            #line hidden
WriteLiteral("</title>\r\n\r\n    <meta");

WriteLiteral(" id=\"meta_IsDebug\"");

WriteLiteral(" name=\"meta_IsDebug\"");

WriteAttribute("content", Tuple.Create(" content=\"", 244), Tuple.Create("\"", 293)
            
            #line 7 "..\..\Views\Administration\MessageQueueManagement.cshtml"
, Tuple.Create(Tuple.Create("", 254), Tuple.Create<System.Object, System.Int32>(HttpContext.Current.IsDebuggingEnabled
            
            #line default
            #line hidden
, 254), false)
);

WriteLiteral(" />\r\n    <meta");

WriteLiteral(" id=\"meta_Revision\"");

WriteLiteral(" name=\"meta_Revision\"");

WriteAttribute("content", Tuple.Create(" content=\"", 348), Tuple.Create("\"", 392)
            
            #line 8 "..\..\Views\Administration\MessageQueueManagement.cshtml"
, Tuple.Create(Tuple.Create("", 358), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 358), false)
);

WriteLiteral(" />\r\n    \r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 446), Tuple.Create("\"", 507)
, Tuple.Create(Tuple.Create("", 453), Tuple.Create("/Content/Global.css?", 453), true)
            
            #line 10 "..\..\Views\Administration\MessageQueueManagement.cshtml"
, Tuple.Create(Tuple.Create("", 473), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 473), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 555), Tuple.Create("\"", 624)
, Tuple.Create(Tuple.Create("", 562), Tuple.Create("/Content/Administration.css?", 562), true)
            
            #line 11 "..\..\Views\Administration\MessageQueueManagement.cshtml"
, Tuple.Create(Tuple.Create("", 590), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 590), false)
);

WriteLiteral("/>\r\n    \r\n");

            
            #line 13 "..\..\Views\Administration\MessageQueueManagement.cshtml"
    
            
            #line default
            #line hidden
            
            #line 13 "..\..\Views\Administration\MessageQueueManagement.cshtml"
     if (HttpContext.Current.IsDebuggingEnabled)
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 707), Tuple.Create("\"", 774)
, Tuple.Create(Tuple.Create("", 713), Tuple.Create("/Scripts/ext-base-debug.js?", 713), true)
            
            #line 15 "..\..\Views\Administration\MessageQueueManagement.cshtml"
, Tuple.Create(Tuple.Create("", 740), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 740), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 825), Tuple.Create("\"", 902)
, Tuple.Create(Tuple.Create("", 831), Tuple.Create("/Scripts/ext-all-debug-w-comments.js?", 831), true)
            
            #line 16 "..\..\Views\Administration\MessageQueueManagement.cshtml"
, Tuple.Create(Tuple.Create("", 868), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 868), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 17 "..\..\Views\Administration\MessageQueueManagement.cshtml"
    }
    else
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 977), Tuple.Create("\"", 1038)
, Tuple.Create(Tuple.Create("", 983), Tuple.Create("/Scripts/ext-base.js?", 983), true)
            
            #line 20 "..\..\Views\Administration\MessageQueueManagement.cshtml"
, Tuple.Create(Tuple.Create("", 1004), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1004), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1089), Tuple.Create("\"", 1149)
, Tuple.Create(Tuple.Create("", 1095), Tuple.Create("/Scripts/ext-all.js?", 1095), true)
            
            #line 21 "..\..\Views\Administration\MessageQueueManagement.cshtml"
, Tuple.Create(Tuple.Create("", 1115), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1115), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 22 "..\..\Views\Administration\MessageQueueManagement.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1205), Tuple.Create("\"", 1347)
, Tuple.Create(Tuple.Create("", 1211), Tuple.Create("/Scripts/", 1211), true)
            
            #line 24 "..\..\Views\Administration\MessageQueueManagement.cshtml"
, Tuple.Create(Tuple.Create("", 1220), Tuple.Create<System.Object, System.Int32>("Ext.LocalizedResources." + ViewData.GetUserLocaleInfo().TwoLetterISOLanguageName + ".js"
            
            #line default
            #line hidden
, 1220), false)
, Tuple.Create(Tuple.Create("", 1312), Tuple.Create("?", 1312), true)
            
            #line 24 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                                       , Tuple.Create(Tuple.Create("", 1313), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1313), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.USER_ID = ");

            
            #line 27 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                 Write(ViewData.GetUserIdentityInfo().Code);

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.USER_NAME = ");

            
            #line 28 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                   Write(ViewData.GetUserIdentityDisplayName());

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo = ");

            
            #line 29 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                     Write(Html.WriteJson(ViewData.GetUserLocaleInfo()));

            
            #line default
            #line hidden
WriteLiteral(";\r\n        Ext.CultureInfo.NumberFormatInfo.CurrencySymbol = \'");

            
            #line 30 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                                                      Write(ViewData.GetErmBaseCurrencySymbol());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.CRM_URL = \'");

            
            #line 31 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                  Write(ViewData.GetMsCrmSettingsUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.BasicOperationsServiceRestUrl = \'");

            
            #line 32 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                                        Write(ViewData.GetBasicOperationsServiceRestUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n        Ext.SpecialOperationsServiceRestUrl = \'");

            
            #line 33 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                                          Write(ViewData.GetSpecialOperationsServiceRestUrl());

            
            #line default
            #line hidden
WriteLiteral("\';\r\n    </script>\r\n    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 2001), Tuple.Create("\"", 2079)
, Tuple.Create(Tuple.Create("", 2007), Tuple.Create("/Scripts/DoubleGis.GlobalVariables.js?", 2007), true)
            
            #line 36 "..\..\Views\Administration\MessageQueueManagement.cshtml"
, Tuple.Create(Tuple.Create("", 2045), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 2045), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(">\r\n        .style1\r\n        {\r\n            width: 48px;\r\n        }\r\n    </style>\r" +
"\n</head>\r\n<body>\r\n    <table");

WriteLiteral(" cellspacing=\"0\"");

WriteLiteral(" cellpadding=\"0\"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(" height=\"100%\"");

WriteLiteral(">\r\n        <tbody>\r\n            <tr>\r\n                <td>\r\n                    <" +
"div");

WriteLiteral(" class=\"content\"");

WriteLiteral(">\r\n");

            
            #line 51 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                        
            
            #line default
            #line hidden
            
            #line 51 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                         if (Model.HasAccessToCorporateQueue)
                        {

            
            #line default
            #line hidden
WriteLiteral("                        <div");

WriteLiteral(" class=\"subtitle\"");

WriteLiteral(">");

            
            #line 53 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                                         Write(BLResources.SecurityAdministrationPage_HeaderExternalSystems);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n");

WriteLiteral("                        <div");

WriteLiteral(" class=\"headline\"");

WriteLiteral(" />\r\n");

WriteLiteral("                        <table");

WriteLiteral(" style=\"table-layout: fixed\"");

WriteLiteral(" cellspacing=\"0\"");

WriteLiteral(" cellpadding=\"0\"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(">\r\n                            <tbody>\r\n                                <tr>\r\n   " +
"                                 <td>\r\n                                        <" +
"table");

WriteLiteral(" style=\"cursor: hand\"");

WriteLiteral(" id=\"_L01\"");

WriteLiteral(" tabindex=\"0\"");

WriteLiteral(" cellspacing=\"0\"");

WriteLiteral(" cellpadding=\"12\"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(">\r\n                                            <tbody>\r\n                         " +
"                       <tr>\r\n                                                   " +
" <td");

WriteLiteral(" onclick=\"window.location.href = \'../Grid/View/LocalMessage\' \"");

WriteLiteral(" class=\"style1\"");

WriteLiteral(">\r\n                                                        <div");

WriteLiteral(" class=\"icon_48 icon_default\"");

WriteLiteral(" />\r\n                                                    </td>\r\n                 " +
"                                   <td");

WriteLiteral(" style=\"padding-left: 2px\"");

WriteLiteral(">\r\n                                                        <a");

WriteLiteral(" id=\"_A01\"");

WriteLiteral(" class=\"linktitle ms-crm-Link\"");

WriteLiteral(" onclick=\" window.location.href = \'../Grid/View/LocalMessage\' \"");

WriteLiteral(" href=\"#\"");

WriteLiteral(">");

            
            #line 66 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                                                                                                                                                                      Write(BLResources.SecurityAdministrationPage_LocalMessages);

            
            #line default
            #line hidden
WriteLiteral("</a>\r\n                                                        <div");

WriteLiteral(" id=\"_I01\"");

WriteLiteral(" class=\"linkhelp\"");

WriteLiteral(" onclick=\" window.location.href = \'../Grid/View/LocalMessage\'\"");

WriteLiteral(">");

            
            #line 67 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                                                                                                                                                 Write(BLResources.SecurityAdministrationPage_LocalMessageDetails);

            
            #line default
            #line hidden
WriteLiteral(@"</div>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                    <td>
                                        <table");

WriteLiteral(" style=\"cursor: hand\"");

WriteLiteral(" id=\"Table10\"");

WriteLiteral(" tabindex=\"0\"");

WriteLiteral(" cellspacing=\"0\"");

WriteLiteral(" cellpadding=\"12\"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(">\r\n                                            <tbody>\r\n                         " +
"                       <tr>\r\n                                                   " +
" <td");

WriteLiteral(" class=\"style1\"");

WriteLiteral("></td>\r\n                                                    <td");

WriteLiteral(" style=\"padding-left: 2px\"");

WriteLiteral(@"></td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
");

            
            #line 86 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                        }

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 88 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                        
            
            #line default
            #line hidden
            
            #line 88 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                         if (Model.HasAccessToRelease || Model.HasAccessToWithdrawal)
                        {

            
            #line default
            #line hidden
WriteLiteral("                        <div");

WriteLiteral(" class=\"subtitle\"");

WriteLiteral(">");

            
            #line 90 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                                         Write(BLResources.SecurityAdministrationPage_HeaderReleasing);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n");

WriteLiteral("                        <div");

WriteLiteral(" class=\"headline\"");

WriteLiteral(" />\r\n");

WriteLiteral("                        <table");

WriteLiteral(" style=\"table-layout: fixed\"");

WriteLiteral(" cellspacing=\"0\"");

WriteLiteral(" cellpadding=\"0\"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(">\r\n                            <tbody>\r\n                                <tr>\r\n   " +
"                                 <td>\r\n");

            
            #line 96 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                                        
            
            #line default
            #line hidden
            
            #line 96 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                                         if (Model.HasAccessToRelease)
                                        {

            
            #line default
            #line hidden
WriteLiteral("                                            <table");

WriteLiteral(" style=\"cursor: hand\"");

WriteLiteral(" id=\"Table13\"");

WriteLiteral(" tabindex=\"0\"");

WriteLiteral(" cellspacing=\"0\"");

WriteLiteral(" cellpadding=\"12\"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(">\r\n                                                <tbody>\r\n                     " +
"                               <tr>\r\n                                           " +
"             <td");

WriteLiteral(" onclick=\"\"");

WriteLiteral(" width=\"48\"");

WriteLiteral(">\r\n                                                            <div");

WriteLiteral(" class=\"icon_48 icon_default\"");

WriteLiteral(" />\r\n                                                        </td>\r\n             " +
"                                           <td");

WriteLiteral(" style=\"padding-left: 2px\"");

WriteLiteral(">\r\n                                                            <a");

WriteLiteral(" id=\"A11\"");

WriteLiteral(" class=\"linktitle ms-crm-Link\"");

WriteLiteral(" onclick=\"window.location.href = \'../Grid/View/ReleaseInfo\'\"");

WriteLiteral(" href=\"#\"");

WriteLiteral(">");

            
            #line 105 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                                                                                                                                                                      Write(BLResources.SecurityAdministrationPage_ReleaseInfos);

            
            #line default
            #line hidden
WriteLiteral("</a>\r\n                                                            <div");

WriteLiteral(" id=\"Div11\"");

WriteLiteral(" class=\"linkhelp\"");

WriteLiteral(" onclick=\"window.location.href = \'../Grid/View/ReleaseInfo\'\"");

WriteLiteral(">");

            
            #line 106 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                                                                                                                                                    Write(BLResources.SecurityAdministrationPage_ReleaseInfoDetails);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                                                        </td>\r\n          " +
"                                          </tr>\r\n                               " +
"                 </tbody>\r\n                                            </table>\r" +
"\n");

            
            #line 111 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                                        }

            
            #line default
            #line hidden
WriteLiteral("                                    </td>\r\n                                    <t" +
"d>\r\n");

            
            #line 114 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                                        
            
            #line default
            #line hidden
            
            #line 114 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                                         if (Model.HasAccessToWithdrawal)
                                        {

            
            #line default
            #line hidden
WriteLiteral("                                        <table");

WriteLiteral(" style=\"cursor: hand\"");

WriteLiteral(" id=\"Table12\"");

WriteLiteral(" tabindex=\"0\"");

WriteLiteral(" cellspacing=\"0\"");

WriteLiteral(" cellpadding=\"12\"");

WriteLiteral(" width=\"100%\"");

WriteLiteral(">\r\n                                            <tbody>\r\n                         " +
"                       <tr>\r\n                                                   " +
" <td");

WriteLiteral(" onclick=\"\"");

WriteLiteral(" width=\"48\"");

WriteLiteral(">\r\n                                                        <div");

WriteLiteral(" class=\"icon_48 icon_default\"");

WriteLiteral(" />\r\n                                                    </td>\r\n                 " +
"                                   <td");

WriteLiteral(" style=\"padding-left: 2px\"");

WriteLiteral(">\r\n                                                        <a");

WriteLiteral(" id=\"A8\"");

WriteLiteral(" class=\"linktitle ms-crm-Link\"");

WriteLiteral(" onclick=\"window.location.href = \'../Grid/View/WithdrawalInfo\'\"");

WriteLiteral(" href=\"#\"");

WriteLiteral(">");

            
            #line 123 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                                                                                                                                                                    Write(BLResources.SecurityAdministrationPage_WithdrawalInfos);

            
            #line default
            #line hidden
WriteLiteral("</a>\r\n                                                        <div");

WriteLiteral(" id=\"Div8\"");

WriteLiteral(" class=\"linkhelp\"");

WriteLiteral(" onclick=\"window.location.href = \'../Grid/View/WithdrawalInfo\'\"");

WriteLiteral(">");

            
            #line 124 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                                                                                                                                                  Write(BLResources.SecurityAdministrationPage_WithdrawalInfoDetails);

            
            #line default
            #line hidden
WriteLiteral(@"</div>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>                                                 
");

            
            #line 129 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                                        }

            
            #line default
            #line hidden
WriteLiteral("                                    </td>\r\n                                </tr>\r" +
"\n                            </tbody>\r\n                        </table>\r\n");

            
            #line 134 "..\..\Views\Administration\MessageQueueManagement.cshtml"
                        }

            
            #line default
            #line hidden
WriteLiteral("                    </div>\r\n                </td>\r\n            </tr>\r\n        </t" +
"body>\r\n    </table>\r\n</body>\r\n</html>\r\n");

        }
    }
}
#pragma warning restore 1591
