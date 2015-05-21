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
    /// <summary>
    /// В тематику по умолчанию могут размещаться только подразделения 2гис.
    /// Сейчас это возможно проверить по признаку саморекламы (но это не очень надёжно - могут забыть проставить)
    /// 
    /// Проверка выполняется в массовом и единичном порядке. 
    /// В случае, если подразделение (в массовом варианте) или заказ (в единичном) не содержат тематики по умолчанию,
    /// проверка проходит успешно.
    /// </summary>
    public sealed class DefaultThemeMustContainOnlySelfAdvValidationRule : OrderValidationRuleBase<HybridParamsValidationRuleContext>
    {
        private readonly IQuery _query;
        private readonly IFinder _finder;

        public DefaultThemeMustContainOnlySelfAdvValidationRule(IQuery query, IFinder finder)
        {
            _query = query;
            _finder = finder;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(HybridParamsValidationRuleContext ruleContext)
        {
            var defaultThemeId = ruleContext.ValidationParams.IsMassValidation
                                         ? GetDefaultThemeId(ruleContext.ValidationParams.Mass.OrganizationUnitId, ruleContext.ValidationParams.Mass.Period)
                                         : GetDefaultThemeId(ruleContext.ValidationParams.Single.OrderId);
            var filterByThemeUsage = GetFilterByThemeUsagePredicate(defaultThemeId);

            var existsInvalid = _query.For<Order>()
                                      .Where(ruleContext.OrdersFilterPredicate)
                                      .Where(filterByThemeUsage)
                                      .Any(order => order.OrderType != OrderType.SelfAds);

            if (!existsInvalid)
            {
                return Enumerable.Empty<OrderValidationMessage>();
            }

            var themeLabel = GetThemeDescription(ruleContext.ValidationParams.IsMassValidation, defaultThemeId);
            var orderInfo = ruleContext.ValidationParams.IsMassValidation
                                ? null
                                : _query.For(Specs.Find.ById<Order>(ruleContext.ValidationParams.Single.OrderId)) // для не массовых проверок идентификатор присутствует
                                        .Select(order => new { order.Id, order.Number })
                                        .SingleOrDefault();
            return new[]
                       {
                           new OrderValidationMessage
                               {
                                   Type = MessageType.Error,
                                   OrderId = orderInfo == null ? 0 : orderInfo.Id,
                                   OrderNumber = orderInfo == null ? null : orderInfo.Number,
                                   MessageText = string.Format(BLResources.DeafaultThemeMustContainOnlySelfAds, themeLabel),
                               }
                       };
        }

        private string GetThemeDescription(bool isMassValidation, long? themeId)
        {
            if (!themeId.HasValue)
            {
                throw new ArgumentNullException("themeId");
            }

            var name = _finder.FindOne(new SelectSpecification<Theme, string>(x => x.Name), Specs.Find.ById<Theme>(themeId.Value));

            return GenerateDescription(isMassValidation, EntityType.Instance.Theme(), name, themeId.Value);
        }

        private Expression<Func<Order, bool>> GetFilterByThemeUsagePredicate(long? themeId)
        {
            switch (themeId.HasValue)
            {
                case true:
                    return order => order.OrderPositions
                                 .Where(position => position.IsActive && !position.IsDeleted)
                                 .SelectMany(position => position.OrderPositionAdvertisements)
                                 .Any(advertisement => advertisement.ThemeId == themeId);
                default:
                    return order => false;
            }
        }

        private long? GetDefaultThemeId(long? orderId)
        {
            if (!orderId.HasValue)
            {
                throw new ArgumentNullException("orderId");
            }

            return _query.For(Specs.Find.ById<Order>(orderId.Value))
                         .SelectMany(order => order.OrderPositions)
                         .Where(Specs.Find.ActiveAndNotDeleted<OrderPosition>())
                         .SelectMany(position => position.OrderPositionAdvertisements)
                         .Select(advertisement => advertisement.Theme)
                         .Where(theme => theme.IsDefault)
                         .Select(theme => (long?)theme.Id)
                         .FirstOrDefault();
        }

        private long? GetDefaultThemeId(long? organizationUnitId, TimePeriod period)
        {
            if (!organizationUnitId.HasValue)
            {
                throw new ArgumentNullException("organizationUnitId");
            }

            if (period == null)
            {
                throw new ArgumentNullException("period");
            }

            return _query.For(Specs.Find.ActiveAndNotDeleted<ThemeOrganizationUnit>())
                         .Where(link => link.OrganizationUnitId == organizationUnitId)
                         .Select(link => link.Theme)
                         .Where(Specs.Find.ActiveAndNotDeleted<Theme>())
                         .Where(theme => theme.IsDefault && theme.BeginDistribution <= period.Start && theme.EndDistribution >= period.End)
                         .Select(theme => (long?)theme.Id)
                         .FirstOrDefault();
        }
    }
}