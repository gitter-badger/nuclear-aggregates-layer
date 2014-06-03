using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Modify;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify.DomainEntityObtainers
{
    public class RussiaBranchOfficeOrganizationUnitObtainer : BranchOfficeOrganizationUnitObtainerBase<RussiaBranchOfficeOrganizationUnitDomainEntityDto>, IRussiaAdapted
    {
        public RussiaBranchOfficeOrganizationUnitObtainer(IFinder finder)
            : base(finder)
        {
        }

        protected override IAssignSpecification<RussiaBranchOfficeOrganizationUnitDomainEntityDto, BranchOfficeOrganizationUnit> GetAssignSpecification()
        {
            return BranchOfficeFlexSpecs.BranchOfficeOrganizationUnits.Russia.Assign.Entity();
        }
    }
}