using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.BargainTypes.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.ContributionTypes.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Czech;
using DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;
using NuClear.IdentityService.Client.Settings;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic.Get
{
    public class CzechGetBranchOfficeDtoService : GetBranchOfficeDtoServiceBase<CzechBranchOfficeDomainEntityDto>, ICzechAdapted
    {
        public CzechGetBranchOfficeDtoService(
            IUserContext userContext,
            IBranchOfficeReadModel readModel,
            IBargainTypeReadModel bargainTypeReadModel,
            IContributionTypeReadModel contributionTypeReadModel)
            : base(userContext, readModel, bargainTypeReadModel, contributionTypeReadModel)
        {
        }

        protected override IProjectSpecification<BranchOffice, CzechBranchOfficeDomainEntityDto> GetProjectSpecification()
        {
            return BranchOfficeFlexSpecs.BranchOffices.Czech.Project.DomainEntityDto();
        }
    }
}
