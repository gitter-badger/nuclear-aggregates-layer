using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public sealed class WithdrawalDialogViewModel : ViewModel
    {
        private DateTime _periodStart;

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

        public Guid OperationId { get; set; }
        public bool? HasErrors { get; set; }
    }
}