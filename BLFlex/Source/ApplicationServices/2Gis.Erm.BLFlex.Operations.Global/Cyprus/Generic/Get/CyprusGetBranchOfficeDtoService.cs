﻿using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.BargainTypes.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.ContributionTypes.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Cyprus;
using DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Generic.Modify;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;

using NuClear.Security.API.UserContext;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Generic.Get
{
    public class CyprusGetBranchOfficeDtoService : GetBranchOfficeDtoServiceBase<CyprusBranchOfficeDomainEntityDto>, ICyprusAdapted
    {
        public CyprusGetBranchOfficeDtoService(
            IUserContext userContext,
            IBranchOfficeReadModel readModel,
            IBargainTypeReadModel bargainTypeReadModel,
            IContributionTypeReadModel contributionTypeReadModel)
            : base(userContext, readModel, bargainTypeReadModel, contributionTypeReadModel)
        {
        }

        protected override MapSpecification<BranchOffice, CyprusBranchOfficeDomainEntityDto> GetProjectSpecification()
        {
            return BranchOfficeFlexSpecs.BranchOffices.Cyprus.Project.DomainEntityDto();
        }
    }
}
