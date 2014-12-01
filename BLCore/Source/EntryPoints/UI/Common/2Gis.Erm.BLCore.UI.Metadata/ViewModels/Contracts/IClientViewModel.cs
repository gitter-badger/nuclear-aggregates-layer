using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IClientViewModel : IEntityViewModelAbstract<Client>
    {
        string Name { get; set; }
    }
}