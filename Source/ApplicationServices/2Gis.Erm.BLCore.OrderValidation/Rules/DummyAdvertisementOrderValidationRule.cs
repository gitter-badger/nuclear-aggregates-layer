﻿using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверка на наличие заглушек в заказе
    /// </summary>
    public sealed class DummyAdvertisementOrderValidationRule : OrderValidationRuleBase<HybridParamsValidationRuleContext>
    {
        private readonly IFinder _finder;

        public DummyAdvertisementOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(HybridParamsValidationRuleContext ruleContext)
        {
            var dummyAdvertisementsIds =
                _finder.Find<AdvertisementTemplate>(x => !x.IsDeleted && x.DummyAdvertisementId != null).Select(x => x.DummyAdvertisementId);

            var badAdvertisemements =
                _finder.Find(ruleContext.OrdersFilterPredicate)
                       .SelectMany(order => order.OrderPositions)
                       .Where(orderPosition => orderPosition.IsActive
                                               && !orderPosition.IsDeleted)
                       .SelectMany(
                           orderPosition =>
                           orderPosition.OrderPositionAdvertisements.Where(opa => dummyAdvertisementsIds.Contains(opa.AdvertisementId))
                                        .Select(advertisement => new
                                            {
                                                OrderPositionId = orderPosition.Id,
                                                OrderPositionName = advertisement.Position.Name,
                                                OrderId = orderPosition.Order.Id,
                                                OrderNumber = orderPosition.Order.Number
                                            }))
                       .ToArray();

            return badAdvertisemements.Select(x => new OrderValidationMessage
                                                       {
                                                           Type = ruleContext.ValidationParams.Type == ValidationType.PreReleaseFinal ? MessageType.Error : MessageType.Warning,
                                                           OrderId = x.OrderId,
                                                           OrderNumber = x.OrderNumber,
                                                           MessageText = string.Format(
                                                                             BLResources.OrderContainsDummyAdvertisementError,
                                                                             GenerateDescription(
                                                                                ruleContext.ValidationParams.IsMassValidation,
                                                                                EntityName.OrderPosition,
                                                                                x.OrderPositionName,
                                                                                x.OrderPositionId))
                                                       });
        }
    }
}
