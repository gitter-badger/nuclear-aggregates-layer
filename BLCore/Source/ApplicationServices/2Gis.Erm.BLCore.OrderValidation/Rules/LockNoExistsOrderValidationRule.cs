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
    /// <summary>
    /// Проверить на отсутсвие созданных блокировок по периоду
    /// </summary>
    public sealed class LockNoExistsOrderValidationRule : OrderValidationRuleBase<MassOverridibleValidationRuleContext>
    {
        private readonly IQuery _query;

        public LockNoExistsOrderValidationRule(IQuery query)
        {
            _query = query;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(MassOverridibleValidationRuleContext ruleContext)
        {
            return _query.For<Order>()
                          .Where(ruleContext.CombinedPredicate.GetCombinedPredicate())
                          .Where(x => x.DestOrganizationUnitId == ruleContext.ValidationParams.OrganizationUnitId)
                          .Where(o => o.Locks.Any(l => l.IsActive && !l.IsDeleted && l.PeriodStartDate == ruleContext.ValidationParams.Period.Start
                                                        && l.PeriodEndDate == ruleContext.ValidationParams.Period.End))
                          .Select(o => new { o.Id, o.Number })
                          .AsEnumerable()
                          .Select(x => new OrderValidationMessage
                                           {
                                               Type = MessageType.Error,
                                               MessageText = string.Format(BLResources.OrdersCheckOrderHasLock, x.Number),
                                               OrderId = x.Id,
                                               OrderNumber = x.Number
                                           });
        }
    }
}
