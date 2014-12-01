using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IAdvertisementElementTemplateViewModel : IEntityViewModelAbstract<AdvertisementElementTemplate>
    {
        string Name { get; set; }
    }
}
