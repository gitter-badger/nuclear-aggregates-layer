using DoubleGis.Erm.BLCore.UI.Metadata.Aspects;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IAssociatedPositionViewModel : INewableAspect
    {
        bool PriceIsDeleted { get; set; }
        bool PriceIsPublished { get; set; }
    }
}