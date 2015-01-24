using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IOrderViewModel : IViewModelAbstract
    {
        bool CanSwitchToAccount { get; set; }
    }
}