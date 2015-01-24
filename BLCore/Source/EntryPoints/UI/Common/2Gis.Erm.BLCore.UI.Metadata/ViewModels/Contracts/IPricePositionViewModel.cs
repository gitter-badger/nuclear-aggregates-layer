using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.Web.Mvc.Utils;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IPricePositionViewModel : IViewModelAbstract
    {
        LookupField Position { get; set; }
        LookupField Price { get; set; }
    }
}