using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class ChangeOrderStateOnApprovalViewModel : ViewModel
    {
        public long OrderId { get; set; }

        public long? SourceOrganizationUnitId { get; set; }

        [RequiredLocalized]
        public LookupField Inspector { get; set; }
    }
}