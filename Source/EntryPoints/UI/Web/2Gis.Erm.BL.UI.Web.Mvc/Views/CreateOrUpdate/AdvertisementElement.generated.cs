﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34011
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Views.CreateOrUpdate
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
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/AdvertisementElement.cshtml")]
    public partial class AdvertisementElement : System.Web.Mvc.WebViewPage<AdvertisementElementViewModel>
    {
        public AdvertisementElement()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(@">
        #TxtContainer TABLE TD
        {
            padding-bottom: 0px !important;
            padding-left: 0px !important;
            padding-right: 0px !important;
        }
        DIV.row-wrapper TABLE TD
        {
            padding-left: 5px;
        }
        TD.span-wrapper
        {
            padding-left: 10px !important;
            padding-right: 10px;
        }
        DIV.Tab TABLE
        {
            table-layout: auto;
            border-spacing: inherit;
            width: auto;
        }

        DIV.Tab TABLE TD
        {
            padding-bottom: inherit;
        }
    </style>
    
");

            
            #line 38 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
    
            
            #line default
            #line hidden
            
            #line 38 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
     if (HttpContext.Current.IsDebuggingEnabled)
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 870), Tuple.Create("\"", 943)
, Tuple.Create(Tuple.Create("", 876), Tuple.Create("/Scripts/TinyMCE/tiny_mce_src.js?", 876), true)
            
            #line 40 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
, Tuple.Create(Tuple.Create("", 909), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 909), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 994), Tuple.Create("\"", 1076)
, Tuple.Create(Tuple.Create("", 1000), Tuple.Create("/Scripts/Ext.ux.TinyMCE/Ext.ux.TinyMCE.js?", 1000), true)
            
            #line 41 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
, Tuple.Create(Tuple.Create("", 1042), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1042), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 42 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
    }
    else
    {

            
            #line default
            #line hidden
WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1151), Tuple.Create("\"", 1220)
, Tuple.Create(Tuple.Create("", 1157), Tuple.Create("/Scripts/TinyMCE/tiny_mce.js?", 1157), true)
            
            #line 45 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
, Tuple.Create(Tuple.Create("", 1186), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1186), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

WriteLiteral("        <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1271), Tuple.Create("\"", 1357)
, Tuple.Create(Tuple.Create("", 1277), Tuple.Create("/Scripts/Ext.ux.TinyMCE/Ext.ux.TinyMCE.min.js?", 1277), true)
            
            #line 46 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
, Tuple.Create(Tuple.Create("", 1323), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1323), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

            
            #line 47 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
    }

            
            #line default
            #line hidden
WriteLiteral("    \r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 1417), Tuple.Create("\"", 1507)
, Tuple.Create(Tuple.Create("", 1423), Tuple.Create("/Scripts/Ext.DoubleGis.UI.AdvertisementElement.js?", 1423), true)
            
            #line 49 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
, Tuple.Create(Tuple.Create("", 1473), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 1473), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 1603), Tuple.Create("\"", 1639)
            
            #line 54 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
, Tuple.Create(Tuple.Create("", 1611), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 1611), false)
);

WriteLiteral(">\r\n");

            
            #line 55 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
        
            
            #line default
            #line hidden
            
            #line 55 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
         if (Model != null)
        {
            
            
            #line default
            #line hidden
            
            #line 57 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
       Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
            
            #line 57 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
                                      
            
            
            #line default
            #line hidden
            
            #line 58 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
       Write(Html.HiddenFor(m => m.CanUserChangeStatus));

            
            #line default
            #line hidden
            
            #line 58 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
                                                       
            
            
            #line default
            #line hidden
            
            #line 59 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
       Write(Html.HiddenFor(m => m.NeedsValidation));

            
            #line default
            #line hidden
            
            #line 59 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
                                                   

            
            
            #line default
            #line hidden
            
            #line 61 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
                                                                                                                    
            
            
            #line default
            #line hidden
            
            #line 62 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
       Write(Html.HiddenFor(m => m.ActualType));

            
            #line default
            #line hidden
            
            #line 62 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
                                              
        }

            
            #line default
            #line hidden
WriteLiteral("        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 65 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
       Write(Html.TemplateField(m => m.AdvertisementElementTemplate, FieldFlex.lone, new LookupSettings { EntityName = EntityName.AdvertisementElementTemplate, ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    \r\n");

            
            #line 68 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
        
            
            #line default
            #line hidden
            
            #line 68 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
         if (Model.NeedsValidation)
        {

            
            #line default
            #line hidden
WriteLiteral("            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 71 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
           Write(Html.TemplateField(m => m.Status, FieldFlex.lone, new Dictionary<string, object> { { "disabled", "disabled" } }, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n");

            
            #line 73 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 75 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
        
            
            #line default
            #line hidden
            
            #line 75 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
         switch (Model.ActualType)
        {
            case AdvertisementElementRestrictionActualType.File:
                
            
            #line default
            #line hidden
            
            #line 78 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
           Write(Html.EditorFor(m => m.File));

            
            #line default
            #line hidden
            
            #line 78 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
                                            
                break;
            case AdvertisementElementRestrictionActualType.Date:
            
            
            #line default
            #line hidden
            
            #line 81 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
       Write(Html.EditorFor(m => m.Period));

            
            #line default
            #line hidden
            
            #line 81 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
                                        
                break;
            case AdvertisementElementRestrictionActualType.FasComment:
            
            
            #line default
            #line hidden
            
            #line 84 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
       Write(Html.EditorFor(m => m.FasComment));

            
            #line default
            #line hidden
            
            #line 84 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
                                              
                break;
            case AdvertisementElementRestrictionActualType.FormattedText:
            
            
            #line default
            #line hidden
            
            #line 87 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
       Write(Html.EditorFor(m => m.FormattedText));

            
            #line default
            #line hidden
            
            #line 87 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
                                                 
                break;
            case AdvertisementElementRestrictionActualType.Image:
            
            
            #line default
            #line hidden
            
            #line 90 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
       Write(Html.EditorFor(m => m.Image));

            
            #line default
            #line hidden
            
            #line 90 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
                                         
                break;
            case AdvertisementElementRestrictionActualType.Link:
            
            
            #line default
            #line hidden
            
            #line 93 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
       Write(Html.EditorFor(m => m.Link));

            
            #line default
            #line hidden
            
            #line 93 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
                                        
                break;
            case AdvertisementElementRestrictionActualType.PlainText:
            
            
            #line default
            #line hidden
            
            #line 96 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
       Write(Html.EditorFor(m => m.PlainText));

            
            #line default
            #line hidden
            
            #line 96 "..\..\Views\CreateOrUpdate\AdvertisementElement.cshtml"
                                             
                break;
        }

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
