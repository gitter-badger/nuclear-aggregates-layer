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
    
    #line 1 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
    using System.ComponentModel;
    
    #line default
    #line hidden
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Phonecall.cshtml")]
    public partial class Phonecall : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BL.UI.Web.Mvc.Models.Activity.PhonecallViewModel>
    {
        public Phonecall()
        {
        }
        public override void Execute()
        {
            
            #line 4 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 194), Tuple.Create("\"", 251)
, Tuple.Create(Tuple.Create("", 200), Tuple.Create("/Scripts/Ext.ux.TimeComboBox.js?", 200), true)
            
            #line 10 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
, Tuple.Create(Tuple.Create("", 232), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 232), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 298), Tuple.Create("\"", 387)
, Tuple.Create(Tuple.Create("", 304), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.RegardingObjectController.js?", 304), true)
            
            #line 11 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
  , Tuple.Create(Tuple.Create("", 368), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 368), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 434), Tuple.Create("\"", 523)
, Tuple.Create(Tuple.Create("", 440), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.ContactRelationController.js?", 440), true)
            
            #line 12 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
  , Tuple.Create(Tuple.Create("", 504), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 504), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 570), Tuple.Create("\"", 646)
, Tuple.Create(Tuple.Create("", 576), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.ActivityBase.js?", 576), true)
            
            #line 13 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
, Tuple.Create(Tuple.Create("", 627), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 627), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 693), Tuple.Create("\"", 766)
, Tuple.Create(Tuple.Create("", 699), Tuple.Create("/Scripts/Activity/Ext.DoubleGis.UI.Phonecall.js?", 699), true)
            
            #line 14 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
, Tuple.Create(Tuple.Create("", 747), Tuple.Create<System.Object, System.Int32>(ThisAssembly.Build
            
            #line default
            #line hidden
, 747), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CustomInit", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.onReady(function() {\r\n            var cardSettings = ");

            
            #line 21 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
                          Write(Html.WriteJson(Model.ViewConfig.CardSettings));

            
            #line default
            #line hidden
WriteLiteral(";\r\n            Ext.apply(cardSettings, { contactField: \"ContactId\", contactCompon" +
"ent: \"Contact\" });\r\n            window.Card = new window.Ext.DoubleGis.UI.Phonec" +
"all(cardSettings);\r\n            window.Card.Build();\r\n        });\r\n    </script>" +
"\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 1282), Tuple.Create("\"", 1318)
            
            #line 31 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
, Tuple.Create(Tuple.Create("", 1290), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 1290), false)
);

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 32 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 33 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.HiddenFor(m => m.Status));

            
            #line default
            #line hidden
WriteLiteral("\r\n        \r\n");

            
            #line 35 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
        
            
            #line default
            #line hidden
            
            #line 35 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
          
            var isOwnerAndNameSortFields = new[] { new LookupSortInfo { Direction = ListSortDirection.Descending, Field = "IsOwner" }, new LookupSortInfo { Direction = ListSortDirection.Ascending, Field = "Name" } };
            var isOwnerAndFullNameSortFields = new[] { new LookupSortInfo { Direction = ListSortDirection.Descending, Field = "IsOwner" }, new LookupSortInfo { Direction = ListSortDirection.Ascending, Field = "FullName" } };
        
            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 40 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.SectionHead("regardingObjectHeader", BLResources.TitleRegarding));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 41 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.SectionRow(
            @Html.TemplateField(m => m.Client, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Client, DefaultSortFields = isOwnerAndNameSortFields }),
            @Html.TemplateField(m => m.Firm, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Firm, ExtendedInfo = "filterToParent=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId", DefaultSortFields = isOwnerAndNameSortFields })));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 44 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Deal, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Deal, ExtendedInfo = "filterToParent=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId", DefaultSortFields = isOwnerAndNameSortFields })));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 46 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.SectionHead("planHeader", BLResources.TitlePlan));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 47 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Purpose, FieldFlex.lone, null, EnumResources.ResourceManager)));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 48 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Title, FieldFlex.lone)));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 49 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.SectionRow(
            @Html.TemplateField(m => m.ScheduledStart, FieldFlex.twins,
                new CalendarSettings { Store = CalendarSettings.StoreMode.Absolute, Time = new CalendarSettings.TimeSettings() }),
            @Html.TemplateField(m => m.Priority, FieldFlex.twins, null, EnumResources.ResourceManager)
            ));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 54 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Contact, FieldFlex.lone, new LookupSettings { EntityName = EntityName.Contact, ExtendedInfo = "filterToParent=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId", DefaultSortFields = isOwnerAndFullNameSortFields })));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

WriteLiteral("        ");

            
            #line 56 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.SectionHead("resultHeader", BLResources.TitleResult));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("        ");

            
            #line 57 "..\..\Views\CreateOrUpdate\Phonecall.cshtml"
   Write(Html.SectionRow(@Html.TemplateField(m => m.Description, FieldFlex.lone, new Dictionary<string, object> { { "rows", "10" } })));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
