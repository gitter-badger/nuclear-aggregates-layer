using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public sealed class WithdrawalDialogViewModel : ViewModel
    {
        [RequiredLocalized]
        public LookupField OrganizationUnit { get; set; }

        [RequiredLocalized]
        [DisplayNameLocalized("PaymentMonth")]
        public DateTime PeriodStart { get; set; }
    }
}