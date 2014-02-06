using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class MergeClientsViewModel : ViewModel
    {
        [RequiredLocalized]
        public LookupField Client1 { get; set; }
        [RequiredLocalized]
        public LookupField Client2 { get; set; }
        public ClientViewModel PostData { get; set; }
    }

    public sealed class MergeClientsDataViewModel : ViewModel
    {
        public ClientViewModel Client1 { get; set; }
        public ClientViewModel Client2 { get; set; }
    }
}