using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public sealed class DeactivateUserViewModel : ViewModel
    {
        public LookupField UserCode { get; set; }
        public long DeactivatedUserCode { get; set; }
        public bool AssignToMe { get; set; }
    }
}