using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class ThemePositionCountValidationRule : OrderValidationRuleCommonPredicate
    {
        private const int MaxPositionsPerTheme = 10;
        private readonly IFinder _finder;

        public ThemePositionCountValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IList<OrderValidationMessage> messages)
        {
            var query = GetThemeFromOrderSelectQuery();
            var filter = IsCheckMassive
                             ? GetMassiveCheckFiler(request.OrganizationUnitId, request.Period)
                             : GetSingleOrderCheckFilter(request.OrderId);
            var oversaleQuery = IsCheckMassive
                                    ? GetMassiveOversaleQuery()
                                    : GetSingleOversaleQuery(request.OrderId);

            var themeUsages = _finder.Find(filter)
                                    .SelectMany(query)
                                    .GroupBy(themeId => themeId)
                                    .Select(grouping => new ThemeSale
                                        {
                                            SaleCount = grouping.Count(),
                                            ThemeId = grouping.Key,
                                        })
                                    .ToArray();

            var oversales = themeUsages.Where(oversaleQuery).ToArray();

            var orderNumber = !IsCheckMassive && request.OrderId.HasValue
                                  ? _finder.Find(Specs.Find.ById<Order>(request.OrderId.Value)).Select(order => order.Number).Single()
                                  : null;

            // Получаем пачкой названия тем, по которым произошло превышение продаж
            var themeIds = oversales.Select(sale => sale.ThemeId).ToArray();
            var themeNames = _finder.Find<Theme>(theme => themeIds.Contains(theme.Id)).ToDictionary(theme => theme.Id, theme => theme.Name);

            foreach (var oversale in oversales)
            {
                var themeLabel = GenerateDescription(EntityName.Theme, themeNames[oversale.ThemeId], oversale.ThemeId);
                var message = new OrderValidationMessage
                    {
                        Type = MessageType.Error,
                        OrderId = request.OrderId.HasValue ? request.OrderId.Value : 0,
                        OrderNumber = orderNumber,
                        MessageText = string.Format(BLResources.ThemeSalesExceedsLimit, themeLabel, oversale.SaleCount, MaxPositionsPerTheme)
                    };

                messages.Add(message);
            }
        }

        private static Expression<Func<Order, IEnumerable<long>>> GetThemeFromOrderSelectQuery()
        {
            // Все идентификаторы тематик, использованные в заказах (фильтр настраивается отдельно)
            return order => order.OrderPositions
                .Where(position => position.IsActive && !position.IsDeleted)
                .SelectMany(position => position.OrderPositionAdvertisements)
                .Where(advertisement => advertisement.ThemeId.HasValue)
                .Select(advertisement => advertisement.ThemeId.Value);
        }

        private static Expression<Func<Order, bool>> GetMassiveCheckFiler(long? organizationUnitId, TimePeriod validationPeriod)
        {
            if (!organizationUnitId.HasValue)
            {
                throw new ArgumentNullException("organizationUnitId");
            }

            if (validationPeriod == null)
            {
                throw new ArgumentNullException("validationPeriod");
            }

            var allowedOrderStates = new[]
                {
                    (int)OrderState.OnTermination, 
                    (int)OrderState.Approved
                };

            // Во время массовой проверки (проверки к определенной сборке) выбираем
            // заказы данного подразделения, с периодом, пересекающимся с периодом сборки
            return order => order.DestOrganizationUnitId == organizationUnitId &&
                order.IsActive && !order.IsDeleted &&
                allowedOrderStates.Contains(order.WorkflowStepId) &&
                order.BeginDistributionDate <= validationPeriod.End &&
                validationPeriod.Start <= order.EndDistributionDateFact;
        }

        private Expression<Func<Order, bool>> GetSingleOrderCheckFilter(long? orderId)
        {
            if (!orderId.HasValue)
            {
                throw new ArgumentNullException("orderId");
            }

            var orderInfo = _finder.Find(Specs.Find.ById<Order>(orderId.Value))
                .Select(order => new
                {
                    OrganizationUnitId = order.DestOrganizationUnitId,
                    Period = new { Start = order.BeginDistributionDate, End = order.EndDistributionDateFact }
                })
                .Single();

            var allowedOrderStates = new[]
                {
                    (int)OrderState.OnApproval,
                    (int)OrderState.OnTermination, 
                    (int)OrderState.Approved
                };

            // Во время единичных проверок выбираем заказы, чьи периоды пересекают период публикации данного
            // Все заказы данного подразделения, с периодом, пересекающим период данного
            return order => order.DestOrganizationUnitId == orderInfo.OrganizationUnitId &&
                order.IsActive && !order.IsDeleted &&
                allowedOrderStates.Contains(order.WorkflowStepId) &&
                order.BeginDistributionDate <= orderInfo.Period.End &&
                orderInfo.Period.Start <= order.EndDistributionDateFact;
        }

        private Func<ThemeSale, bool> GetMassiveOversaleQuery()
        {
            return sale => sale.SaleCount > MaxPositionsPerTheme;
        }

        private Func<ThemeSale, bool> GetSingleOversaleQuery(long? orderId)
        {
            if (!orderId.HasValue)
            {
                throw new ArgumentNullException("orderId");
            }

            var themes = _finder.Find(Specs.Find.ById<Order>(orderId.Value))
                               .SelectMany(GetThemeFromOrderSelectQuery())
                               .ToArray();

            return sale => themes.Contains(sale.ThemeId) && sale.SaleCount > MaxPositionsPerTheme;
        }

        private sealed class ThemeSale
        {
            public long ThemeId { get; set; }
            public int SaleCount { get; set; }
        }
    }
}
