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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Letter.cshtml")]
    public partial class Letter : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BL.UI.Web.Mvc.Models.Activity.LetterViewModel>
    {
        public Letter()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\CreateOrUpdate\Letter.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 161), Tuple.Create("\"", 218)
, Tuple.Create(Tuple.Create("", 167), Tuple.Create("/Scripts/Ext.ux.TimeComboBox.js?", 167), true)
            
            #line 9 "..\..\Views\CreateOrUpdate\Letter.cshtml"
, Tuple.Create(Tuple.Create("", 199), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 199), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 265), Tuple.Create("\"", 354)
, Tuple.Create(Tuple.Create("", 271), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.RegardingObjectController.js?", 271), true)
            
            #line 10 "..\..\Views\CreateOrUpdate\Letter.cshtml"
  , Tuple.Create(Tuple.Create("", 335), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 335), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 401), Tuple.Create("\"", 490)
, Tuple.Create(Tuple.Create("", 407), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.ContactRelationController.js?", 407), true)
            
            #line 11 "..\..\Views\CreateOrUpdate\Letter.cshtml"
  , Tuple.Create(Tuple.Create("", 471), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 471), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 537), Tuple.Create("\"", 613)
, Tuple.Create(Tuple.Create("", 543), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.ActivityBase.js?", 543), true)
            
            #line 12 "..\..\Views\CreateOrUpdate\Letter.cshtml"
, Tuple.Create(Tuple.Create("", 594), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 594), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 660), Tuple.Create("\"", 730)
, Tuple.Create(Tuple.Create("", 666), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.Letter.js?", 666), true)
            
            #line 13 "..\..\Views\CreateOrUpdate\Letter.cshtml"
, Tuple.Create(Tuple.Create("", 711), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 711), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CustomInit", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.onReady(function()\r\n        {\r\n            var cardSettings = ");

            
            #line 21 "..\..\Views\CreateOrUpdate\Letter.cshtml"
                          Write(Html.WriteJson(Model.ViewConfig.CardSettings));

            
            #line default
            #line hidden
WriteLiteral(";\r\n            Ext.apply(cardSettings, { contactField: \"RecipientId\", contactComp" +
"onent: \"Recipient\" });\r\n            window.Card = new window.Ext.DoubleGis.UI.Le" +
"tter(cardSettings);\r\n            window.Card.Build();\r\n        });\r\n    </script" +
">\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 1256), Tuple.Create("\"", 1292)
            
            #line 31 "..\..\Views\CreateOrUpdate\Letter.cshtml"
, Tuple.Create(Tuple.Create("", 1264), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 1264), false)
);

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 32 "..\..\Views\CreateOrUpdate\Letter.cshtml"
   Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 33 "..\..\Views\CreateOrUpdate\Letter.cshtml"
   Write(Html.HiddenFor(m => m.Status));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 35 "..\..\Views\CreateOrUpdate\Letter.cshtml"
   Write(Html.SectionHead("regardingObjectHeader", BLResources.TitleRegarding));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 36 "..\..\Views\CreateOrUpdate\Letter.cshtml"
   Write(Html.SectionRow(
            @Html.TemplateField(m => m.Client, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Client }),
            @Html.TemplateField(m => m.Firm, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Firm, ExtendedInfo = "ForClientAndItsDescendants=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId" })));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 39 "..\..\Views\CreateOrUpdate\Letter.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Deal, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Deal, ExtendedInfo = "ForClientAndItsDescendants=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId" })));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 41 "..\..\Views\CreateOrUpdate\Letter.cshtml"
   Write(Html.SectionHead("planHeader", BLResources.TitlePlan));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 42 "..\..\Views\CreateOrUpdate\Letter.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Title, FieldFlex.lone)));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 43 "..\..\Views\CreateOrUpdate\Letter.cshtml"
   Write(Html.SectionRow(
            @Html.TemplateField(m => m.ScheduledStart, FieldFlex.twins,
                new CalendarSettings { Store = CalendarSettings.StoreMode.Absolute, Time = new CalendarSettings.TimeSettings() }),
            @Html.TemplateField(m => m.Priority, FieldFlex.twins, null, EnumResources.ResourceManager)));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 47 "..\..\Views\CreateOrUpdate\Letter.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Sender, FieldFlex.lone, new LookupSettings { EntityName = EntityName.User })));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 48 "..\..\Views\CreateOrUpdate\Letter.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Recipient, FieldFlex.lone, new LookupSettings { EntityName = EntityName.Contact, ExtendedInfo = "ForClientAndItsDescendants=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId" })));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 50 "..\..\Views\CreateOrUpdate\Letter.cshtml"
   Write(Html.SectionHead("resultHeader", BLResources.TitleResult));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 51 "..\..\Views\CreateOrUpdate\Letter.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Description, FieldFlex.lone, new Dictionary<string, object> { { "rows", "10" } })));

            
            #line default
            #line hidden
WriteLiteral("\r\n   </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
