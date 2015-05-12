using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;
using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверить на наличие привязки к лицевому счёту
    /// </summary>
    public sealed class AccountExistsOrderValidationRule : OrderValidationRuleBase<OrdinaryValidationRuleContext>
    {
        private readonly IFinder _finder;

        public AccountExistsOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(OrdinaryValidationRuleContext ruleContext)
        {
            return _finder.Find(ruleContext.OrdersFilterPredicate)
                          .Where(o => o.AccountId == null || (!o.Account.IsActive && o.Account.IsDeleted))
                          .Select(o => new { o.Id, o.Number })
                          .AsEnumerable()
                          .Select(o => new OrderValidationMessage
                                           {
                                               Type = MessageType.Error,
                                               MessageText =
                                                   string.Format(BLResources.OrdersCheckOrderHasNoAccount,
                                                                 GenerateDescription(ruleContext.IsMassValidation, EntityType.Instance.Order(), o.Number, o.Id)),
                                               OrderId = o.Id,
                                               OrderNumber = o.Number
                                           });
        }
    }
}
