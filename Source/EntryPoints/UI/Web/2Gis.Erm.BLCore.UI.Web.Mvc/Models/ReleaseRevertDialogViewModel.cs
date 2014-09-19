using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public sealed class ReleaseRevertDialogViewModel : ViewModel
    {
        [RequiredLocalized]
        public LookupField OrganizationUnit { get; set; }

        [Calendar]
        [RequiredLocalized]
        [DisplayNameLocalized("PaymentMonth")]
        public DateTime PeriodStart { get; set; }

        [StringLengthLocalized(200)]
        public string Comment { get; set; }
    }
}