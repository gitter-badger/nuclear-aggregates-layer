using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IFirmContactViewModel : IEntityViewModelAbstract<FirmContact>
    {
        string Contact { get; set; }
    }
}