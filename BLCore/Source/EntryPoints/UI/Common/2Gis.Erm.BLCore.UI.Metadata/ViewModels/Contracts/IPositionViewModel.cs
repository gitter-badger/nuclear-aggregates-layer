using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IPositionViewModel : IEntityViewModelAbstract<Position>
    {
        string Name { get; set; }
        bool IsComposite { get; set; }
    }
}