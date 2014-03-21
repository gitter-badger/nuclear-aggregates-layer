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
    public sealed class BargainOutOfDateOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public BargainOutOfDateOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            if (request.OrderId == null)
            {
                throw new ArgumentException("request");
            }

            var isBargainOutOfDate = _finder.Find(filterPredicate)
                .Where(x => x.Id == request.OrderId)
                .Any(x => x.SignupDate > x.Bargain.ClosedOn);

            if (isBargainOutOfDate)
            {
                messages.Add(new OrderValidationMessage
                               {
                                   OrderId = (int)request.OrderId,
                                   Type = MessageType.Error,
                                   MessageText = BLResources.OrdersCheckBargainIsOutOfDate
                               });
            }
        }
    }
}