using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Modify.DomainEntityObtainers
{
    public sealed class AdvertisementElementStatusObtainer : IBusinessModelEntityObtainer<AdvertisementElementStatus>, IAggregateReadModel<Advertisement>, IRussiaAdapted
    {
        private readonly IFinder _finder;

        public AdvertisementElementStatusObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public AdvertisementElementStatus ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (AdvertisementElementStatusDomainEntityDto)domainEntityDto;
            var entity = _finder.FindOne(Specs.Find.ById<AdvertisementElementStatus>(dto.Id)) ?? new AdvertisementElementStatus { Id = dto.Id };

            entity.Timestamp = dto.Timestamp;

            return entity;
        }
    }
}
