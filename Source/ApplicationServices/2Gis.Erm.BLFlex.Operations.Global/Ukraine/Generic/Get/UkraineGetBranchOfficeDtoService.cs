﻿using DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.ReadModel;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Ukraine;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get;
using DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Modify;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Get
{
    public class UkraineGetBranchOfficeDtoService : GetBranchOfficeDtoServiceBase<UkraineBranchOfficeDomainEntityDto>, IUkraineAdapted
    {
        public UkraineGetBranchOfficeDtoService(IUserContext userContext, 
            IBranchOfficeReadModel readModel, 
            IBargainTypeReadModel bargainTypeReadModel,
            IContributionTypeReadModel contributionTypeReadModel,
            IAPIIdentityServiceSettings identityServiceSettings)
            : base(userContext, readModel, bargainTypeReadModel, contributionTypeReadModel, identityServiceSettings)
        {
        }

        protected override IProjectSpecification<BranchOffice, UkraineBranchOfficeDomainEntityDto> GetProjectSpecification()
                {
            return BranchOfficeFlexSpecs.BranchOffices.Ukraine.Project.DomainEntityDto();
        }
    }
}