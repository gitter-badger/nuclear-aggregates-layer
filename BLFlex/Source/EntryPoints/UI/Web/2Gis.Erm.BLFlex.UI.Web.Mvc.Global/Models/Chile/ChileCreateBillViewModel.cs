using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Chile
{
    public sealed class ChileCreateBillViewModel : ViewModel, IChileAdapted
    {
        public long OrderId { get; set; }
        public BillPaymentType PaymentType { get; set; }
        public int PaymentsAmount { get; set; }
    }
}
