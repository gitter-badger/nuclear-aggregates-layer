using DoubleGis.Erm.BLCore.UI.Web.Mvc.Attributes;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;
using DoubleGis.Erm.Platform.UI.Web.Mvc.ViewModels;

namespace DoubleGis.Erm.BL.UI.Web.Mvc.Models
{
    public sealed class CopyPriceViewModel : ViewModel
    {
        public long SourcePriceId { get; set; }
        
        public bool CopyNewPrice { get; set; }
        
        [RequiredLocalized]
        public LookupField TargetPrice { get; set; }
    }
}