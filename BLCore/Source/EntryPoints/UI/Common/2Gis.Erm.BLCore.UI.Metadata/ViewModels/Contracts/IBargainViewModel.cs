using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IBargainViewModel : IEntityViewModelAbstract<Bargain>
    {
        string Number { get; set; }
    }
}
