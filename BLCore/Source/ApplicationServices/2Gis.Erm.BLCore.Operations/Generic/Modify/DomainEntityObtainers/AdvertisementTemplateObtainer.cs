using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class AdvertisementTemplateObtainer : IBusinessModelEntityObtainer<AdvertisementTemplate>, IAggregateReadModel<Advertisement>
    {
        private readonly IFinder _finder;

        public AdvertisementTemplateObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public AdvertisementTemplate ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (AdvertisementTemplateDomainEntityDto)domainEntityDto;

            var entity = _finder.Find(Specs.Find.ById<AdvertisementTemplate>(dto.Id)).One() ??
                    new AdvertisementTemplate { Id = dto.Id };

            if (dto.Timestamp == null && entity.Timestamp != null)
            {
                throw new BusinessLogicException(string.Format(BLResources.CannotCreateObjectWithSpecifiedId, dto.Id));
            }

            entity.IsAllowedToWhiteList = dto.IsAllowedToWhiteList;
            entity.IsAdvertisementRequired = dto.IsAdvertisementRequired;
            entity.Name = dto.Name;
            entity.Comment = dto.Comment;
            entity.Timestamp = dto.Timestamp;
            return entity;
        }
    }
}