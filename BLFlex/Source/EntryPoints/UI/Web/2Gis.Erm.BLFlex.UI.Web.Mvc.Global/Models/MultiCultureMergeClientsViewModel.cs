using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models
{
    public sealed class MultiCultureMergeClientsViewModel : ViewModel
    {
        [RequiredLocalized]
        public LookupField Client1 { get; set; }
        [RequiredLocalized]
        public LookupField Client2 { get; set; }
        public MultiCultureClientViewModel PostData { get; set; }

        public bool AssignAllObjects { get; set; }
        public bool DisableMasterClient { get; set; }
    }

    public sealed class MultiCultureMergeClientsDataViewModel : ViewModel
    {
        public MultiCultureClientViewModel Client1 { get; set; }
        public MultiCultureClientViewModel Client2 { get; set; }
    }
}