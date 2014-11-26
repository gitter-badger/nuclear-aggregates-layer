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

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Views.CreateOrUpdate.Cyprus
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Cyprus/LegalPerson.cshtml")]
    public partial class LegalPerson : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Cyprus.CyprusLegalPersonViewModel>
    {
        public LegalPerson()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
  
    Layout = "../../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 184), Tuple.Create("\"", 250)
, Tuple.Create(Tuple.Create("", 190), Tuple.Create("/Scripts/Ext.DoubleGis.UI.LegalPerson.js?", 190), true)
            
            #line 9 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
, Tuple.Create(Tuple.Create("", 231), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 231), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 297), Tuple.Create("\"", 377)
, Tuple.Create(Tuple.Create("", 303), Tuple.Create("/Scripts/Cyprus/Ext.DoubleGis.UI.LegalPerson.Cyprus.js?", 303), true)
            
            #line 10 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
, Tuple.Create(Tuple.Create("", 358), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 358), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(">\r\n        div.label-wrapper {\r\n            width: 150px !important;\r\n        }\r\n" +
"    </style>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 21 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 22 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
Write(Html.HiddenFor(m => m.HasProfiles));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 671), Tuple.Create("\"", 707)
            
            #line 24 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
, Tuple.Create(Tuple.Create("", 679), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 679), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 26 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.LegalName, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 27 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.LegalPersonType, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 30 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.Client, FieldFlex.lone, new LookupSettings { EntityName = EntityName.Client, ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 33 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.Inn, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 34 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.BusinessmanInn, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 35 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.VAT, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 38 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.PassportNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 39 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.PassportIssuedBy, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 42 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.CardNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 43 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.RegistrationAddress, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 46 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.LegalAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 49 "..\..\Views\CreateOrUpdate\Cyprus\LegalPerson.cshtml"
       Write(Html.TemplateField(m => m.Comment, FieldFlex.lone, new Dictionary<string, object> { { "rows", 3 } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
