using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;

using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверить на отсутсвие созданных блокировок по периоду
    /// </summary>
    public sealed class LockNoExistsOrderValidationRule : OrderValidationRuleBase<MassOverridibleValidationRuleContext>
    {
        private readonly IFinder _finder;

        public LockNoExistsOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(MassOverridibleValidationRuleContext ruleContext)
        {
            return _finder.Find(ruleContext.CombinedPredicate.GetCombinedPredicate())
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
