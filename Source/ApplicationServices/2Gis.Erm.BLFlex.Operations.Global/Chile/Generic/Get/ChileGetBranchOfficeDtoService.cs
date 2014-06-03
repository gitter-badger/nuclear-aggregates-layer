using DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Get
{
    public class ChileGetBranchOfficeDtoService : GetBranchOfficeDtoServiceBase<ChileBranchOfficeDomainEntityDto>, IChileAdapted
    {
        public ChileGetBranchOfficeDtoService(
            IUserContext userContext,
            IBranchOfficeReadModel readModel,
            IBargainTypeReadModel bargainTypeReadModel,
            IContributionTypeReadModel contributionTypeReadModel,
            IAPIIdentityServiceSettings identityServiceSettings)
            : base(userContext, readModel, bargainTypeReadModel, contributionTypeReadModel, identityServiceSettings)
        {
        }

        protected override IProjectSpecification<BranchOffice, ChileBranchOfficeDomainEntityDto> GetProjectSpecification()
        {
            return BranchOfficeFlexSpecs.BranchOffices.Chile.Project.DomainEntityDto();
        }
    }
}
