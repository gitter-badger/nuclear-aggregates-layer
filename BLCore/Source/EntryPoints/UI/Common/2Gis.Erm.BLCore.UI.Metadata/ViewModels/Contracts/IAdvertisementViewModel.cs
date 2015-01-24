using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IAdvertisementViewModel : IViewModelAbstract
    {
        bool IsDummy { get; set; }
        bool IsSelectedToWhiteList { get; set; }
    }
}
