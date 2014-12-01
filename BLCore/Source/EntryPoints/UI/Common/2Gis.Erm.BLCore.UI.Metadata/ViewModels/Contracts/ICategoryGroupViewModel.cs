using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface ICategoryGroupViewModel : IEntityViewModelAbstract<CategoryGroup>
    {
        string CategoryGroupName { get; set; }
    }
}