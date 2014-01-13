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

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Views.Client
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Client/AllClientsDebtReport.cshtml")]
    public partial class AllClientsDebtReport : System.Web.Mvc.WebViewPage<AllClientsDebtReportViewModel>
    {
        public AllClientsDebtReport()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 4 "..\..\Views\Client\AllClientsDebtReport.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 8 "..\..\Views\Client\AllClientsDebtReport.cshtml"
            Write(BLResources.AllClientsDebtReport);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 9 "..\..\Views\Client\AllClientsDebtReport.cshtml"
                  Write(BLResources.ReportParametersAccepting);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 10 "..\..\Views\Client\AllClientsDebtReport.cshtml"
                    Write(BLResources.ReportParametersAcceptingLegend);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

WriteLiteral("\r\n");

DefineSection("PageContent", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">
        var submitForm = function ()
        {
            if (Ext.DoubleGis.FormValidator.validate(window.EntityForm))
            {
                window.close();
            }
        };
        var setVisualFeatures = function ()
        {
            var divRows = window.Ext.query(""div.field-wrapper"");
            var i;
            for (i = 0; i < divRows.length; i++)
            {
                window.Ext.fly(divRows[i]).addClassOnOver(""field-wrapper-over"");
            }

            var inputs = window.Ext.query("".inputfields"");
            for (i = 0; i < inputs.length; i++)
            {
                window.Ext.fly(inputs[i]).addClassOnFocus(""inputfields-selected"");
            }
        };
        Ext.onReady(function ()
        {

            window.Ext.each(window.Ext.CardLookupSettings, function (item)
            {
                new window.Ext.ux.LookupField(item);
            }, this);
            setVisualFeatures();
            window.Ext.get(""Cancel"").on(""click"", function () { window.close(); });
            window.Ext.get(""OK"").on(""click"", submitForm);
        });
        
    </script>
");

            
            #line 50 "..\..\Views\Client\AllClientsDebtReport.cshtml"
    
            
            #line default
            #line hidden
            
            #line 50 "..\..\Views\Client\AllClientsDebtReport.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 53 "..\..\Views\Client\AllClientsDebtReport.cshtml"
   Write(Html.TemplateField(m=>m.Owner, FieldFlex.lone, new LookupSettings{EntityName = EntityName.User}));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n");

WriteLiteral("    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 56 "..\..\Views\Client\AllClientsDebtReport.cshtml"
   Write(Html.TemplateField(m => m.OrganizationUnit, FieldFlex.lone, new LookupSettings { EntityName = EntityName.OrganizationUnit }));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n");

            
            #line 58 "..\..\Views\Client\AllClientsDebtReport.cshtml"
    
            
            #line default
            #line hidden
            
            #line 58 "..\..\Views\Client\AllClientsDebtReport.cshtml"
Write(Html.SectionHead("debtType", BLResources.DebtTypes));

            
            #line default
            #line hidden
            
            #line 58 "..\..\Views\Client\AllClientsDebtReport.cshtml"
                                                        

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 60 "..\..\Views\Client\AllClientsDebtReport.cshtml"
   Write(Html.TemplateField(m=>m.WithPaymentDebt, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n");

WriteLiteral("    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 63 "..\..\Views\Client\AllClientsDebtReport.cshtml"
   Write(Html.TemplateField(m=>m.WithDocDebtOrder, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n");

WriteLiteral("    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 66 "..\..\Views\Client\AllClientsDebtReport.cshtml"
   Write(Html.TemplateField(m=>m.WithDocDebtBargain, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n");

WriteLiteral("    <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("        ");

            
            #line 69 "..\..\Views\Client\AllClientsDebtReport.cshtml"
   Write(Html.TemplateField(m=>m.WithDocDebtTermination, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n");

            
            #line 71 "..\..\Views\Client\AllClientsDebtReport.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
