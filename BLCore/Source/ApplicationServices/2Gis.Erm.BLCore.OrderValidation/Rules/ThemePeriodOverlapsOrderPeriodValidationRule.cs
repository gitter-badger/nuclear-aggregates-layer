using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class ThemePeriodOverlapsOrderPeriodValidationRule : OrderValidationRuleBase<OrdinaryValidationRuleContext>
    {
        private readonly IFinder _finder;

        public ThemePeriodOverlapsOrderPeriodValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(OrdinaryValidationRuleContext ruleContext)
        {
            var invalidOrders = _finder.Find(ruleContext.OrdersFilterPredicate)
                .Select(order => new
                    {
                        Order = order,
                        InvalidThemes = order.OrderPositions
                            .Where(position => position.IsActive && !position.IsDeleted)
                            .SelectMany(position => position.OrderPositionAdvertisements
                                .Select(advertisement => advertisement.Theme)
                                .Where(theme => theme.BeginDistribution > order.BeginDistributionDate || theme.EndDistribution < order.EndDistributionDateFact))
                    })
                .Where(info => info.InvalidThemes.Any())
                .ToArray();

            return from invalidOrder in invalidOrders
                   from theme in invalidOrder.InvalidThemes
                   select new OrderValidationMessage
                              {
                                  OrderNumber = invalidOrder.Order.Number,
                                  OrderId = invalidOrder.Order.Id,
                                  Type = MessageType.Error,
                                  MessageText =
                                      string.Format(BLResources.ThemePeriodDoesNotOverlapOrderPeriod,
                                                    GenerateDescription(ruleContext.IsMassValidation, EntityType.Instance.Order(), invalidOrder.Order.Number, invalidOrder.Order.Id),
                                                    GenerateDescription(ruleContext.IsMassValidation, EntityType.Instance.Theme(), theme.Name, theme.Id)),
                              };
        }
    }
}
