using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models.Report
{
    public class ReportModel : ViewModel, IConfigurableViewModel
    {
        private EntityViewConfig _viewConfig;

        public string DisplayName { get; set; }
        public string ReportName { get; set; }
        public string ReportType { get; set; }
        public string Format { get; set; }
        public string HiddenField { get; set; }
        public IEnumerable<ReportFieldDefinition> Fields { get; set; }
        public string ReportServerFormatParameter { get; set; }

        public EntityViewConfig ViewConfig
        {
            get { return _viewConfig ?? (_viewConfig = new EntityViewConfig()); }
            set { _viewConfig = value; }
        }

        public class ReportFieldDefinition
        {
            public Type Type { get; set; }
            public string Name { get; set; }
            public object Config { get; set; }
            public object DefaultValue { get; set; }

            /// <summary>Выражение, используемое для получения значения поля модели</summary>
            public Expression GetExpression { get; set; }
        }
    }
}
