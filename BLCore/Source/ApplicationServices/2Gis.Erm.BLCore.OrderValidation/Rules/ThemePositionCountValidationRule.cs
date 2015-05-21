using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Storage;
using NuClear.Storage.Specifications;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class ThemePositionCountValidationRule : OrderValidationRuleBase<HybridParamsValidationRuleContext>
    {
        private const int MaxPositionsPerTheme = 10;
        private readonly IQuery _query;

        public ThemePositionCountValidationRule(IQuery query)
        {
            _query = query;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(HybridParamsValidationRuleContext ruleContext)
        {
            var query = GetThemeFromOrderSelectQuery();
            var filter = ruleContext.ValidationParams.IsMassValidation
                             ? GetMassiveCheckFilter(ruleContext.ValidationParams.Mass.OrganizationUnitId, ruleContext.ValidationParams.Mass.Period)
                             : GetSingleOrderCheckFilter(ruleContext.ValidationParams.Single.OrderId);
            var oversaleQuery = ruleContext.ValidationParams.IsMassValidation
                                    ? GetMassiveOversaleQuery()
                                    : GetSingleOversaleQuery(ruleContext.ValidationParams.Single.OrderId);

            var themeUsages = _query.For<Order>()
                                    .Where(filter)
                                    .SelectMany(query)
                                    .GroupBy(themeId => themeId)
                                    .Select(grouping => new ThemeSale
                                        {
                                            SaleCount = grouping.Count(),
                                            ThemeId = grouping.Key,
                                        })
                                    .ToArray();

            var oversales = themeUsages.Where(oversaleQuery).ToArray();

            var orderNumber = !ruleContext.ValidationParams.IsMassValidation
                                  ? _query.For(Specs.Find.ById<Order>(ruleContext.ValidationParams.Single.OrderId)).Select(order => order.Number).Single()
                                  : null;

            // Получаем пачкой названия тем, по которым произошло превышение продаж
            var themeIds = oversales.Select(sale => sale.ThemeId).ToArray();
            var themeNames = _query.For(new FindSpecification<Theme>(theme => themeIds.Contains(theme.Id))).ToDictionary(theme => theme.Id, theme => theme.Name);


            return oversales.Select(x => new OrderValidationMessage
                                             {
                                                 Type = MessageType.Error,
                                                 OrderId = !ruleContext.ValidationParams.IsMassValidation ? ruleContext.ValidationParams.Single.OrderId : 0,
                                                 OrderNumber = orderNumber,
                                                 MessageText =
                                                     string.Format(BLResources.ThemeSalesExceedsLimit,
                                                                   GenerateDescription(ruleContext.ValidationParams.IsMassValidation,
                                                                                       EntityType.Instance.Theme(),
                                                                                       themeNames[x.ThemeId],
                                                                                       x.ThemeId),
                                                                   x.SaleCount,
                                                                   MaxPositionsPerTheme)
                                             });
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

        private static Expression<Func<Order, bool>> GetMassiveCheckFilter(long organizationUnitId, TimePeriod validationPeriod)
        {
            var allowedOrderStates = new[]
                {
                    OrderState.OnTermination, 
                    OrderState.Approved
                };

            // Во время массовой проверки (проверки к определенной сборке) выбираем
            // заказы данного подразделения, с периодом, пересекающимся с периодом сборки
            return order => order.DestOrganizationUnitId == organizationUnitId &&
                order.IsActive && !order.IsDeleted &&
                allowedOrderStates.Contains(order.WorkflowStepId) &&
                order.BeginDistributionDate <= validationPeriod.End &&
                validationPeriod.Start <= order.EndDistributionDateFact;
        }

        private Expression<Func<Order, bool>> GetSingleOrderCheckFilter(long orderId)
        {
            var orderInfo = _query.For(Specs.Find.ById<Order>(orderId))
                .Select(order => new
                {
                    OrganizationUnitId = order.DestOrganizationUnitId,
                    Period = new { Start = order.BeginDistributionDate, End = order.EndDistributionDateFact }
                })
                .Single();

            var allowedOrderStates = new[]
                {
                    OrderState.OnApproval,
                    OrderState.OnTermination, 
                    OrderState.Approved
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

        private Func<ThemeSale, bool> GetSingleOversaleQuery(long orderId)
        {
            var themes = _query.For(Specs.Find.ById<Order>(orderId))
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
