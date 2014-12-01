using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IFirmAddressViewModel : IEntityViewModelAbstract<FirmAddress>
    {
        string Address { get; set; }
    }
}