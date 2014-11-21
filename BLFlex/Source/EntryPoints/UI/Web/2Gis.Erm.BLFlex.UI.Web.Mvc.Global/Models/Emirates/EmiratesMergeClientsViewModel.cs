using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Emirates
{
    public sealed class EmiratesMergeClientsViewModel : ViewModel
    {
        [RequiredLocalized]
        public LookupField Client1 { get; set; }
        [RequiredLocalized]
        public LookupField Client2 { get; set; }
        public EmiratesClientViewModel PostData { get; set; }

        public bool AssignAllObjects { get; set; }
        public bool DisableMasterClient { get; set; }
    }

    public sealed class EmiratesMergeClientsDataViewModel : ViewModel
    {
        public EmiratesClientViewModel Client1 { get; set; }
        public EmiratesClientViewModel Client2 { get; set; }
    }
}