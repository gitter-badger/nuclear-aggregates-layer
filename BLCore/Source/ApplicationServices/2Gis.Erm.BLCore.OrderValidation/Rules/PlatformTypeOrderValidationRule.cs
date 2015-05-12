using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;

using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class PlatformTypeOrderValidationRule : OrderValidationRuleBase<OrdinaryValidationRuleContext>
    {
        private readonly IFinder _finder;

        public PlatformTypeOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(OrdinaryValidationRuleContext ruleContext)
        {
            var ordersWithErrors = _finder.Find(ruleContext.OrdersFilterPredicate)
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