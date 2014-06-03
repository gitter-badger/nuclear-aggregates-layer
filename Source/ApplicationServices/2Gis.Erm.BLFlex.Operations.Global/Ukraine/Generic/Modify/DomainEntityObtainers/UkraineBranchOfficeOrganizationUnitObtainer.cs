using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Modify;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Modify.DomainEntityObtainers
{
    public class UkraineBranchOfficeOrganizationUnitObtainer : BranchOfficeOrganizationUnitObtainerBase<UkraineBranchOfficeOrganizationUnitDomainEntityDto>, IUkraineAdapted
    {
        public UkraineBranchOfficeOrganizationUnitObtainer(IFinder finder)
            : base(finder)
        {
        }

        protected override IAssignSpecification<UkraineBranchOfficeOrganizationUnitDomainEntityDto, BranchOfficeOrganizationUnit> GetAssignSpecification()
        {
            return BranchOfficeFlexSpecs.BranchOfficeOrganizationUnits.Ukraine.Assign.Entity();
        }
    }
}