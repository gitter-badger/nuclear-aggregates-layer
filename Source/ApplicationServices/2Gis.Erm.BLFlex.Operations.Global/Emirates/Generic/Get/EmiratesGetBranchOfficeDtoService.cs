﻿using DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates;
using DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Get
{
    public class EmiratesGetBranchOfficeDtoService : GetBranchOfficeDtoServiceBase<EmiratesBranchOfficeDomainEntityDto>, IEmiratesAdapted
    {
        public EmiratesGetBranchOfficeDtoService(
            IUserContext userContext,
            IBranchOfficeReadModel readModel,
            IBargainTypeReadModel bargainTypeReadModel,
            IContributionTypeReadModel contributionTypeReadModel,
            IAPIIdentityServiceSettings identityServiceSettings)
            : base(userContext, readModel, bargainTypeReadModel, contributionTypeReadModel, identityServiceSettings)
        {
        }

        protected override IProjectSpecification<BranchOffice, EmiratesBranchOfficeDomainEntityDto> GetProjectSpecification()
        {
            return BranchOfficeFlexSpecs.BranchOffices.Emirates.Project.DomainEntityDto();
        }
    }
}