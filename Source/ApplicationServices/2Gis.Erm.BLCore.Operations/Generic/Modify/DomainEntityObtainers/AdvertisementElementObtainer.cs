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

            var advertisementElement = _finder.FindOne(Specs.Find.ById<AdvertisementElement>(dto.Id)) 
                ?? new AdvertisementElement();

            advertisementElement.Error = (int)dto.Error;
            advertisementElement.Status = (int)dto.Status;
            advertisementElement.FileId = dto.FileId;
            advertisementElement.BeginDate = dto.BeginDate;
            advertisementElement.EndDate = dto.EndDate;
            advertisementElement.FasCommentType = (int?)dto.FasCommentType;
            advertisementElement.Timestamp = dto.Timestamp;
            return advertisementElement;
        }
    }
}