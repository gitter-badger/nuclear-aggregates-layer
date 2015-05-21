using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Storage;
using NuClear.Storage.Specifications;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверка на наличие заглушек в заказе
    /// </summary>
    public sealed class DummyAdvertisementOrderValidationRule : OrderValidationRuleBase<HybridParamsValidationRuleContext>
    {
        private readonly IQuery _query;
        private readonly IFinder _finder;

        public DummyAdvertisementOrderValidationRule(IQuery query, IFinder finder)
        {
            _query = query;
            _finder = finder;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(HybridParamsValidationRuleContext ruleContext)
        {
            var dummyAdvertisementsIds =
                _finder.FindMany(new SelectSpecification<AdvertisementTemplate, long?>(x => x.DummyAdvertisementId),
                                 new FindSpecification<AdvertisementTemplate>(x => !x.IsDeleted && x.DummyAdvertisementId != null));

            var badAdvertisemements =
                _query.For<Order>()
                       .Where(ruleContext.OrdersFilterPredicate)
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
                                                                                EntityType.Instance.OrderPosition(),
                                                                                x.OrderPositionName,
                                                                                x.OrderPositionId))
                                                       });
        }
    }
}
