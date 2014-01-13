﻿using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Settings.APIServices;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetTerritoryDtoService : GetDomainEntityDtoServiceBase<Territory>
    {
        private readonly ISecureFinder _finder;
        private readonly IAPIIdentityServiceSettings _identityServiceSettings;

        public GetTerritoryDtoService(IUserContext userContext, ISecureFinder finder, IAPIIdentityServiceSettings identityServiceSettings) : base(userContext)
        {
            _finder = finder;
            _identityServiceSettings = identityServiceSettings;
        }

        protected override IDomainEntityDto<Territory> GetDto(long entityId)
        {
            return _finder.Find<Territory>(x => x.Id == entityId)
                          .Select(entity => new TerritoryDomainEntityDto
                              {
                                  Id = entity.Id,
                                  Name = entity.Name,
                                  OrganizationUnitRef = new EntityReference { Id = entity.OrganizationUnitId, Name = entity.OrganizationUnit.Name },
                                  Timestamp = entity.Timestamp,
                                  CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                  CreatedOn = entity.CreatedOn,
                                  IsActive = entity.IsActive,
                                  ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                  ModifiedOn = entity.ModifiedOn
                              })
                          .Single();
        }

        protected override IDomainEntityDto<Territory> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new TerritoryDomainEntityDto
                {
                    IdentityServiceUrl = _identityServiceSettings.RestUrl
                };
        }
    }
}