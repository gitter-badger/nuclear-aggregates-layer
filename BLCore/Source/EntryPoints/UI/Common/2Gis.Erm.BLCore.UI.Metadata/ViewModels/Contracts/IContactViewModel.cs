using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IContactViewModel : IEntityViewModelAbstract<Contact>
    {
        string FullName { get; set; }
    }
}