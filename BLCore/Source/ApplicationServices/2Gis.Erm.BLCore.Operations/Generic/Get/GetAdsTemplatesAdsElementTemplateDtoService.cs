using System.Linq;

using NuClear.Security.API.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
    {
    public class GetAdsTemplatesAdsElementTemplateDtoService : GetDomainEntityDtoServiceBase<AdsTemplatesAdsElementTemplate>
    {
        private readonly ISecureFinder _finder;

        public GetAdsTemplatesAdsElementTemplateDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<AdsTemplatesAdsElementTemplate> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            switch (parentEntityName)
            {
                case EntityName.AdvertisementTemplate:
                    return _finder.Find<AdvertisementTemplate>(x => x.Id == parentEntityId)
                                  .Select(x => new AdsTemplatesAdsElementTemplateDomainEntityDto
                                      {
                                          AdsTemplateRef = new EntityReference { Id = x.Id, Name = x.Name }
                                      }).Single();

                case EntityName.AdvertisementElementTemplate:
                    return _finder.Find<AdvertisementElementTemplate>(x => x.Id == parentEntityId)
                                  .Select(x => new AdsTemplatesAdsElementTemplateDomainEntityDto
                                      {
                                          AdsElementTemplateRef = new EntityReference { Id = x.Id, Name = x.Name }
                                      }).Single();

                default:
                    return new AdsTemplatesAdsElementTemplateDomainEntityDto();
            }
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