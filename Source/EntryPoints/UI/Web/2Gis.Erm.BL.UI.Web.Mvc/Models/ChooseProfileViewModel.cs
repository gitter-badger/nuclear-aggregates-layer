using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class ChooseProfileViewModel : ViewModel
    {
        [RequiredLocalized]
        public LookupField LegalPersonProfile { get; set; }

        public long? DefaultLegalPersonProfileId { get; set; }

        public long LegalPersonId { get; set; }

        public bool IsCardReadOnly { get; set; }
    }
}