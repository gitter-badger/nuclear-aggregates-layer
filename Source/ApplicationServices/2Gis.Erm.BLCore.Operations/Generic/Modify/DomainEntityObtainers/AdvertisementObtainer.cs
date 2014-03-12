using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class AdvertisementObtainer : IBusinessModelEntityObtainer<Advertisement>, IAggregateReadModel<Advertisement>
    {
        private readonly IFinder _finder;

        public AdvertisementObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public Advertisement ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (AdvertisementDomainEntityDto)domainEntityDto;

            var advertisement =
                dto.Id == 0
                    ? new Advertisement()
                    : _finder.Find(Specs.Find.ById<Advertisement>(dto.Id)).Single();

            advertisement.FirmId = dto.FirmRef.Id.Value;
            advertisement.AdvertisementTemplateId = dto.AdvertisementTemplateRef.Id.Value;
            advertisement.Name = dto.Name;
            advertisement.Comment = dto.Comment;
            advertisement.Timestamp = dto.Timestamp;
            return advertisement;
        }
    }
}