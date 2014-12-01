using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IBargainTypeViewModel : IEntityViewModelAbstract<BargainType>
    {
        string Name { get; set; }
    }
}
