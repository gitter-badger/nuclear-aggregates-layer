using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Storage;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// В ERM дорабатываем запрет возможности продажи на пустой адрес.
    /// Необходимо дополнительный атруб от IR по которому мы будем определять что адрес пустой.
    /// Отображаться в списке доступных объектов привязки данный адрес не должен.
    /// </summary>
    public sealed class IsAdvertisementLinkedWithLocatedOnTheMapAddressOrderValidationRule : OrderValidationRuleBase<OrdinaryValidationRuleContext>
    {
        private readonly IQuery _query;

        public IsAdvertisementLinkedWithLocatedOnTheMapAddressOrderValidationRule(IQuery query)
        {
            _query = query;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(OrdinaryValidationRuleContext ruleContext)
        {
            var badAdvertisemements =
                    _query.For<Order>()
                           .Where(ruleContext.OrdersFilterPredicate)
                           .SelectMany(order => order.OrderPositions)
                           .Where(orderPosition => orderPosition.IsActive
                                                   && !orderPosition.IsDeleted)
                           .SelectMany(
                               orderPosition =>
                               orderPosition.OrderPositionAdvertisements.Where(opa => !opa.FirmAddress.IsLocatedOnTheMap
                                                                                      && opa.FirmAddress.Firm.OrganizationUnit.InfoRussiaLaunchDate != null
                                                                                      && opa.Position.CategoryId != 11
                                                                                      && opa.Position.CategoryId != 14
                                                                                      && opa.Position.CategoryId != 26)
                                            .Select(advertisement => new
                                                {
                                                    OrderPositionId = orderPosition.Id,
                                                    OrderPositionName = advertisement.Position.Name,
                                                    OrderId = orderPosition.Order.Id,
                                                    OrderNumber = orderPosition.Order.Number,
                                                    FirmAddressId = advertisement.FirmAddress.Id,
                                                    FirmAddressName = advertisement.FirmAddress.Address,
                                                }))
                           .ToArray();

            return badAdvertisemements.Select(x => new OrderValidationMessage
                                                       {
                                                           Type = MessageType.Error,
                                                           OrderId = x.OrderId,
                                                           OrderNumber = x.OrderNumber,
                                                           MessageText =
                                                               string.Format(BLResources.AdvertisementIsLinkedWithEmptyAddressError,
                                                                             GenerateDescription(ruleContext.IsMassValidation,
                                                                                                 EntityType.Instance.OrderPosition(),
                                                                                                 x.OrderPositionName,
                                                                                                 x.OrderPositionId),
                                                                             GenerateDescription(ruleContext.IsMassValidation,
                                                                                                 EntityType.Instance.FirmAddress(),
                                                                                                 x.FirmAddressName,
                                                                                                 x.FirmAddressId))
                                                       });
        }
    }
}
