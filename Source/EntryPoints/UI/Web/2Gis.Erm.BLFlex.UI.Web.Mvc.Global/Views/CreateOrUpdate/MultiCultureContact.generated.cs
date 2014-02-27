﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Views.CreateOrUpdate
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc.Html;
#line 1 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
    using BLCore.UI.Web.Mvc.Utils;
    
    #line default
    #line hidden
    using DoubleGis.Erm.BLCore.Resources.Server.Properties;
    using DoubleGis.Erm.Platform.Common;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    
    #line 2 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
    using Newtonsoft.Json;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/MultiCultureContact.cshtml")]
    public partial class MultiCultureContact : System.Web.Mvc.WebViewPage<Models.MultiCultureContactViewModel>
    {
        public MultiCultureContact()
        {
        }
        public override void Execute()
        {
            
            #line 5 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteAttribute("src", Tuple.Create(" src=\"", 193), Tuple.Create("\"", 270)
, Tuple.Create(Tuple.Create("", 199), Tuple.Create("/Scripts/Ext.DoubleGis.UI.Contact.js?", 199), true)
            
            #line 11 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
, Tuple.Create(Tuple.Create("", 236), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 236), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 16 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 17 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
Write(Html.HiddenFor(m => m.ReplicationCode));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 18 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
Write(Html.HiddenFor(m => m.FullName));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n    ");

WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 21 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
Write(Html.HiddenFor(m => m.BusinessModelArea));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 22 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
Write(Html.Hidden("Salutations", JsonConvert.SerializeObject(Model.AvailableSalutations)));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 23 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
Write(Html.Hidden("InitialSalutation", Model.Salutation));

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 708), Tuple.Create("\"", 744)
            
            #line 25 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
, Tuple.Create(Tuple.Create("", 716), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 716), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 27 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
       Write(Html.TemplateField(m => m.Gender, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 28 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
       Write(Html.TemplateField(m => m.MainPhoneNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 31 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
       Write(Html.TemplateField(m => m.Salutation, FieldFlex.twins, new Dictionary<string, object>{{"combobox", "true"}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 32 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
       Write(Html.TemplateField(m => m.MobilePhoneNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 35 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
       Write(Html.TemplateField(m => m.LastName, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 36 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
       Write(Html.TemplateField(m => m.AdditionalPhoneNumber, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 39 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
       Write(Html.TemplateField(m => m.FirstName, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 40 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
       Write(Html.TemplateField(m => m.Fax, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 43 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
       Write(Html.TemplateField(m => m.WorkEmail, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 44 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
       Write(Html.TemplateField(m => m.Im, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 47 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
       Write(Html.TemplateField(m => m.Client, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Client, ReadOnly =  Model != null && Model.Client != null && !string.IsNullOrEmpty(Model.Client.Value) }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n");

WriteLiteral("        ");

            
            #line 49 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
   Write(Html.SectionHead("SectionHead1", @BLResources.TitleServiceInformation));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 51 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
       Write(Html.TemplateField(m => m.JobTitle, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 52 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
       Write(Html.TemplateField(m => m.AccountRole, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 55 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
       Write(Html.TemplateField(m => m.Department, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 56 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
       Write(Html.TemplateField(m => m.IsFired, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 59 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
       Write(Html.TemplateField(m => m.WorkAddress, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 62 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
       Write(Html.TemplateField(m => m.Comment, FieldFlex.lone, new Dictionary<string, object>{{"rows", "3"}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n");

WriteLiteral("        ");

            
            #line 64 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
   Write(Html.SectionHead("SectionHead2", @BLResources.AdditionalTabTitle));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 66 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
       Write(Html.TemplateField(m => m.BirthDate, FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 67 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
       Write(Html.TemplateField(m => m.Website , FieldFlex.twins));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

WriteLiteral("\r\n");

            
            #line 72 "..\..\Views\CreateOrUpdate\MultiCultureContact.cshtml"
Write(RenderBody());

            
            #line default
            #line hidden
        }
    }
}
#pragma warning restore 1591
