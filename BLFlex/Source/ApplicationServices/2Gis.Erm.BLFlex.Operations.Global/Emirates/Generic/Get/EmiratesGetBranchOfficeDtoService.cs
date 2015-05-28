﻿using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.BargainTypes.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.ContributionTypes.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates;
using DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;
using NuClear.IdentityService.Client.Settings;
using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Emirates.Generic.Get
{
    public class EmiratesGetBranchOfficeDtoService : GetBranchOfficeDtoServiceBase<EmiratesBranchOfficeDomainEntityDto>, IEmiratesAdapted
    {
        public EmiratesGetBranchOfficeDtoService(
            IUserContext userContext,
            IBranchOfficeReadModel readModel,
            IBargainTypeReadModel bargainTypeReadModel,
            IContributionTypeReadModel contributionTypeReadModel)
            : base(userContext, readModel, bargainTypeReadModel, contributionTypeReadModel)
        {
        }

        protected override MapSpecification<BranchOffice, EmiratesBranchOfficeDomainEntityDto> GetProjectSpecification()
        {
            return BranchOfficeFlexSpecs.BranchOffices.Emirates.Project.DomainEntityDto();
        }
    }
}