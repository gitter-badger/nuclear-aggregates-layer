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

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Views.CreateOrUpdate.Russia
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Russia/Client.cshtml")]
    public partial class Client : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia.ClientViewModel>
    {
        public Client()
        {
        }
        public override void Execute()
        {
            
            #line 2 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
  
    Layout = "../../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 171), Tuple.Create("\"", 247)
, Tuple.Create(Tuple.Create("", 177), Tuple.Create("/Scripts/Ext.DoubleGis.UI.Client.js?", 177), true)
            
            #line 8 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
, Tuple.Create(Tuple.Create("", 213), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 213), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 294), Tuple.Create("\"", 384)
, Tuple.Create(Tuple.Create("", 300), Tuple.Create("/Scripts/Russia/Ext.DoubleGis.UI.Client.Russia.js?", 300), true)
            
            #line 9 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
, Tuple.Create(Tuple.Create("", 350), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 350), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 14 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 15 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
Write(Html.HiddenFor(m => m.ReplicationCode));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 16 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
Write(Html.HiddenFor(m => m.CanEditIsAdvertisingAgency));

            
            #line default
            #line hidden
WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 613), Tuple.Create("\"", 649)
            
            #line 17 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
, Tuple.Create(Tuple.Create("", 621), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 621), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 19 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
       Write(Html.TemplateField(m => m.Name, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 22 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
       Write(Html.TemplateField(m => m.MainAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 25 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
       Write(Html.TemplateField(m => m.MainPhoneNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 26 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
       Write(Html.TemplateField(m => m.Fax, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 29 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
       Write(Html.TemplateField(m => m.AdditionalPhoneNumber1, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 30 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
       Write(Html.TemplateField(m => m.Email, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 33 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
       Write(Html.TemplateField(m => m.AdditionalPhoneNumber2, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 34 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
       Write(Html.TemplateField(m => m.Website, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 37 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
       Write(Html.TemplateField(m => m.IsAdvertisingAgency, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 40 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
       Write(Html.TemplateField(m => m.Comment, FieldFlex.lone, new Dictionary<string, object> { { "rows", 5 } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n");

WriteLiteral("        ");

            
            #line 42 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
   Write(Html.SectionHead("SectionHead2", BLResources.AdditionalTabTitle));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 44 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
       Write(Html.TemplateField(m => m.InformationSource, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 45 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
       Write(Html.TemplateField(m => m.LastQualifyTime, FieldFlex.twins, new DateTimeSettings { ReadOnly = true, ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 48 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
       Write(Html.TemplateField(m => m.PromisingScore, FieldFlex.twins, new Dictionary<string, object> { { "readonly", "readonly" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 49 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
       Write(Html.TemplateField(m => m.LastDisqualifyTime, FieldFlex.twins, new DateTimeSettings { ReadOnly = true, ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 52 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
       Write(Html.TemplateField(m => m.MainFirm, FieldFlex.lone, new LookupSettings { EntityName = EntityName.Firm, ExtendedInfo = "filterToParent=true", ParentEntityName = EntityName.Client, ParentIdPattern = "Id" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"AdministrationTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 2789), Tuple.Create("\"", 2832)
            
            #line 55 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
, Tuple.Create(Tuple.Create("", 2797), Tuple.Create<System.Object, System.Int32>(BLResources.AdministrationTabTitle
            
            #line default
            #line hidden
, 2797), false)
);

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 56 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
   Write(Html.SectionHead("adminHeader", BLResources.AdministrationTabTitle));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

            
            #line 57 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
        
            
            #line default
            #line hidden
            
            #line 57 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
         if (Model.IsCurated && Model.IsSecurityRoot)
        {

            
            #line default
            #line hidden
WriteLiteral("            <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("                ");

            
            #line 60 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
           Write(Html.TemplateField(m => m.Owner, FieldFlex.twins, new LookupSettings { EntityName = EntityName.User, Plugins = new[] { "new Ext.ux.LookupFieldOwner()" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                ");

            
            #line 61 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
           Write(Html.TemplateField(m => m.Territory, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Territory, ReadOnly = Model != null && Model.Territory != null && !string.IsNullOrEmpty(Model.Territory.Value) }));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </div>\r\n");

            
            #line 63 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 65 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
       Write(Html.TemplateField(m => m.CreatedBy, FieldFlex.twins, new LookupSettings { EntityName = EntityName.User, ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 66 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
       Write(Html.TemplateField(m => m.CreatedOn, FieldFlex.twins, new DateTimeSettings { ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 69 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
       Write(Html.TemplateField(m => m.ModifiedBy, FieldFlex.twins, new LookupSettings { EntityName = EntityName.User, ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 70 "..\..\Views\CreateOrUpdate\Russia\Client.cshtml"
       Write(Html.TemplateField(m => m.ModifiedOn, FieldFlex.twins, new DateTimeSettings { ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
