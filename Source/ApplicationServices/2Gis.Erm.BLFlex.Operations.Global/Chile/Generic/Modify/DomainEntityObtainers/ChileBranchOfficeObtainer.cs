using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Modify;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify.DomainEntityObtainers
{
    public class ChileBranchOfficeObtainer : BranchOfficeObtainerBase<ChileBranchOfficeDomainEntityDto, BranchOffice>, IChileAdapted
    {
        public ChileBranchOfficeObtainer(IFinder finder)
            : base(finder)
        {
        }

        protected override IAssignSpecification<ChileBranchOfficeDomainEntityDto, BranchOffice> GetAssignSpecification()
        {
            return BranchOfficeFlexSpecs.BranchOffices.Chile.Assign.Entity();
        }
    }
}
