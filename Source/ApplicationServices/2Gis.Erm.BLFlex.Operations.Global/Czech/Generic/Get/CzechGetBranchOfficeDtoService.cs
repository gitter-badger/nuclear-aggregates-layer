﻿using DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Czech;
using DoubleGis.Erm.BLFlex.Operations.Global.Czech.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
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
            IContributionTypeReadModel contributionTypeReadModel,
            IAPIIdentityServiceSettings identityServiceSettings)
            : base(userContext, readModel, bargainTypeReadModel, contributionTypeReadModel, identityServiceSettings)
        {
        }

        protected override IProjectSpecification<BranchOffice, CzechBranchOfficeDomainEntityDto> GetProjectSpecification()
        {
            return BranchOfficeFlexSpecs.BranchOffices.Czech.Project.DomainEntityDto();
        }
    }
}
