using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IDeniedPositionViewModel : IEntityViewModelAbstract<DeniedPosition>
    {
        bool IsPricePublished { get; set; }
    }
}