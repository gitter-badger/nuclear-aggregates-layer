using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Metadata.Models.Contracts
{
    public interface IContactViewModel : IEntityViewModelAbstract<Contact>
    {
        string FullName { get; set; }
    }
}