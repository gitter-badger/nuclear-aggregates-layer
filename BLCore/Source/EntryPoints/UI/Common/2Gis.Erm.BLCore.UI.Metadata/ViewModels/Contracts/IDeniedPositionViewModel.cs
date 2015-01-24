using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IDeniedPositionViewModel : IViewModelAbstract
    {
        bool IsPricePublished { get; set; }
    }
}