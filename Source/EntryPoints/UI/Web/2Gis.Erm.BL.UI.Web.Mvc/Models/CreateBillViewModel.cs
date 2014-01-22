using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class CreateBillViewModel : ViewModel
    {
        public long OrderId { get; set; }
        public BillPaymentType PaymentType { get; set; }
        public int PaymentsAmount { get; set; }
        public bool IsMassBillCreateAvailable { get; set; }
    }
}
