using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IBranchOfficeViewModel : IEntityViewModelAbstract<BranchOffice>
    {
        string Name { get; set; }
    }
}
