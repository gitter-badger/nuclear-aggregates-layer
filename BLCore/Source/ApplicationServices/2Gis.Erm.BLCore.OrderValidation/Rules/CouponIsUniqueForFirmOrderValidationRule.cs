using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Storage.Readings;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class CouponIsUniqueForFirmOrderValidationRule : OrderValidationRuleBase<HybridParamsValidationRuleContext>
    {
        // Выгодные покупки с 2ГИС
        private const int BargainWith2GisPositionCategoryId = 14;
        private readonly IQuery _query;

        public CouponIsUniqueForFirmOrderValidationRule(IQuery query)
        {
            _query = query;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(HybridParamsValidationRuleContext ruleContext)
        {
            var predicate = ruleContext.OrdersFilterPredicate;
            long? firmId = null;
            if (!ruleContext.ValidationParams.IsMassValidation)
            {
                long organizationUnitId;
                predicate = GetFilterPredicateToGetLinkedOrders(_query, ruleContext.ValidationParams.Single.OrderId, out organizationUnitId, out firmId);
            }

            var couponFails = _query.For<Order>()
                                    .Where(predicate)
                                    .Where(x => ruleContext.ValidationParams.IsMassValidation || (firmId.HasValue && x.FirmId == firmId.Value))
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

            return couponFails.Select(x => new OrderValidationMessage
                {
                    Type = MessageType.Error,
                    OrderId = x.First().OrderId,
                    OrderNumber = x.First().OrderNumber,

                    MessageText = 
                        string.Format(
                            BLResources.CouponIsBoundToMultiplePositionTemplate,
                            GenerateDescription(ruleContext.ValidationParams.IsMassValidation, EntityType.Instance.Advertisement(), x.Key.AdvertisementName, x.Key.AdvertisementId),
                            string.Join(", ", x.Select(p => GenerateDescription(ruleContext.ValidationParams.IsMassValidation, EntityType.Instance.OrderPosition(), p.OrderPositionName, p.OrderPositionId)))),
                });
        }
    }
}