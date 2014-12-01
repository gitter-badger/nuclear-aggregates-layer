using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IDepartmentViewModel : IEntityViewModelAbstract<Department>
    {
        string Name { get; set; }
    }
}