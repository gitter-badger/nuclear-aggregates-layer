using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public sealed class DealCreateReasonViewModel : ViewModel
    {
        [DisplayNameLocalized("Reason")]
        public ReasonForNewDeal ReasonForNewDeal { get; set; }
    }
}