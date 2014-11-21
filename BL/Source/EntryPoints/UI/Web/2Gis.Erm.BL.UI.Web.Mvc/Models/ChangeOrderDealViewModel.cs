using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public class ChangeOrderDealViewModel: ViewModel
    {
        public long OrderId { get; set; }

        [RequiredLocalized]
        public LookupField Deal { get; set; }
    }
}