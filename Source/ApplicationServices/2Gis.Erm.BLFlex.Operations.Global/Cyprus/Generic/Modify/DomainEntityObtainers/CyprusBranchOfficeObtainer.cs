using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Modify;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Generic.Modify.DomainEntityObtainers
{
    public class CyprusBranchOfficeObtainer : BranchOfficeObtainerBase<CyprusBranchOfficeDomainEntityDto>, ICyprusAdapted
    {
        public CyprusBranchOfficeObtainer(IFinder finder)
            : base(finder)
        {
        }

        protected override IAssignSpecification<CyprusBranchOfficeDomainEntityDto, BranchOffice> GetAssignSpecification()
        {
            return BranchOfficeFlexSpecs.BranchOffices.Cyprus.Assign.Entity();
        }
    }
}
