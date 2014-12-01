using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IUserViewModel : IEntityViewModelAbstract<User>
    {
        string DisplayName { get; set; }
    }
}