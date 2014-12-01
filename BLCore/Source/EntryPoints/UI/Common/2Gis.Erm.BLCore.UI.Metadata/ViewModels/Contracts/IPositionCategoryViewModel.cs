using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IPositionCategoryViewModel : IEntityViewModelAbstract<PositionCategory>
    {
        string Name { get; set; }
    }
}