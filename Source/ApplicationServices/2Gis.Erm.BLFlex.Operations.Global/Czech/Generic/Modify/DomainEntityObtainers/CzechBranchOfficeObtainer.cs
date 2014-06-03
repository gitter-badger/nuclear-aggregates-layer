using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Modify;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic.Modify.DomainEntityObtainers
{
    public class CzechBranchOfficeObtainer : BranchOfficeObtainerBase<CzechBranchOfficeDomainEntityDto>, ICzechAdapted
    {
        public CzechBranchOfficeObtainer(IFinder finder)
            : base(finder)
        {
        }

        protected override IAssignSpecification<CzechBranchOfficeDomainEntityDto, BranchOffice> GetAssignSpecification()
        {
            return BranchOfficeFlexSpecs.BranchOffices.Czech.Assign.Entity();
        }
    }
}
