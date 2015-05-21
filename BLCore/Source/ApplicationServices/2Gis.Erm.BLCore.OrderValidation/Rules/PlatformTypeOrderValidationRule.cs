using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class PlatformTypeOrderValidationRule : OrderValidationRuleBase<OrdinaryValidationRuleContext>
    {
        private readonly IQuery _query;

        public PlatformTypeOrderValidationRule(IQuery query)
        {
            _query = query;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(OrdinaryValidationRuleContext ruleContext)
        {
            var ordersWithErrors = _query.For<Order>()
                                         .Where(ruleContext.OrdersFilterPredicate)
                                         .Select(x => new
                                             {
                                                 x.Id,
                                                 x.Number,
                                                 OrderPositionsAdvertisements =
                                                          x.OrderPositions
                                                           .Where(y => y.IsActive && !y.IsDeleted)
                                                           .SelectMany(y => y.OrderPositionAdvertisements)
                                                           .Select(y => new
                                                               {
                                                                   y.Position.Name,
                                                                   y.Position.Platform.IsSupportedByExport
                                                               })
                                                           .Where(y => !y.IsSupportedByExport)
                                             })
                                         .Where(y => y.OrderPositionsAdvertisements.Any())
                                         .ToArray();

            return from order in ordersWithErrors
                   from orderPositionAdvertisement in order.OrderPositionsAdvertisements
                   select new OrderValidationMessage
                              {
                                  Type = MessageType.Error,
                                  OrderId = order.Id,
                                  OrderNumber = order.Number,
                                  MessageText = string.Format(BLResources.RegisteredPositionIsNotSupportedByExport, orderPositionAdvertisement.Name)
                              };
        }
    }
}