using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверить на наличие привязки к лицевому счёту
    /// </summary>
    public sealed class AccountExistsOrderValidationRule : OrderValidationRuleBase<OrdinaryValidationRuleContext>
    {
        private readonly IQuery _query;

        public AccountExistsOrderValidationRule(IQuery query)
        {
            _query = query;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(OrdinaryValidationRuleContext ruleContext)
        {
            return _query.For(new FindSpecification<Order>(ruleContext.OrdersFilterPredicate))
                         .Where(o => o.AccountId == null || (!o.Account.IsActive && o.Account.IsDeleted))
                         .Select(o => new { o.Id, o.Number })
                         .Select(o => new OrderValidationMessage
                             {
                                 Type = MessageType.Error,
                                 MessageText = string.Format(BLResources.OrdersCheckOrderHasNoAccount,
                                                             GenerateDescription(ruleContext.IsMassValidation, EntityType.Instance.Order(), o.Number, o.Id)),
                                 OrderId = o.Id,
                                 OrderNumber = o.Number
                             });
        }
    }
}
