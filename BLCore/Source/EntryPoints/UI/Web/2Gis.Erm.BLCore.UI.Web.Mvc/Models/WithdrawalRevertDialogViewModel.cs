using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public sealed class WithdrawalRevertDialogViewModel : ViewModel
    {
        private DateTime _periodStart;

        [RequiredLocalized]
        public LookupField OrganizationUnit { get; set; }

        [RequiredLocalized]
        [ExcludeZeroValue]
        public AccountingMethod AccountingMethod { get; set; }

        [Calendar]
        [RequiredLocalized]
        [DisplayNameLocalized("PaymentMonth")]
        public DateTime PeriodStart
        {
            get { return _periodStart; }
            set { _periodStart = value.AssumeUtcKind(); }
        }

        [StringLengthLocalized(200)]
        public string Comment { get; set; }
    }
}