using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IProjectViewModel : IEntityViewModelAbstract<Project>
    {
        string DisplayName { get; set; }
    }
}