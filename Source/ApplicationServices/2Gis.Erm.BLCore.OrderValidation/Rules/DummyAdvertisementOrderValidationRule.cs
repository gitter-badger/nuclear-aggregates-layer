using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Проверка на наличие заглушек в заказе
    /// </summary>
    public sealed class DummyAdvertisementOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public DummyAdvertisementOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IList<OrderValidationMessage> messages)
        {
            var dummyAdvertisementsIds =
                _finder.Find<AdvertisementTemplate>(x => !x.IsDeleted && x.DummyAdvertisementId != null).Select(x => x.DummyAdvertisementId);

            var badAdvertisemements =
                _finder.Find(filterPredicate)
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

            foreach (var advertisemement in badAdvertisemements)
            {
                var orderPositionDescription = GenerateDescription(EntityName.OrderPosition, advertisemement.OrderPositionName, advertisemement.OrderPositionId);

                messages.Add(
                    new OrderValidationMessage
                        {
                            Type = request.Type == ValidationType.PreReleaseFinal ? MessageType.Error : MessageType.Warning,
                            OrderId = advertisemement.OrderId,
                            OrderNumber = advertisemement.OrderNumber,
                            MessageText = string.Format(BLResources.OrderContainsDummyAdvertisementError, orderPositionDescription)
                        });
            }
        }
    }
}
