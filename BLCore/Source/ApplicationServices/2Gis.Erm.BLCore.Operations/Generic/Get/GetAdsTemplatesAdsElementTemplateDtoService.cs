using System.Linq;

using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
    {
    public class GetAdsTemplatesAdsElementTemplateDtoService : GetDomainEntityDtoServiceBase<AdsTemplatesAdsElementTemplate>
    {
        private readonly ISecureFinder _finder;

        public GetAdsTemplatesAdsElementTemplateDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<AdsTemplatesAdsElementTemplate> CreateDto(long? parentEntityId, IEntityType parentEntityName, string extendedInfo)
        {
            if (parentEntityName.Equals(EntityType.Instance.AdvertisementTemplate()))
            {
                return _finder.Find<AdvertisementTemplate>(x => x.Id == parentEntityId)
                              .Select(x => new AdsTemplatesAdsElementTemplateDomainEntityDto
                                  {
                                      AdsTemplateRef = new EntityReference { Id = x.Id, Name = x.Name }
                                  })
                              .Single();
            }

            if (parentEntityName.Equals(EntityType.Instance.AdvertisementElementTemplate()))
            {
                return _finder.Find<AdvertisementElementTemplate>(x => x.Id == parentEntityId)
                              .Select(x => new AdsTemplatesAdsElementTemplateDomainEntityDto
                                  {
                                      AdsElementTemplateRef = new EntityReference { Id = x.Id, Name = x.Name }
                                  })
                              .Single();
            }

            return new AdsTemplatesAdsElementTemplateDomainEntityDto();
        }

        protected override IDomainEntityDto<AdsTemplatesAdsElementTemplate> GetDto(long entityId)
        {
            return _finder.Find<AdsTemplatesAdsElementTemplate>(x => x.Id == entityId)
                          .Select(x => new AdsTemplatesAdsElementTemplateDomainEntityDto
                              {
                                  Id = x.Id,
                                  AdsTemplateRef = new EntityReference { Id = x.AdsTemplateId, Name = x.AdvertisementTemplate.Name },
                                  AdsElementTemplateRef = new EntityReference { Id = x.AdsElementTemplateId, Name = x.AdvertisementElementTemplate.Name },
                                  ExportCode = x.ExportCode,
                                  Timestamp = x.Timestamp,
                                  CreatedByRef = new EntityReference { Id = x.CreatedBy, Name = null },
                                  CreatedOn = x.CreatedOn,
                                  IsDeleted = x.IsDeleted,
                                  ModifiedByRef = new EntityReference { Id = x.ModifiedBy, Name = null },
                                  ModifiedOn = x.ModifiedOn
                              })
                          .Single();
        }
    }
}