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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/CreateOrUpdate/Note.cshtml")]
    public partial class Note : System.Web.Mvc.WebViewPage<NoteViewModel>
    {
        public Note()
        {
        }
        public override void Execute()
        {
WriteLiteral("\r\n");

            
            #line 4 "..\..\Views\CreateOrUpdate\Note.cshtml"
  
    Layout = "../Shared/_CardLayout.cshtml";

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

DefineSection("CardScripts", () => {

WriteLiteral("\r\n    <style");

WriteLiteral(" type=\"text/css\"");

WriteLiteral(">\r\n        DIV.row-wrapper TABLE TD\r\n        {\r\n            padding-left: 5px;\r\n " +
"       }\r\n        TD.span-wrapper\r\n        {\r\n            padding-left: 10px !im" +
"portant;\r\n            padding-right: 10px;\r\n        }\r\n    </style>\r\n    <script" +
"");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(@">
        window.InitPage = function ()
        {
            this.on(""afterbuild"", function () {
                var u = new Ext.ux.AsyncFileUpload(
                    {
                        applyTo: 'FileId',
                        uploadUrl: '/Upload/Note',
                        downloadUrl: Ext.BasicOperationsServiceRestUrl + 'DownloadBinary.svc/Rest/Note/{0}',
                        listeners: {
                            fileuploadbegin: function ()
                            {
                                this.Items.Toolbar.disable();
                            },
                            fileuploadcomplete: function ()
                            {
                                this.Items.Toolbar.enable();
                            },
                            fileuploadsuccess: function () {
                                if (Ext.getDom('FileExists').value.toLowerCase() == ""true"") {
                                    this.refresh();
                                }
                            },
                            scope: this
                        },
                        fileInfo:
                            {
                                fileId: '");

            
            #line 48 "..\..\Views\CreateOrUpdate\Note.cshtml"
                                    Write(Model.FileId);

            
            #line default
            #line hidden
WriteLiteral("\',\r\n                                fileName: \'");

            
            #line 49 "..\..\Views\CreateOrUpdate\Note.cshtml"
                                      Write(Model.FileName);

            
            #line default
            #line hidden
WriteLiteral("\',\r\n                                contentType: \'");

            
            #line 50 "..\..\Views\CreateOrUpdate\Note.cshtml"
                                         Write(Model.FileContentType);

            
            #line default
            #line hidden
WriteLiteral("\',\r\n                                fileSize: \'");

            
            #line 51 "..\..\Views\CreateOrUpdate\Note.cshtml"
                                      Write(Model.FileContentLength);

            
            #line default
            #line hidden
WriteLiteral(@"'
                            }
                    });
            }, this);

            this.on('afterpost', function () {
                Ext.getDom('FileExists').value = Ext.getDom('FileId').value > 0;
            }, this);
        };
    </script>
");

});

WriteLiteral("\r\n");

DefineSection("CardBody", () => {

WriteLiteral("\r\n    <div");

WriteLiteral(" class=\"Tab\"");

WriteLiteral(" id=\"MainTab\"");

WriteAttribute("title", Tuple.Create(" title=\"", 2183), Tuple.Create("\"", 2208)
            
            #line 65 "..\..\Views\CreateOrUpdate\Note.cshtml"
, Tuple.Create(Tuple.Create("", 2191), Tuple.Create<System.Object, System.Int32>(BLResources.Note
            
            #line default
            #line hidden
, 2191), false)
);

WriteLiteral(">\r\n");

            
            #line 66 "..\..\Views\CreateOrUpdate\Note.cshtml"
        
            
            #line default
            #line hidden
            
            #line 66 "..\..\Views\CreateOrUpdate\Note.cshtml"
         if (Model != null)
        {
            
            
            #line default
            #line hidden
            
            #line 68 "..\..\Views\CreateOrUpdate\Note.cshtml"
       Write(Html.HiddenFor(m => m.Id));

            
            #line default
            #line hidden
            
            #line 68 "..\..\Views\CreateOrUpdate\Note.cshtml"
                                      
            
            
            #line default
            #line hidden
            
            #line 69 "..\..\Views\CreateOrUpdate\Note.cshtml"
       Write(Html.HiddenFor(m => m.ParentId));

            
            #line default
            #line hidden
            
            #line 69 "..\..\Views\CreateOrUpdate\Note.cshtml"
                                            
            
            
            #line default
            #line hidden
            
            #line 70 "..\..\Views\CreateOrUpdate\Note.cshtml"
       Write(Html.HiddenFor(m => m.ParentTypeName));

            
            #line default
            #line hidden
            
            #line 70 "..\..\Views\CreateOrUpdate\Note.cshtml"
                                                  
            
            
            #line default
            #line hidden
            
            #line 71 "..\..\Views\CreateOrUpdate\Note.cshtml"
       Write(Html.Hidden("FileExists", Model.FileId > 0));

            
            #line default
            #line hidden
            
            #line 71 "..\..\Views\CreateOrUpdate\Note.cshtml"
                                                        
        }

            
            #line default
            #line hidden
WriteLiteral("        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 74 "..\..\Views\CreateOrUpdate\Note.cshtml"
       Write(Html.TemplateField(m => m.Title, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 77 "..\..\Views\CreateOrUpdate\Note.cshtml"
       Write(Html.TemplateField(m => m.Text, FieldFlex.lone, new Dictionary<string, object>{{"rows", 5}}));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n        <div");

WriteLiteral(" class=\"row-wrapper\"");

WriteLiteral(">\r\n");

WriteLiteral("            ");

            
            #line 80 "..\..\Views\CreateOrUpdate\Note.cshtml"
       Write(Html.TemplateField(m => m.FileId, FieldFlex.lone));

            
            #line default
            #line hidden
WriteLiteral("\r\n        </div>\r\n    </div>\r\n");

});

        }
    }
}
#pragma warning restore 1591
