using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public sealed class PrintOrderViewModel : ViewModel
    {
        [RequiredLocalized]
        public LookupField LegalPersonProfile { get; set; }

        public PrintOrderType PrintOrderType { get; set; }

        public long LegalPersonId { get; set; }

        public long OrderId { get; set; }
    }
}