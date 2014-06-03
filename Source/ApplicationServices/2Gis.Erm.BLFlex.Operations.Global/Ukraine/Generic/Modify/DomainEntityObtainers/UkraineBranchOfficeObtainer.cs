using DoubleGis.Erm.BLCore.API.Aggregates.Dynamic.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Modify;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Modify.DomainEntityObtainers
{
    public class UkraineBranchOfficeObtainer : DynamicBranchOfficeObtainerBase<UkraineBranchOfficeDomainEntityDto, UkraineBranchOfficePart>, IUkraineAdapted
    {
        public UkraineBranchOfficeObtainer(IFinder finder)
            : base(finder)
        {
        }

        protected override IAssignSpecification<UkraineBranchOfficeDomainEntityDto, BranchOffice> GetAssignSpecification()
        {
            return BranchOfficeFlexSpecs.BranchOffices.Ukraine.Assign.Entity();
        }
    }
}
