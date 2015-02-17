using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Get
{
    public class GetAdvertisementTemplateDtoService : GetDomainEntityDtoServiceBase<AdvertisementTemplate>
    {
        private readonly ISecureFinder _finder;

        public GetAdvertisementTemplateDtoService(IUserContext userContext, ISecureFinder finder)
            : base(userContext)
        {
            _finder = finder;
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

        protected override IDomainEntityDto<AdvertisementTemplate> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            return new AdvertisementTemplateDomainEntityDto();
        }
    }
}