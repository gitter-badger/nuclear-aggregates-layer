using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

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

            var pricePosition = _finder.FindOne(Specs.Find.ById<PricePosition>(dto.Id)) 
                ?? new PricePosition { IsActive = true };

            pricePosition.PriceId = dto.PriceRef.Id.Value;
            pricePosition.PositionId = dto.PositionRef.Id.Value;
            pricePosition.Cost = dto.Cost;
            pricePosition.Amount = dto.Amount;
            pricePosition.AmountSpecificationMode = dto.AmountSpecificationMode;
            pricePosition.RateType = dto.RateType;
            pricePosition.MinAdvertisementAmount = dto.MinAdvertisementAmount;
            pricePosition.MaxAdvertisementAmount = dto.MaxAdvertisementAmount;
            pricePosition.Timestamp = dto.Timestamp;

            return pricePosition;
        }
    }
}