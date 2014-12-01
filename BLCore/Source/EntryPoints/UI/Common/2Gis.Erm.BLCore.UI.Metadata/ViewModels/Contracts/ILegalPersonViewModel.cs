using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface ILegalPersonViewModel : IEntityViewModelAbstract<LegalPerson>
    {
        string LegalName { get; set; }
    }
}