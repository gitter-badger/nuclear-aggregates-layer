using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IAssociatedPositionViewModel : IEntityViewModelAbstract<AssociatedPosition>
    {
        bool PriceIsDeleted { get; set; }
        bool PriceIsPublished { get; set; }
    }
}