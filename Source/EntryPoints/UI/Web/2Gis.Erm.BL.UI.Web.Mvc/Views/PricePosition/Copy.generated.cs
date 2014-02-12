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

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Views.PricePosition
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/PricePosition/Copy.cshtml")]
    public partial class Copy : System.Web.Mvc.WebViewPage<Models.CopyPricePositionModel>
    {
        public Copy()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 4 "..\..\Views\PricePosition\Copy.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 8 "..\..\Views\PricePosition\Copy.cshtml"
            Write(BLResources.CopyPricePosition);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 9 "..\..\Views\PricePosition\Copy.cshtml"
                  Write(BLResources.CopyPricePosition);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 10 "..\..\Views\PricePosition\Copy.cshtml"
                    Write(BLResources.SpecifyPositionForNewPricePosition);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

WriteLiteral("\r\n");

DefineSection("PageContent", () => {

WriteLiteral("\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(">\r\n        td.itemCaption\r\n        {\r\n            vertical-align: top;\r\n         " +
"   padding-top: 5px;\r\n        }\r\n\r\n        td.itemValue\r\n        {\r\n            " +
"vertical-align: top;\r\n        }\r\n    </style>\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">
        Ext.onReady(function() {
            //Show error messages
            if (Ext.getDom(""Notifications"").innerHTML.trim() == ""OK"") {
                window.close();
                return;
            } else if (Ext.getDom(""Notifications"").innerHTML.trim() != """") {
                Ext.get(""Notifications"").addClass(""Notifications"");
            }
            //write eventhandlers for buttons
            Ext.get(""Cancel"").on(""click"", function() { window.close(); });
            Ext.get(""OK"").on(""click"", function() {
                if (!Ext.DoubleGis.FormValidator.validate(window.EntityForm)) {
                    return;
                }

                Ext.getDom(""OK"").disabled = ""disabled"";
                Ext.getDom(""Cancel"").disabled = ""disabled"";

                EntityForm.submit();
            });


            window.Ext.each(window.Ext.CardLookupSettings, function(item) {
                new window.Ext.ux.LookupField(item);
            }, this);

        });
    </script>
");

            
            #line 55 "..\..\Views\PricePosition\Copy.cshtml"
    
            
            #line default
            #line hidden
            
            #line 55 "..\..\Views\PricePosition\Copy.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {

            
            #line default
            #line hidden
WriteLiteral("        <div");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 58 "..\..\Views\PricePosition\Copy.cshtml"
       Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n");

            
            #line 60 "..\..\Views\PricePosition\Copy.cshtml"
        
            
            #line default
            #line hidden
            
            #line 60 "..\..\Views\PricePosition\Copy.cshtml"
   Write(Html.HiddenFor(m => m.PriceId));

            
            #line default
            #line hidden
            
            #line 60 "..\..\Views\PricePosition\Copy.cshtml"
                                       
        
            
            #line default
            #line hidden
            
            #line 61 "..\..\Views\PricePosition\Copy.cshtml"
   Write(Html.HiddenFor(m => m.SourcePricePositionId));

            
            #line default
            #line hidden
            
            #line 61 "..\..\Views\PricePosition\Copy.cshtml"
                                                     

            
            #line default
            #line hidden
WriteLiteral("        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 63 "..\..\Views\PricePosition\Copy.cshtml"
       Write(Html.TemplateField(m => m.Position, FieldFlex.lone, new LookupSettings { EntityName = EntityName.Position }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n");

            
            #line 65 "..\..\Views\PricePosition\Copy.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
