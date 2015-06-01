using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Readings;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class AdvertisementElementObtainer : IBusinessModelEntityObtainer<AdvertisementElement>, IAggregateReadModel<Advertisement>
    {
        private readonly IFinder _finder;

        public AdvertisementElementObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public AdvertisementElement ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (AdvertisementElementDomainEntityDto)domainEntityDto;
            var advertisementElement = _finder.Find(Specs.Find.ById<AdvertisementElement>(dto.Id)).One()
                                       ?? new AdvertisementElement();

            advertisementElement.FileId = dto.FileId;
            advertisementElement.BeginDate = dto.BeginDate;
            advertisementElement.EndDate = dto.EndDate;
            advertisementElement.FasCommentType = dto.FasCommentType;
            advertisementElement.Timestamp = dto.Timestamp;

            return advertisementElement;
        }
    }
}