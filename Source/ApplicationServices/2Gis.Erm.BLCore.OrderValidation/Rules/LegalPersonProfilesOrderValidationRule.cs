using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class LegalPersonProfilesOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public LegalPersonProfilesOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            var badOrders = _finder.Find(filterPredicate)
                .Where(x => !x.LegalPerson.LegalPersonProfiles.Any(y => y.IsActive && !y.IsDeleted));

            foreach (var order in badOrders)
            {
                messages.Add(new OrderValidationMessage
                {
                    Type = MessageType.Error,
                    OrderId = order.Id,
                    OrderNumber = order.Number,
                    MessageText = BLResources.MustMakeLegalPersonProfile
                });
            }
        }
    }
}
