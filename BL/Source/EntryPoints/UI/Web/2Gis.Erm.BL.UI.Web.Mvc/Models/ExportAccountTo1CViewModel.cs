using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public class ExportAccountTo1CViewModel : ViewModel
    {
        private DateTime _periodStart;

        [RequiredLocalized]
        public LookupField OrganizationUnit { get; set; }

        [Calendar]
        [RequiredLocalized]
        [DisplayNameLocalized("PaymentMonth")]
        public DateTime PeriodStart
        {
            get { return _periodStart; }
            set { _periodStart = value.AssumeUtcKind(); }
        }

        public bool? HasResult { get; set; }
        public Guid FileId { get; set; }
    }

    public class ExportAccountToServiceBusViewModel : ViewModel
    {
        private DateTime _periodStart;

        [RequiredLocalized]
        public LookupField OrganizationUnit { get; set; }

        [Calendar]
        [RequiredLocalized]
        [DisplayNameLocalized("PaymentMonth")]
        public DateTime PeriodStart
        {
            get { return _periodStart; }
            set { _periodStart = value.AssumeUtcKind(); }
        }

        public bool CreateCsvFile { get; set; }

        public bool? HasResult { get; set; }
        public Guid FileId { get; set; }
    }
}