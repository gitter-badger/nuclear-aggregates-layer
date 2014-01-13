using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Models
{
    public class CopyPricePositionModel : ViewModel
    {
        [PresentationLayerProperty]
        public long PriceId { get; set; }
        
        [PresentationLayerProperty]
        public long SourcePricePositionId { get; set; }

        [RequiredLocalized]
        public LookupField Position { get; set; }
    }
}