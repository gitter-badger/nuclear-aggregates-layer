using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public sealed class CloseDealViewModel : ViewModel
    {
        public long Id { get; set; }

        public CloseDealReason CloseReason { get; set; }

        public string CloseReasonOther { get; set; }

        public string Comment { get; set; }
    }
}