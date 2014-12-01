using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface ILegalPersonProfileViewModel : IEntityViewModelAbstract<LegalPersonProfile>
    {
        string Name { get; set; }
    }
}