using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IRoleViewModel : IEntityViewModelAbstract<Role>
    {
        string Name { get; set; }
    }
}