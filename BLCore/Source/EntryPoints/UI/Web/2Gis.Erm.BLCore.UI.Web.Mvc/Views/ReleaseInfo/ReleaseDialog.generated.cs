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

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Views.ReleaseInfo
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
    using DoubleGis.Erm.BLCore.UI.Metadata.Confirmations;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Controllers;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models.GroupOperation;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Settings.ConfigurationDto;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.UserProfiles;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
    using DoubleGis.Erm.Platform.Common;
    using DoubleGis.Erm.Platform.Model.Entities;
    using DoubleGis.Erm.Platform.Model.Entities.Enums;
    using DoubleGis.Erm.Platform.Model.Metadata.Enums;
    using DoubleGis.Erm.Platform.UI.Web.Mvc;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Security;
    using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
    using NuClear.Model.Common.Entities;
    using NuClear.Model.Common.Operations.Identity;
    using NuClear.Model.Common.Operations.Identity.Generic;
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/ReleaseInfo/ReleaseDialog.cshtml")]
    public partial class ReleaseDialog : System.Web.Mvc.WebViewPage<ReleaseDialogViewModel>
    {
        public ReleaseDialog()
        {
        }
        public override void Execute()
        {
            
            #line 3 "..\..\Views\ReleaseInfo\ReleaseDialog.cshtml"
  
    Layout = "../Shared/_DialogLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("Title", () => {

WriteLiteral(" ");

            
            #line 7 "..\..\Views\ReleaseInfo\ReleaseDialog.cshtml"
            Write(BLResources.PeriodAssembling);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarTitle", () => {

WriteLiteral(" ");

            
            #line 8 "..\..\Views\ReleaseInfo\ReleaseDialog.cshtml"
                  Write(BLResources.PeriodAssembling);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

DefineSection("TopBarMessage", () => {

WriteLiteral(" ");

            
            #line 9 "..\..\Views\ReleaseInfo\ReleaseDialog.cshtml"
                    Write(BLResources.SpecifyAssemblyPeriodAndOrganizationUnit);

            
            #line default
            #line hidden
WriteLiteral(" ");

});

WriteLiteral("\r\n");

DefineSection("PageContent", () => {

WriteLiteral("\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(">\r\n        td.itemCaption\r\n        {\r\n            vertical-align: top;\r\n         " +
"   padding-top: 5px;\r\n        }\r\n        td.itemValue\r\n        {\r\n            ve" +
"rtical-align: top;\r\n        }\r\n    </style>\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        Ext.onReady(function ()\r\n        {\r\n            var isSuccess = \'");

            
            #line 27 "..\..\Views\ReleaseInfo\ReleaseDialog.cshtml"
                        Write(Model.IsSuccess);

            
            #line default
            #line hidden
WriteLiteral(@"';
            if (isSuccess == 'True') {
                alert(Ext.getDom(""Notifications"").innerHTML.trim());
                window.close();
                return;
            } else if (Ext.getDom(""Notifications"").innerHTML.trim() != """")
            {
                Ext.getDom(""Notifications"").style.display = ""block"";
            }

            // show error messages
            if (Ext.getDom(""Notifications"").innerHTML.trim() != """")
            {
                Ext.get(""Notifications"").addClass(""Notifications"");
            }
            else
            {
                Ext.get(""Notifications"").removeClass(""Notifications"");
            }

            Ext.get(""Cancel"").on(""click"", function () { window.close(); });
            Ext.get(""OK"").on(""click"", function ()
            {
                if (Ext.DoubleGis.FormValidator.validate(EntityForm))
                {
                    Ext.getDom(""OK"").disabled = ""disabled"";
                    Ext.getDom(""Cancel"").disabled = ""disabled"";
                    EntityForm.submit();
                }
            });
        });
    </script>
");

            
            #line 59 "..\..\Views\ReleaseInfo\ReleaseDialog.cshtml"
    
            
            #line default
            #line hidden
            
            #line 59 "..\..\Views\ReleaseInfo\ReleaseDialog.cshtml"
     using (Html.BeginForm(null, null, null, FormMethod.Post, new Dictionary<string, object> { { "id", "EntityForm" } }))
    {

            
            #line default
            #line hidden
WriteLiteral("    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(">\r\n        <div");

WriteLiteral(" style=\"display: none; height: 15px;\"");

WriteLiteral(" id=\"Notifications\"");

WriteLiteral(" class=\"Notifications\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 63 "..\..\Views\ReleaseInfo\ReleaseDialog.cshtml"
       Write(Model.Message);

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 66 "..\..\Views\ReleaseInfo\ReleaseDialog.cshtml"
       Write(Html.TemplateField(m => m.OrganizationUnit, FieldFlex.lone, new LookupSettings{EntityName = EntityType.Instance.OrganizationUnit()}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 69 "..\..\Views\ReleaseInfo\ReleaseDialog.cshtml"
       Write(Html.TemplateField(m => m.IsBeta, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 72 "..\..\Views\ReleaseInfo\ReleaseDialog.cshtml"
       Write(Html.TemplateField(m => m.PeriodStart, FieldFlex.lone, new CalendarSettings
                                                                        {
                                                                            Store = CalendarSettings.StoreMode.Relative,
                                                                            Display = CalendarSettings.DisplayMode.Month,
                                                                        }));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

            
            #line 79 "..\..\Views\ReleaseInfo\ReleaseDialog.cshtml"
    }

            
            #line default
            #line hidden
});

        }
    }
}
#pragma warning restore 1591
