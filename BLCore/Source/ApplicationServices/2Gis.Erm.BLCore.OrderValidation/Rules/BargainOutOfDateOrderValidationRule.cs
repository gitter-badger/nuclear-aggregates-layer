using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Readings;
using NuClear.Storage.Specifications;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class BargainOutOfDateOrderValidationRule : OrderValidationRuleBase<SingleValidationRuleContext>
    {
        private readonly IFinder _finder;

        public BargainOutOfDateOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(SingleValidationRuleContext ruleContext)
        {
            return _finder.Find(Specs.Find.ById<Order>(ruleContext.ValidationParams.OrderId) &&
                                new FindSpecification<Order>(x => x.SignupDate > x.Bargain.ClosedOn))
                          .Many()
                          .Select(x => new OrderValidationMessage
                              {
                                  OrderId = x.Id,
                                  Type = MessageType.Error,
                                  MessageText = BLResources.OrdersCheckBargainIsOutOfDate
                              });
        }
    }
}