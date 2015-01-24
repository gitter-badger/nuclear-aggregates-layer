using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IPositionViewModel : IViewModelAbstract
    {
        bool IsComposite { get; set; }
    }
}