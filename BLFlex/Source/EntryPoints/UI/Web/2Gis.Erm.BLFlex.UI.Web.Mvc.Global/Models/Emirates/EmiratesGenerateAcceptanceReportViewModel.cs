using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Emirates
{
    public sealed class EmiratesGenerateAcceptanceReportViewModel : ViewModel, IEmiratesAdapted
    {
        private DateTime _month;

        [Calendar]
        [RequiredLocalized]
        public DateTime Month
        {
            get { return _month; }
            set { _month = value.AssumeUtcKind(); }
        }

        [RequiredLocalized]
        public LookupField OrganizationUnit { get; set; }

        public long UserId { get; set; }
    }
}