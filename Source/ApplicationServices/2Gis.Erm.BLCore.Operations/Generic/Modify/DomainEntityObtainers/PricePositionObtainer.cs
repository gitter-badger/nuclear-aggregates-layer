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
    public sealed class PricePositionObtainer : IBusinessModelEntityObtainer<PricePosition>, IAggregateReadModel<Price>
    {
        private readonly IFinder _finder;

        public PricePositionObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public PricePosition ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (PricePositionDomainEntityDto)domainEntityDto;

            var pricePosition = dto.Id == 0
                                    ? new PricePosition { IsActive = true }
                                    : _finder.Find(Specs.Find.ById<PricePosition>(dto.Id)).Single();

            pricePosition.PriceId = dto.PriceRef.Id.Value;
            pricePosition.PositionId = dto.PositionRef.Id.Value;
            pricePosition.Cost = dto.Cost;
            pricePosition.Amount = dto.Amount;
            pricePosition.AmountSpecificationMode = (int)dto.AmountSpecificationMode;
            pricePosition.RateType = (int)dto.RateType;
            pricePosition.MinAdvertisementAmount = dto.MinAdvertisementAmount;
            pricePosition.MaxAdvertisementAmount = dto.MaxAdvertisementAmount;
            pricePosition.Timestamp = dto.Timestamp;

            return pricePosition;
        }
    }
}