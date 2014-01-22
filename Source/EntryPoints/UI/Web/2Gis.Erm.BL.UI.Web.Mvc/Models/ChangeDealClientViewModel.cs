using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class ChangeDealClientViewModel : ViewModel
    {
        public long DealId { get; set; }
        [RequiredLocalized]
        public LookupField Client { get; set; }
    }
}