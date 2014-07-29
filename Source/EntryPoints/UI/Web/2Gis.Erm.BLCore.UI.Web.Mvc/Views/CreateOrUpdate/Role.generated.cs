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

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Views.CreateOrUpdate
{
    using System;
    using System.Web.Mvc.Html;

    using DoubleGis.Erm.BLCore.Resources.Server.Properties;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Models;
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Utils;

    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Role.cshtml")]
    public partial class Role : System.Web.Mvc.WebViewPage<RoleViewModel>
    {
        public Role()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 4 "..\..\Views\CreateOrUpdate\Role.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">
        window.InitPage = function ()
        {
            this.on('beforepost', function ()
            {
                this.genericSave(this.submitMode);
                return false;
            });
            Ext.apply(this, {
                genericSave: function (submitMode)
                {
                    if (this.ReadOnly)
                    {
                        this.AddNotification(Ext.LocalizedResources.CardIsReadOnly, ""Notification"", ""CardIsReadOnly"");
                        return;
                    }
                    var card = this;
                    var continuation = function ()
                    {
                        card.submitMode = submitMode;
                        if (card.normalizeForm() !== false)
                        {
                            card.postForm();
                        }
                    };

                    var failure = function ()
                    {
                        // TODO {all, 18.12.2013}: возможно некоректное отображение диакритики
                        // TODO {all, 18.12.2013}: alert можно заменить на ext'овый messagebox
                        // TODO {all, 18.12.2013}: ресурс можно перенести в ClientResourceStorage
                        alert('");

            
            #line 41 "..\..\Views\CreateOrUpdate\Role.cshtml"
                          Write(BLResources.SaveError);

            
            #line default
            #line hidden
WriteLiteral(@"');
                        card.Items.Toolbar.enable();
                    };

                    var entityPrivilegeIFrame = Ext.getDom('RoleEntityPrivilege_frame');
                    var functionalPrivilegeIFrame = Ext.getDom('RoleFunctionalPrivilege_frame');

                    if (entityPrivilegeIFrame && !functionalPrivilegeIFrame)
                    {
                        entityPrivilegeIFrame.contentWindow.BeforeSave(continuation, failure);
                    } else if (!entityPrivilegeIFrame && functionalPrivilegeIFrame)
                    {
                        functionalPrivilegeIFrame.contentWindow.BeforeSave(continuation, failure);
                    } else if (entityPrivilegeIFrame && functionalPrivilegeIFrame)
                    {
                        var currentContinuation = function ()
                        {
                            functionalPrivilegeIFrame.contentWindow.BeforeSave(continuation, failure);
                        };
                        entityPrivilegeIFrame.contentWindow.BeforeSave(currentContinuation, failure);
                    } else
                    {
                        continuation();
                    }
                }
            });
        };
    </script>
");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n");

WriteLiteral("    ");

            
            #line 73 "..\..\Views\CreateOrUpdate\Role.cshtml"
Write(Html.HiddenFor(m => m.IdentityServiceUrl));

            
            #line default
            #line hidden
WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 2863), Tuple.Create("\"", 2899)
            
            #line 74 "..\..\Views\CreateOrUpdate\Role.cshtml"
, Tuple.Create(Tuple.Create("", 2871), Tuple.Create<System.Object, System.Int32>(BLResources.GeneralTabTitle
            
            #line default
            #line hidden
, 2871), false)
);

WriteLiteral(">\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 76 "..\..\Views\CreateOrUpdate\Role.cshtml"
       Write(Html.EditableId(m => m.Id));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 79 "..\..\Views\CreateOrUpdate\Role.cshtml"
       Write(Html.TemplateField(m => m.Name, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
