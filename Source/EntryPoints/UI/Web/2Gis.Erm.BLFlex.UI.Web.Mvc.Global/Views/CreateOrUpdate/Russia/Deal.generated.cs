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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Russia/Deal.cshtml")]
    public partial class Deal : System.Web.Mvc.WebViewPage<DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.MultiCultureDealViewModel>
    {
        public Deal()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 4 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
  
    Layout = "../../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 209), Tuple.Create("\"", 275)
, Tuple.Create(Tuple.Create("", 216), Tuple.Create("/Content/time-period.css?", 216), true)
            
            #line 10 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
, Tuple.Create(Tuple.Create("", 241), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 241), false)
);

WriteLiteral(" />\r\n    <link");

WriteLiteral(" rel=\"stylesheet\"");

WriteLiteral(" type=\"text/css\"");

WriteAttribute("href", Tuple.Create(" href=\"", 323), Tuple.Create("\"", 391)
, Tuple.Create(Tuple.Create("", 330), Tuple.Create("/Content/input-overlay.css?", 330), true)
            
            #line 11 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
, Tuple.Create(Tuple.Create("", 357), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 357), false)
);

WriteLiteral(" />\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(">\r\n        .wide-labels div.label-wrapper { width: 180px; }\r\n    </style>\r\n\r\n    " +
"<script");

WriteAttribute("src", Tuple.Create(" src=\"", 511), Tuple.Create("\"", 585)
, Tuple.Create(Tuple.Create("", 517), Tuple.Create("/Scripts/Ext.DoubleGis.UI.Deal.js?", 517), true)
            
            #line 16 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
, Tuple.Create(Tuple.Create("", 551), Tuple.Create<System.Object, System.Int32>(SolutionInfo.ProductVersion.Build
            
            #line default
            #line hidden
, 551), false)
);

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral("></script>\r\n");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n");

            
            #line 21 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
    
            
            #line default
            #line hidden
            
            #line 21 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
     if (Model != null)
    {
        
            
            #line default
            #line hidden
            
            #line 23 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
   Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
            
            #line 23 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
                                  
        
            
            #line default
            #line hidden
            
            #line 24 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
   Write(Html.HiddenFor(m => m.ReplicationCode));

            
            #line default
            #line hidden
            
            #line 24 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
                                               

        
            
            #line default
            #line hidden
            
            #line 26 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
                                                    
        
            
            #line default
            #line hidden
            
            #line 27 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
   Write(Html.HiddenFor(m => m.ClientReplicationCode));

            
            #line default
            #line hidden
            
            #line 27 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
                                                     
        
            
            #line default
            #line hidden
            
            #line 28 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
   Write(Html.HiddenFor(m => m.ClientName));

            
            #line default
            #line hidden
            
            #line 28 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
                                          
    }

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 960), Tuple.Create("\"", 996)
            
            #line 30 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
, Tuple.Create(Tuple.Create("", 968), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 968), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 32 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
       Write(Html.TemplateField(m => m.Name, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 35 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
       Write(Html.TemplateField(m => m.Client, FieldFlex.lone, new LookupSettings { EntityName = EntityName.Client, ReadOnly = !Model.IsNew }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 38 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
       Write(Html.TemplateField(m => m.DealStage, FieldFlex.lone, new Dictionary<string, object> { { "disabled", "disabled" } }, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 41 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
       Write(Html.TemplateField(m => m.Comment, FieldFlex.lone, new Dictionary<string, object> { { "rows", "5" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n");

WriteLiteral("        ");

            
            #line 43 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
   Write(Html.SectionHead("section", BLResources.DealFinancialIndicatorsHead));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 45 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
       Write(Html.TemplateField(m => m.Currency, FieldFlex.lone, new LookupSettings { EntityName = EntityName.Currency, ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n");

WriteLiteral("        ");

            
            #line 47 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
   Write(Html.SectionHead("detailsForOrders", BLResources.DetailsForOrders));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 49 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
       Write(Html.TemplateField(m => m.Bargain, FieldFlex.lone, new LookupSettings { EntityName = EntityName.Bargain, ShowReadOnlyCard = true, ExtendedInfo = "dealId={Id}" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"AdditionalTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 2313), Tuple.Create("\"", 2352)
            
            #line 52 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
, Tuple.Create(Tuple.Create("", 2321), Tuple.Create<System.Object, System.Int32>(BLResources.AdditionalTabTitle
            
            #line default
            #line hidden
, 2321), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 54 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
       Write(Html.TemplateField(m => m.MainFirm, FieldFlex.twins, new LookupSettings { EntityName = EntityName.Firm, ExtendedInfo = "filterToParent=true", ParentEntityName = EntityName.Client, ParentIdPattern = "ClientId" }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 57 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
       Write(Html.TemplateField(m => m.StartReason, FieldFlex.lone, new Dictionary<string, object>{{"disabled", "disabled"}}, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n");

WriteLiteral("        ");

            
            #line 59 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
   Write(Html.SectionHead("sectionCloseReason", BLResources.DealCloseInfoHead));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 61 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
       Write(Html.TemplateField(m => m.CloseDate, FieldFlex.twins, new DateTimeSettings { ReadOnly = true }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 64 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
       Write(Html.TemplateField(m => m.CloseReason, FieldFlex.lone, new Dictionary<string, object> { { "disabled", "disabled" } }, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 67 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
       Write(Html.TemplateField(m => m.CloseReasonOther, FieldFlex.lone, new Dictionary<string, object> { { "rows", 3 }, { "disabled", "disabled" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"AdvertisementCampaignTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 3562), Tuple.Create("\"", 3602)
            
            #line 70 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
, Tuple.Create(Tuple.Create("", 3570), Tuple.Create<System.Object, System.Int32>(BLResources.AdvertisingCampaign
            
            #line default
            #line hidden
, 3570), false)
);

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 71 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
   Write(Html.SectionHead("advertisingCampaignDetails", BLResources.AdvertisingCampaign));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"display-wrapper field-wrapper twins\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"label-wrapper\"");

WriteLiteral(">\r\n                    <span>");

            
            #line 75 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
                     Write(BLResources.AdvertisingCampaignPeriod);

            
            #line default
            #line hidden
WriteLiteral(":</span>\r\n                </div>\r\n                <div");

WriteLiteral(" class=\"input-wrapper\"");

WriteLiteral(">\r\n                    <table");

WriteLiteral(" class=\"time-period\"");

WriteLiteral(">\r\n                        <tr>\r\n                            <td");

WriteLiteral(" class=\"time-period-date\"");

WriteLiteral(">");

            
            #line 80 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
                                                    Write(Html.DateFor(m => m.AdvertisingCampaignBeginDate, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                            <td");

WriteLiteral(" class=\"time-period-span\"");

WriteLiteral(">-</td>\r\n                            <td");

WriteLiteral(" class=\"time-period-date\"");

WriteLiteral(">");

            
            #line 82 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
                                                    Write(Html.DateFor(m => m.AdvertisingCampaignEndDate, new DateTimeSettings { ShiftOffset = false }));

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                        </tr>\r\n                    </table>\r\n             " +
"   </div>\r\n            </div>\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 89 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
       Write(Html.TemplateField(m => m.PaymentFormat, FieldFlex.twins, null, EnumResources.ResourceManager));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n            <div");

WriteLiteral(" class=\"display-wrapper field-wrapper twins\"");

WriteLiteral(">\r\n                <div");

WriteLiteral(" class=\"label-wrapper\"");

WriteLiteral(">\r\n                    <span>");

            
            #line 94 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
                     Write(MetadataResources.AgencyFee);

            
            #line default
            #line hidden
WriteLiteral(":</span>\r\n                </div>\r\n                <div");

WriteLiteral(" class=\"input-wrapper input-overlay-parent\"");

WriteLiteral(">\r\n");

WriteLiteral("                    ");

            
            #line 97 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
               Write(Html.TextBoxFor(m => m.AgencyFee, new Dictionary<string, object> { { "class", "inputfields" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("                    ");

            
            #line 98 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
               Write(Html.ValidationMessageFor(m => m.AgencyFee, null, new Dictionary<string, object> { { "class", "error" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    <div");

WriteLiteral(" class=\"input-overlay\"");

WriteLiteral(">%</div>\r\n                </div>\r\n            </div>\r\n        </div>\r\n");

WriteLiteral("        ");

            
            #line 103 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
   Write(Html.SectionHead("advertisingCampaignGoals", BLResources.AdvertisingCampaignGoals));

            
            #line default
            #line hidden
WriteLiteral("\r\n        <div");

WriteLiteral(" class=\"row-wrapper wide-labels\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 105 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
       Write(Html.TemplateField(m => m.IncreaseSalesGoal, FieldFlex.quadruplet));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 106 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
       Write(Html.TemplateField(m => m.AttractAudienceToSiteGoal, FieldFlex.quadruplet));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 107 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
       Write(Html.TemplateField(m => m.IncreasePhoneCallsGoal, FieldFlex.quadruplet));

            
            #line default
            #line hidden
WriteLiteral("\r\n");

WriteLiteral("            ");

            
            #line 108 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
       Write(Html.TemplateField(m => m.IncreaseBrandAwarenessGoal, FieldFlex.quadruplet));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 111 "..\..\Views\CreateOrUpdate\Russia\Deal.cshtml"
       Write(Html.TemplateField(m => m.AdvertisingCampaignGoalText, FieldFlex.lone, new Dictionary<string, object> { { "rows", 5 } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
