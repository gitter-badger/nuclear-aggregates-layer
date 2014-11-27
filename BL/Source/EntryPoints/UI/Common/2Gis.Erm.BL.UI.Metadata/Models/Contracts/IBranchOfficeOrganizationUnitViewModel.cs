using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Metadata.Models.Contracts
{
    public interface IBranchOfficeOrganizationUnitViewModel : IEntityViewModelAbstract<BranchOfficeOrganizationUnit>
    {
        string ShortLegalName { get; set; }
    }
}
