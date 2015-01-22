﻿using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Metadata.Settings;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetAdvertisementTemplateDtoService : GetDomainEntityDtoServiceBase<AdvertisementTemplate>
    {
        private readonly ISecureFinder _finder;
        private readonly IAPIIdentityServiceSettings _identityServiceSettings;

        public GetAdvertisementTemplateDtoService(IUserContext userContext, ISecureFinder finder, IAPIIdentityServiceSettings identityServiceSettings)
            : base(userContext)
        {
            _finder = finder;
            _identityServiceSettings = identityServiceSettings;
        }

        protected override IDomainEntityDto<AdvertisementTemplate> GetDto(long entityId)
        {
            return _finder.Find<AdvertisementTemplate>(x => x.Id == entityId)
                          .Select(entity => new AdvertisementTemplateDomainEntityDto
                              {
                                  Id = entity.Id,
                                  IsAllowedToWhiteList = entity.IsAllowedToWhiteList,
                                  HasActiveAdvertisement = entity.Advertisements.Any(a => !a.IsDeleted),
                                  Comment = entity.Comment,
                                  Name = entity.Name,
                                  IsAdvertisementRequired = entity.IsAdvertisementRequired,
                                  DummyAdvertisementRef = new EntityReference { Id = entity.DummyAdvertisementId, Name = BLResources.DummyValue },
                                  IsPublished = entity.IsPublished,
                                  Timestamp = entity.Timestamp,
                                  CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                                  CreatedOn = entity.CreatedOn,
                                  IsDeleted = entity.IsDeleted,
                                  ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                                  ModifiedOn = entity.ModifiedOn
                              })
                          .Single();
        }

        protected override IDomainEntityDto<AdvertisementTemplate> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            return new AdvertisementTemplateDomainEntityDto
                {
                    IdentityServiceUrl = _identityServiceSettings.RestUrl
                };
        }
    }
}