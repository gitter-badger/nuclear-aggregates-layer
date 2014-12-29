using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IAdvertisementViewModel : IEntityViewModelAbstract<Advertisement>
    {
        string Name { get; set; }
        bool IsDummy { get; set; }
        bool IsSelectedToWhiteList { get; set; }
    }
}
