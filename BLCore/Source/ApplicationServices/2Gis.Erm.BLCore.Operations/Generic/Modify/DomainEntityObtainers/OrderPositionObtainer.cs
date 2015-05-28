using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.DomainEntityObtainers
{
    public sealed class OrderPositionObtainer : IBusinessModelEntityObtainer<OrderPosition>, IAggregateReadModel<Order>
    {
        private readonly IFinder _finder;

        public OrderPositionObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public OrderPosition ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (OrderPositionDomainEntityDto)domainEntityDto;

            var orderPosition = _finder.Find(Specs.Find.ById<OrderPosition>(dto.Id)).SingleOrDefault() ?? 
                new OrderPosition { IsActive = true };

            orderPosition.PricePositionId = dto.PricePositionRef.Id.Value;
            orderPosition.Amount = dto.Amount;
            orderPosition.DiscountPercent = dto.DiscountPercent;
            orderPosition.DiscountSum = dto.DiscountSum;
            orderPosition.PayablePrice = dto.PayablePrice;
            orderPosition.PayablePlan = dto.PayablePlan;
            orderPosition.ShipmentPlan = dto.ShipmentPlan;
            orderPosition.OrderId = dto.OrderId;
            orderPosition.Comment = dto.Comment;
            orderPosition.CalculateDiscountViaPercent = dto.CalculateDiscountViaPercent;
            orderPosition.PricePerUnit = dto.PricePerUnit;
            orderPosition.PricePerUnitWithVat = dto.PricePerUnitWithVat;
            orderPosition.Timestamp = dto.Timestamp;

            return orderPosition;
        }
    }
}