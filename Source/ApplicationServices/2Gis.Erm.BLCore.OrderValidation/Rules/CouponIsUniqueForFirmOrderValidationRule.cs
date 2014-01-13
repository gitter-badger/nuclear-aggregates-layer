using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class CouponIsUniqueForFirmOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        // Выгодные покупки с 2ГИС
        private const int BargainWith2GisPositionCategoryId = 14;
        private readonly IFinder _finder;

        public CouponIsUniqueForFirmOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IList<OrderValidationMessage> messages)
        {
            var predicate = filterPredicate;
            long? firmId = null;
            if (!IsCheckMassive)
            {
                if (request.OrderId == null)
                {
                    throw new ArgumentNullException("request.OrderId");
                }

                long organizationUnitId;
                predicate = GetFilterPredicateToGetLinkedOrders(_finder, request.OrderId.Value, out organizationUnitId, out firmId);
            }

            var couponFails = _finder.Find(predicate)
            .Where(x => IsCheckMassive || (firmId.HasValue && x.FirmId == firmId.Value))
            .SelectMany(x => x.OrderPositions)
            .Where(x => x.IsActive && !x.IsDeleted)
            .SelectMany(x => x.OrderPositionAdvertisements)
            .Where(x => x.Position.CategoryId == BargainWith2GisPositionCategoryId && x.AdvertisementId.HasValue && x.AdvertisementId != x.Advertisement.AdvertisementTemplate.DummyAdvertisementId)
            .GroupBy(x => new
            {
                // advertidement description
                AdvertisementId = x.AdvertisementId.Value,
                AdvertisementName = x.Advertisement.Name,
                x.OrderPosition.Order.FirmId
            },
            x => new
            {
                // order position description
                x.OrderPositionId,
                OrderPositionName = x.Position.Name,

                // order description
                x.OrderPosition.OrderId,
                OrderNumber = x.OrderPosition.Order.Number,
            })
            .Where(x => x.Count() > 1)
            .ToArray();

            foreach (var couponFail in couponFails)
            {
                var advertisementDescription = GenerateDescription(EntityName.Advertisement, couponFail.Key.AdvertisementName, couponFail.Key.AdvertisementId);

                var orderPositions = string.Join(", ", couponFail.Select(x => GenerateDescription(EntityName.OrderPosition, x.OrderPositionName, x.OrderPositionId)));

                messages.Add(new OrderValidationMessage
                {
                    Type = MessageType.Error,
                    OrderId = couponFail.First().OrderId,
                    OrderNumber = couponFail.First().OrderNumber,

                    MessageText = string.Format(CultureInfo.CurrentCulture, BLResources.CouponIsBoundToMultiplePositionTemplate, advertisementDescription, orderPositions),
                });
            }
        }
    }
}