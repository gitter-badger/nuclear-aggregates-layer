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

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Views.Advertisement
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Advertisement/PreviewOnline.cshtml")]
    public partial class PreviewOnline : System.Web.Mvc.WebViewPage<AdvertisementElementPreviewModel>
    {
        public PreviewOnline()
        {
        }
        public override void Execute()
        {
WriteLiteral("<!DOCTYPE html>\r\n<html>\r\n<!--font: 1.1em sans-serif Arial; -->\r\n<head>\r\n    <titl" +
"e>");

            
            #line 7 "..\..\Views\Advertisement\PreviewOnline.cshtml"
      Write(BLResources.PreviewAdvertisement);

            
            #line default
            #line hidden
WriteLiteral("</title>\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(">\r\n        body, div, dl, dt, dd, ul, ol, li, h1, h2, h3, h4, h5, h6, pre, code, " +
"form, fieldset, legend, input, textarea, p, blockquote, th, td, address\r\n       " +
" {\r\n            margin: 0;\r\n            padding: 0;\r\n        }\r\n        \r\n      " +
"  li\r\n        {\r\n            list-style: none;\r\n        }\r\n        \r\n        bod" +
"y\r\n        {\r\n            background: #fff;\r\n            color: #222;\r\n         " +
"   font: 13px Arial, sans-serif;\r\n        }\r\n        \r\n        /* Links */\r\n    " +
"    \r\n        a\r\n        {\r\n            color: #0070c0;\r\n        }\r\n        \r\n  " +
"      a:hover\r\n        {\r\n            color: #579a23;\r\n            text-decorati" +
"on: none;\r\n        }\r\n        \r\n        .pseudo\r\n        {\r\n            cursor: " +
"pointer;\r\n            color: #21201e;\r\n            border-bottom: 1px dotted #90" +
"a7b9;\r\n        }\r\n        \r\n        .pseudo, .link-natural\r\n        {\r\n         " +
"   text-decoration: none;\r\n        }\r\n        \r\n        .link-text\r\n        {\r\n " +
"           border-bottom-style: solid;\r\n        }\r\n        \r\n        .pseudo .li" +
"nk-text\r\n        {\r\n            border-bottom-style: dotted;\r\n            border" +
"-bottom-color: #90A7B9;\r\n        }\r\n        \r\n        .pseudo:hover, .link-natur" +
"al:hover\r\n        {\r\n            color: #004baf;\r\n        }\r\n        \r\n        a" +
":hover .link-text, .link-natural, .pseudo:hover\r\n        {\r\n            border-b" +
"ottom-color: transparent;\r\n        }\r\n        \r\n        .link-text\r\n        {\r\n " +
"           border-bottom-width: 1px;\r\n        }\r\n        \r\n        .link-natural" +
" .link-text\r\n        {\r\n            vertical-align: 1px;\r\n        }\r\n        \r\n " +
"       .dg-firm-bullet\r\n        {\r\n            background: url(\'/Content/images/" +
"Erm/Preview/Online/dg_sprite.png?");

            
            #line 85 "..\..\Views\Advertisement\PreviewOnline.cshtml"
                                                                         Write(SolutionInfo.ProductVersion.Build);

            
            #line default
            #line hidden
WriteLiteral("\') no-repeat;\r\n            background-position: 0 -258px;\r\n            position: " +
"absolute;\r\n            left: -12px;\r\n            top: 6px;\r\n            height: " +
"6px;\r\n            width: 6px;\r\n        }\r\n        \r\n        .link-natural .link-" +
"text span, .dg-link-natural .dg-link-text span\r\n        {\r\n            vertical-" +
"align: -1px;\r\n        }\r\n        \r\n        .moz .link-natural .link-text span, ." +
"moz .dg-link-natural .dg-link-text span\r\n        {\r\n            vertical-align: " +
"-2px;\r\n        }\r\n        \r\n        .dg-firm\r\n        {\r\n            position: r" +
"elative;\r\n            margin: 0 10px 20px 15px;\r\n            line-height: 1.35;\r" +
"\n        }\r\n        \r\n        .dg-firm-expanded\r\n        {\r\n            border-c" +
"olor: #c6d2db transparent #c6d2db #8499AA;\r\n            border-style: solid;\r\n  " +
"          border-width: 1px 2px 1px 10px;\r\n            margin: -7px -10px 6px -1" +
"5px;\r\n            padding: 6px 20px;\r\n        }\r\n        \r\n        .dg-firm-expa" +
"nded .dg-firm-bullet\r\n        {\r\n            left: 8px;\r\n            top: 12px;\r" +
"\n        }\r\n        \r\n        .dg-firm-expanded .dg-firm-fas\r\n        {\r\n       " +
"     margin-bottom: -3px;\r\n        }\r\n        \r\n        .dg-firm-head\r\n        {" +
"\r\n            cursor: pointer;\r\n        }\r\n        \r\n        .dg-firm-head:hover" +
" .pseudo .link-text\r\n        {\r\n            border-bottom-color: transparent;\r\n " +
"           color: #004baf;\r\n        }\r\n        \r\n        .dg-firm-title\r\n       " +
" {\r\n            cursor: pointer;\r\n        }\r\n        \r\n        .dg-firm-single ." +
"dg-firm-title .pseudo, .dg-firm-single .dg-firm-title .pseudo:hover, .dg-firm-si" +
"ngle .dg-firm-title .link-text, .dg-firm-single .dg-firm-title .link-text:hover\r" +
"\n        {\r\n            border-bottom-color: transparent;\r\n            color: #3" +
"43434 !important;\r\n            cursor: default;\r\n        }\r\n        \r\n        .d" +
"g-firm-title\r\n        {\r\n            margin: 0 0 3px;\r\n            word-wrap: br" +
"eak-word;\r\n        }\r\n        \r\n        .dg-firm-title, .dg-result-title, .dg-re" +
"sult-title a\r\n        {\r\n            color: #343434;\r\n            font-family: A" +
"rial, sans-serif;\r\n            font-weight: normal;\r\n            font-size: 15px" +
";\r\n            line-height: 19px;\r\n            vertical-align: 1px;\r\n        }\r\n" +
"        \r\n        h2, .dg-result-title\r\n        {\r\n            line-height: 22px" +
";\r\n            color: #222;\r\n            font-size: 17px;\r\n        }\r\n        \r\n" +
"        .dg-firm-micro-comment\r\n        {\r\n            background: #fff8e7 url(\'" +
"/Content/images/Erm/Preview/Online/micro-comment-arrow.png?");

            
            #line 179 "..\..\Views\Advertisement\PreviewOnline.cshtml"
                                                                                           Write(SolutionInfo.ProductVersion.Build);

            
            #line default
            #line hidden
WriteLiteral("\') no-repeat 0 0;\r\n            font-size: 13px;\r\n            -webkit-border-radiu" +
"s: 2px;\r\n            -moz-border-radius: 2px;\r\n            border-radius: 2px;\r\n" +
"            padding: 5px 10px 7px;\r\n            margin: 0 -17px 0 -10px;\r\n      " +
"      cursor: pointer;\r\n            position: relative;\r\n            z-index: 1;" +
"\r\n        }\r\n        \r\n        .dg-firm-box\r\n        {\r\n            margin: 0 0 " +
"8px;\r\n        }\r\n        \r\n        .dg-firm-box .pseudo\r\n        {\r\n            " +
"color: #0070c0;\r\n            border-bottom: 1px dotted;\r\n            cursor: poi" +
"nter;\r\n        }\r\n        \r\n        .dg-firm-box a:hover .pseudo, .dg-firm-box ." +
"pseudo:hover\r\n        {\r\n            color: #004baf;\r\n            border-bottom:" +
" 0;\r\n        }\r\n        \r\n        .dg-firm-row\r\n        {\r\n            margin: 0" +
";\r\n            padding-right: 5px;\r\n            font-size: 12px;\r\n            co" +
"lor: #666;\r\n        }\r\n        \r\n        .dg-firm-ads-text\r\n        {\r\n         " +
"   font-size: 13px;\r\n        }\r\n        \r\n        .dg-firm-fas\r\n        {\r\n     " +
"       color: #888;\r\n            font-size: 11px;\r\n            margin-top: 3px;\r" +
"\n            line-height: 13px;\r\n        }\r\n        \r\n        .dg-firm-expanded " +
".dg-firm-fas\r\n        {\r\n            margin-bottom: -3px;\r\n        }\r\n        \r\n" +
"        .dg-firm-ads .link-row\r\n        {\r\n            line-height: normal;\r\n   " +
"         text-align: right;\r\n        }\r\n        \r\n        .result-view-more\r\n   " +
"     {\r\n            text-align: right;\r\n        }\r\n        \r\n        .catalogue-" +
"item\r\n        {\r\n            padding-top: 1px;\r\n            padding-bottom: 5px;" +
"\r\n            line-height: 15px;\r\n            background: #fff;\r\n            pos" +
"ition: relative;\r\n            z-index: 2;\r\n        }\r\n        \r\n        .catalog" +
"-wrap\r\n        {\r\n            margin-right: 10px;\r\n            width: 340px;\r\n  " +
"      }\r\n        \r\n        .dg-sidebar-content\r\n        {\r\n            padding: " +
"0 0 0 10px;\r\n            width: 360px;\r\n            height: 100% !important;\r\n  " +
"          position: relative;\r\n        }\r\n        \r\n        div.dg-firm-address\r" +
"\n        {\r\n            color: #70747a;\r\n            font-size: 13px;\r\n         " +
"   margin: 0 0 1px;\r\n        }\r\n        \r\n        li.dg-firm-address a\r\n        " +
"{\r\n            font-weight: bold;\r\n            font-family: Arial, sans-serif;\r\n" +
"        }\r\n    </style>\r\n</head>\r\n<body>\r\n    <div");

WriteLiteral(" class=\"dg-sidebar-content\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"dgcontent\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"catalog-wrap\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"firm-list catalogue-item\"");

WriteLiteral(">\r\n                    <div");

WriteLiteral(" class=\"results-list\"");

WriteLiteral(">\r\n                        <!--свернутая карточка-->\r\n                        <di" +
"v");

WriteLiteral(" class=\"dg-firm\"");

WriteLiteral(" id=\"cat-firm_id-141265770892498\"");

WriteLiteral(">\r\n                            <div");

WriteLiteral(" class=\"dg-firm-bullet\"");

WriteLiteral(">\r\n                            </div>\r\n                            <div");

WriteLiteral(" class=\"dg-firm-head\"");

WriteLiteral(">\r\n                                <h2");

WriteLiteral(" class=\"dg-firm-title dg-result-title\"");

WriteLiteral(">\r\n                                    <a");

WriteLiteral(" href=\"javascript:void(0)\"");

WriteLiteral(" class=\"pseudo link-natural\"");

WriteLiteral("><span");

WriteLiteral(" class=\"link-text\"");

WriteLiteral("><span>\r\n");

WriteLiteral("                                        ");

            
            #line 297 "..\..\Views\Advertisement\PreviewOnline.cshtml"
                                   Write(Model.OrgName);

            
            #line default
            #line hidden
WriteLiteral("</span></span></a>\r\n                                </h2>\r\n                      " +
"          <div");

WriteLiteral(" class=\"dg-firm-address\"");

WriteLiteral(">\r\n                                    <span");

WriteLiteral(" style=\"\"");

WriteLiteral(">\r\n");

WriteLiteral("                                        ");

            
            #line 301 "..\..\Views\Advertisement\PreviewOnline.cshtml"
                                   Write(Model.Address);

            
            #line default
            #line hidden
WriteLiteral("</span> <span");

WriteLiteral(" class=\"dg-firm-distance\"");

WriteLiteral(" style=\"display: none;\"");

WriteLiteral(">wtf?\r\n                                        </span>\r\n                         " +
"       </div>\r\n                            </div>\r\n                            <" +
"!--микрокомментарий-->\r\n");

            
            #line 306 "..\..\Views\Advertisement\PreviewOnline.cshtml"
                            
            
            #line default
            #line hidden
            
            #line 306 "..\..\Views\Advertisement\PreviewOnline.cshtml"
                             if (Model.Microcomment != null)
                               { 

            
            #line default
            #line hidden
WriteLiteral("                            <div");

WriteLiteral(" class=\"dg-firm-micro-comment\"");

WriteLiteral(">\r\n                                <div>\r\n");

WriteLiteral("                                    ");

            
            #line 310 "..\..\Views\Advertisement\PreviewOnline.cshtml"
                               Write(Model.Microcomment);

            
            #line default
            #line hidden
WriteLiteral("</div>\r\n                                <div");

WriteLiteral(" class=\"dg-firm-fas\"");

WriteLiteral(">\r\n");

WriteLiteral("                                    ");

            
            #line 312 "..\..\Views\Advertisement\PreviewOnline.cshtml"
                               Write(Model.MicrocommentWarning);

            
            #line default
            #line hidden
WriteLiteral("\r\n                                </div>\r\n                            </div>\r\n");

            
            #line 315 "..\..\Views\Advertisement\PreviewOnline.cshtml"
                            } 

            
            #line default
            #line hidden
WriteLiteral("                        </div>\r\n                        <!--развернутая карточка-" +
"->\r\n                        <div");

WriteLiteral(" class=\"dg-firm dg-firm-expanded\"");

WriteLiteral(" id=\"cat-firm_id-141265770848362\"");

WriteLiteral(">\r\n                            <div");

WriteLiteral(" class=\"dg-firm-bullet\"");

WriteLiteral(">\r\n                            </div>\r\n                            <div");

WriteLiteral(" class=\"dg-firm-head\"");

WriteLiteral(">\r\n                                <h2");

WriteLiteral(" class=\"dg-firm-title dg-result-title\"");

WriteLiteral(">\r\n                                    <a");

WriteLiteral(" href=\"javascript:void(0)\"");

WriteLiteral(" class=\"pseudo link-natural\"");

WriteLiteral("><span");

WriteLiteral(" class=\"link-text\"");

WriteLiteral("><span>\r\n");

WriteLiteral("                                        ");

            
            #line 324 "..\..\Views\Advertisement\PreviewOnline.cshtml"
                                   Write(Model.OrgName);

            
            #line default
            #line hidden
WriteLiteral("\r\n                                    </span></span></a>\r\n                       " +
"         </h2>\r\n                            </div>\r\n                        <div" +
"");

WriteLiteral(" style=\"\"");

WriteLiteral(">\r\n                            <div");

WriteLiteral(" class=\"dg-firm-popup\"");

WriteLiteral(">\r\n                                <!-- ADS BLOCK -->\r\n                          " +
"      <ul");

WriteLiteral(" class=\"dg-firm-box\"");

WriteLiteral(">\r\n                                    <li");

WriteLiteral(" class=\"dg-firm-row\"");

WriteLiteral(">\r\n                                        <div");

WriteLiteral(" class=\"dg-firm-ads\"");

WriteLiteral(" title=\"узнать больше\"");

WriteLiteral(">\r\n                                            <div");

WriteLiteral(" class=\"dg-firm-ads-text\"");

WriteLiteral(">\r\n");

WriteLiteral("                                                ");

            
            #line 335 "..\..\Views\Advertisement\PreviewOnline.cshtml"
                                           Write(Html.Raw(Model.AdComment));

            
            #line default
            #line hidden
WriteLiteral("\r\n                                            </div>\r\n                           " +
"                 <div");

WriteLiteral(" class=\"dg-firm-fas\"");

WriteLiteral(">\r\n");

WriteLiteral("                                                ");

            
            #line 338 "..\..\Views\Advertisement\PreviewOnline.cshtml"
                                           Write(Model.AdCommentWarning);

            
            #line default
            #line hidden
WriteLiteral("\r\n                                            </div>\r\n                           " +
"                 <div");

WriteLiteral(" class=\"link-row\"");

WriteLiteral(">\r\n                                                <span");

WriteLiteral(" class=\"pseudo result-view-more\"");

WriteLiteral(">");

            
            #line 341 "..\..\Views\Advertisement\PreviewOnline.cshtml"
                                                                                 Write(BLResources.PreviewMore);

            
            #line default
            #line hidden
WriteLiteral(@" </span>
                                            </div>
                                        </div>
                                    </li>
                                </ul>
                            </div>
                        </div>
                    </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</body>
</html>

");

        }
    }
}
#pragma warning restore 1591
