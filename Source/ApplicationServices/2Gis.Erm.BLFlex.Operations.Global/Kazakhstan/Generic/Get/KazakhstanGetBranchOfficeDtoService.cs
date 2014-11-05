﻿using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.BargainTypes.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.SimplifiedModel.ContributionTypes.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Kazakhstan;
using DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Generic.Modify;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Core.EntityProjection;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Kazakhstan.Generic.Get
{
    public class KazakhstanGetBranchOfficeDtoService : GetDomainEntityDtoServiceBase<BranchOffice>, IKazakhstanAdapted
    {
        private readonly IBranchOfficeReadModel _readModel;
        private readonly IBargainTypeReadModel _bargainTypeReadModel;
        private readonly IContributionTypeReadModel _contributionTypeReadModel;
        private readonly IProjectSpecification<BranchOffice, KazakhstanBranchOfficeDomainEntityDto> _specification;
        private readonly IAPIIdentityServiceSettings _identityServiceSettings;

        public KazakhstanGetBranchOfficeDtoService(IUserContext userContext,
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
            _specification = BranchOfficeFlexSpecs.BranchOffices.Kazakhstan.Project.DomainEntityDto();
        }

        protected override IDomainEntityDto<BranchOffice> GetDto(long entityId)
        {
            var entity = _readModel.GetBranchOffice(entityId);
            var dto = _specification.Project(entity);

            if (dto.BargainTypeRef.Id.HasValue)
            {
                dto.BargainTypeRef.Name = _bargainTypeReadModel.GetBargainTypeName(dto.BargainTypeRef.Id.Value);
            }

            if (dto.ContributionTypeRef.Id.HasValue)
            {
                dto.ContributionTypeRef.Name = _contributionTypeReadModel.GetContributionTypeName(dto.ContributionTypeRef.Id.Value);
            }

            return dto;
        }

        protected override IDomainEntityDto<BranchOffice> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new KazakhstanBranchOfficeDomainEntityDto
                       {
                           IdentityServiceUrl = _identityServiceSettings.RestUrl
                       };
        }
    }
}