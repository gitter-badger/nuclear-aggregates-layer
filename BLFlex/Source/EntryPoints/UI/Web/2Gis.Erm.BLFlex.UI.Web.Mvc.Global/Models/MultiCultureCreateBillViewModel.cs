using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models
{
    public sealed class MultiCultureCreateBillViewModel : ViewModel, IRussiaAdapted, ICyprusAdapted, ICzechAdapted, IUkraineAdapted
    {
        public long OrderId { get; set; }
        public BillPaymentType PaymentType { get; set; }
        public int PaymentsAmount { get; set; }
        public bool IsMassBillCreateAvailable { get; set; }
    }
}
