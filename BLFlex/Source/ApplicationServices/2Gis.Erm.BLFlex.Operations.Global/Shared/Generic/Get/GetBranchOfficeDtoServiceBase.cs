﻿using System;

using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.BargainTypes.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.ContributionTypes.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Generic.Get
{
    // FIXME {d.ivanov, 17.06.2014}: Убрать этот базовый класс, см. комментарии к GetLegalPersonDtoServiceBase
    [Obsolete]
    public abstract class GetBranchOfficeDtoServiceBase<TDto> : GetDomainEntityDtoServiceBase<BranchOffice>
        where TDto : IDomainEntityDto<BranchOffice>, new()
    {
        private readonly IBranchOfficeReadModel _readModel;
        private readonly IBargainTypeReadModel _bargainTypeReadModel;
        private readonly IContributionTypeReadModel _contributionTypeReadModel;
        private readonly IAPIIdentityServiceSettings _identityServiceSettings;

        protected GetBranchOfficeDtoServiceBase(
            IUserContext userContext,
            IBranchOfficeReadModel readModel,
            IBargainTypeReadModel bargainTypeReadModel,
            IContributionTypeReadModel contributionTypeReadModel,
            IAPIIdentityServiceSettings identityServiceSettings)
            : base(userContext)
        {
            _readModel = readModel;
            _bargainTypeReadModel = bargainTypeReadModel;
            _contributionTypeReadModel = contributionTypeReadModel;
            _identityServiceSettings = identityServiceSettings;
        }

        protected override IDomainEntityDto<BranchOffice> GetDto(long entityId)
        {
            var branchOffice = _readModel.GetBranchOffice(entityId);

            var dto = GetProjectSpecification().Project(branchOffice);

            var bargainTypeRef = dto.GetPropertyValue<TDto, EntityReference>("BargainTypeRef");
            if (bargainTypeRef.Id.HasValue)
            {
                bargainTypeRef.Name = _bargainTypeReadModel.GetBargainTypeName(bargainTypeRef.Id.Value);
            }

            var contributionTypeRef = dto.GetPropertyValue<TDto, EntityReference>("ContributionTypeRef");
            if (contributionTypeRef.Id.HasValue)
            {
                contributionTypeRef.Name = _contributionTypeReadModel.GetContributionTypeName(contributionTypeRef.Id.Value);
            }

            return dto;
        }

        protected abstract IProjectSpecification<BranchOffice, TDto> GetProjectSpecification();

        protected override IDomainEntityDto<BranchOffice> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            var dto = new TDto();

            dto.SetPropertyValue("IdentityServiceUrl", _identityServiceSettings.RestUrl);

            return dto;
        }
    }
}
