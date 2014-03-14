﻿using System;
using System.Linq;

using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetOrganizationUnitDtoService : GetDomainEntityDtoServiceBase<OrganizationUnit>
    {
        private readonly ISecureFinder _secureFinder;
        private readonly IFinder _finder;
        private readonly IAPIIdentityServiceSettings _identityServiceSettings;

        public GetOrganizationUnitDtoService(IUserContext userContext, ISecureFinder secureFinder, IFinder finder, IAPIIdentityServiceSettings identityServiceSettings) : base(userContext)
        {
            _secureFinder = secureFinder;
            _finder = finder;
            _identityServiceSettings = identityServiceSettings;
        }

        protected override IDomainEntityDto<OrganizationUnit> GetDto(long entityId)
        {
            var dto = _secureFinder.Find<OrganizationUnit>(x => x.Id == entityId)
                          .Select(entity => new OrganizationUnitDomainEntityDto
                              {
                                  Id = entity.Id,
                                  Code = entity.Code,
                                  Name = entity.Name,
                                  DgppId = entity.DgppId,
                                  CountryRef = new EntityReference { Id = entity.CountryId, Name = entity.Country.Name },
                                  FirstEmitDate = entity.FirstEmitDate,
                                  ErmLaunchDate = entity.ErmLaunchDate,
                                  InfoRussiaLaunchDate = entity.InfoRussiaLaunchDate,
                                  SyncCode1C = entity.SyncCode1C,
                                  TimeZoneRef = new EntityReference { Id = entity.TimeZoneId, Name = null },
                                  ElectronicMedia = entity.ElectronicMedia,
                                  Timestamp = entity.Timestamp,
                                  CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                  CreatedOn = entity.CreatedOn,
                                  IsActive = entity.IsActive,
                                  IsDeleted = entity.IsDeleted,
                                  ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                  ModifiedOn = entity.ModifiedOn
                              })
                          .Single();

            dto.TimeZoneRef.Name = _finder.Find<DoubleGis.Erm.Platform.Model.Entities.Security.TimeZone>(tz => tz.Id == dto.TimeZoneRef.Id).Select(tz => tz.TimeZoneId).FirstOrDefault();

            return dto;
        }

        protected override IDomainEntityDto<OrganizationUnit> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new OrganizationUnitDomainEntityDto
                {
                    FirstEmitDate = DateTime.UtcNow,
                    IdentityServiceUrl = _identityServiceSettings.RestUrl
                };
        }
    }
}