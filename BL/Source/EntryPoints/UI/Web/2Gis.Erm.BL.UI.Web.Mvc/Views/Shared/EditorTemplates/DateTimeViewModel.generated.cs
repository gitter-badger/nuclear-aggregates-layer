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

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Views.Shared.EditorTemplates
{
    using System;
    using System.Collections.Generic;
    
    #line 1 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
    using System.Globalization;
    
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
    
    #line 2 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
    using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
    
    #line default
    #line hidden
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
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/Shared/EditorTemplates/DateTimeViewModel.cshtml")]
    public partial class _DateTimeViewModel : System.Web.Mvc.WebViewPage<DateTime?>
    {
        public _DateTimeViewModel()
        {
        }
        public override void Execute()
        {
            
            #line 5 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
  
    const string ShortIsoFormat = "yyyy-MM-ddTHH:mm:ss";
    const string FullIsoFormat = "o";

    if (!ViewData.ModelMetadata.AdditionalValues.ContainsKey(CalendarAttribute.Name))
    {
        var message = string.Format("Field {0} should contain attribute CalendarAttribute in order to use new calendar control", @Html.IdForModel());
        throw new InvalidOperationException(message);
    }

    var settings = (CalendarSettings)ViewData["CalendarSettings"];
    var isoFormat = settings.Store == CalendarSettings.StoreMode.Relative
        ? ShortIsoFormat
        : FullIsoFormat;

    var storeMode = settings.Store.ToString().ToLower();
    var displayMode = settings.Display.ToString().ToLower();
    var readOnly = settings.ReadOnly.ToString().ToLower();

    if (Model.HasValue && Model.Value.Kind == DateTimeKind.Unspecified)
    {
        throw new ArgumentException(string.Format("DateTimeKind.Unspecified не допустим. Поле {0}", Html.IdForModel()));
    }
    
    var value = Model.HasValue
                      ? Model.Value.ToString(isoFormat, CultureInfo.InvariantCulture)
                      : string.Empty;

    var minDate = settings.MinDate.HasValue
                      ? settings.MinDate.Value.ToString(isoFormat, CultureInfo.InvariantCulture)
                      : string.Empty;

    var maxDate = settings.MaxDate.HasValue
                      ? settings.MaxDate.Value.ToString(isoFormat, CultureInfo.InvariantCulture)
                      : string.Empty;

    var time = settings.Time;
    if (time != null && time.Start < TimeSpan.FromDays(0))
    {
        throw new ArgumentException("Start time must be greater than 0");
    }

    if (time != null && time.Start >= TimeSpan.FromDays(1))
    {
        throw new ArgumentException("End time must be less than 24 hours");
    }

            
            #line default
            #line hidden
WriteLiteral("\r\n\r\n");

            
            #line 53 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
Write(Html.TextBox(string.Empty, value));

            
            #line default
            #line hidden
WriteLiteral("\r\n<table");

WriteAttribute("id", Tuple.Create(" id=\"", 2015), Tuple.Create("\"", 2046)
            
            #line 54 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
, Tuple.Create(Tuple.Create("", 2020), Tuple.Create<System.Object, System.Int32>(Html.IdForModel()
            
            #line default
            #line hidden
, 2020), false)
, Tuple.Create(Tuple.Create("", 2038), Tuple.Create("_wrapper", 2038), true)
);

WriteLiteral(" class=\"x-calendar-v2\"");

WriteLiteral(" >\r\n    <tbody>\r\n        <tr>\r\n            <td>\r\n");

WriteLiteral("                ");

            
            #line 58 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
           Write(Html.TextBox("editor", "", new Dictionary<string, object> { { "class", "inputfields" } }));

            
            #line default
            #line hidden
WriteLiteral("\r\n            </td>\r\n            <td");

WriteLiteral(" style=\"width: 36px;\"");

WriteLiteral(">\r\n                <div");

WriteAttribute("id", Tuple.Create(" id=\"", 2304), Tuple.Create("\"", 2331)
            
            #line 61 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
, Tuple.Create(Tuple.Create("", 2309), Tuple.Create<System.Object, System.Int32>(Html.IdForModel()
            
            #line default
            #line hidden
, 2309), false)
, Tuple.Create(Tuple.Create("", 2327), Tuple.Create("_btn", 2327), true)
);

WriteLiteral(" class=\"calendar-button calendar-button-normal\"");

WriteLiteral("></div>\r\n            </td>\r\n\r\n");

            
            #line 64 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
            
            
            #line default
            #line hidden
            
            #line 64 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
             if (time != null)
            {

            
            #line default
            #line hidden
WriteLiteral("                <td");

WriteLiteral(" style=\"width: 10px\"");

WriteLiteral("></td>\r\n");

WriteLiteral("                <td");

WriteLiteral(" style=\"width: 80px;\"");

WriteAttribute("id", Tuple.Create(" id=\"", 2543), Tuple.Create("\"", 2571)
            
            #line 67 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
, Tuple.Create(Tuple.Create("", 2548), Tuple.Create<System.Object, System.Int32>(Html.IdForModel()
            
            #line default
            #line hidden
, 2548), false)
, Tuple.Create(Tuple.Create("", 2566), Tuple.Create("_time", 2566), true)
);

WriteLiteral("></td>\r\n");

            
            #line 68 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
            }

            
            #line default
            #line hidden
WriteLiteral("        </tr>\r\n    </tbody>\r\n</table>\r\n\r\n");

            
            #line 73 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
 if (time != null)
{

            
            #line default
            #line hidden
WriteLiteral("    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        var time = {\r\n            min: \'");

            
            #line 77 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
             Write(time.Start.ToString("c"));

            
            #line default
            #line hidden
WriteLiteral("\',\r\n            max: \'");

            
            #line 78 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
             Write(time.End.ToString("c"));

            
            #line default
            #line hidden
WriteLiteral("\',\r\n            step: ");

            
            #line 79 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
             Write(time.Step.TotalMilliseconds);

            
            #line default
            #line hidden
WriteLiteral("\r\n        };\r\n    </script>\r\n");

            
            #line 82 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
}
else
{

            
            #line default
            #line hidden
WriteLiteral("    <script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n        var time = null;\r\n    </script>\r\n");

            
            #line 88 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
}

            
            #line default
            #line hidden
WriteLiteral("\r\n<script");

WriteLiteral(" type=\"text/javascript\"");

WriteLiteral(">\r\n    new Ext.ux.Calendar2({\r\n        readOnly: ");

            
            #line 92 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
             Write(readOnly);

            
            #line default
            #line hidden
WriteLiteral(",\r\n        storeId: \'");

            
            #line 93 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
             Write(Html.IdForModel());

            
            #line default
            #line hidden
WriteLiteral("\',\r\n        editorId: \'");

            
            #line 94 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
              Write(Html.IdForModel());

            
            #line default
            #line hidden
WriteLiteral("_editor\',\r\n        buttonId: \'");

            
            #line 95 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
              Write(Html.IdForModel());

            
            #line default
            #line hidden
WriteLiteral("_btn\',\r\n        timeId: \'");

            
            #line 96 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
            Write(Html.IdForModel());

            
            #line default
            #line hidden
WriteLiteral("_time\',\r\n        mode: {\r\n            store: \'");

            
            #line 98 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
               Write(storeMode);

            
            #line default
            #line hidden
WriteLiteral("\',\r\n            display: \'");

            
            #line 99 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
                 Write(displayMode);

            
            #line default
            #line hidden
WriteLiteral("\',\r\n            time: time\r\n        },\r\n        minDate: \'");

            
            #line 102 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
             Write(minDate);

            
            #line default
            #line hidden
WriteLiteral("\',\r\n        maxDate: \'");

            
            #line 103 "..\..\Views\Shared\EditorTemplates\DateTimeViewModel.cshtml"
             Write(maxDate);

            
            #line default
            #line hidden
WriteLiteral("\'\r\n    });\r\n</script>\r\n");

        }
    }
}
#pragma warning restore 1591
