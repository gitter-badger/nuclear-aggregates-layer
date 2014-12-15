using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IAdvertisementElementViewModel : IEntityViewModelAbstract<AdvertisementElement>
    {
        bool CanUserChangeStatus { get; set; }
        bool DisableEdit { get; set; }
        AdvertisementElementStatusValue Status { get; set; }
        bool NeedsValidation { get; set; }
    }
}