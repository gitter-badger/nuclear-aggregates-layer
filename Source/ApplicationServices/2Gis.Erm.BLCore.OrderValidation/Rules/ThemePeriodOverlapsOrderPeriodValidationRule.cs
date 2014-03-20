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
    public sealed class ThemePeriodOverlapsOrderPeriodValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public ThemePeriodOverlapsOrderPeriodValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            var invalidOrders = _finder.Find(filterPredicate)
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

            foreach (var invalid in invalidOrders)
            {
                var orderLabel = GenerateDescription(EntityName.Order, invalid.Order.Number, invalid.Order.Id);
                foreach (var theme in invalid.InvalidThemes)
                {
                    var themeLabel = GenerateDescription(EntityName.Theme, theme.Name, theme.Id);
                    var message = new OrderValidationMessage
                    {
                        OrderNumber = invalid.Order.Number,
                        OrderId = invalid.Order.Id,
                        Type = MessageType.Error,
                        MessageText = string.Format(BLResources.ThemePeriodDoesNotOverlapOrderPeriod, orderLabel, themeLabel),
                    };

                    messages.Add(message);
                }
            }
        }
    }
}
