using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;

using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class LegalPersonProfilesOrderValidationRule : OrderValidationRuleBase<OrdinaryValidationRuleContext>
    {
        private readonly IFinder _finder;

        public LegalPersonProfilesOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(OrdinaryValidationRuleContext ruleContext)
        {
            return _finder.Find(ruleContext.OrdersFilterPredicate)
                          .Where(x => !x.LegalPerson.LegalPersonProfiles.Any(y => y.IsActive && !y.IsDeleted))
                          .AsEnumerable()
                          .Select(x => new OrderValidationMessage
                                           {
                                               Type = MessageType.Error,
                                               OrderId = x.Id,
                                               OrderNumber = x.Number,
                                               MessageText = BLResources.MustMakeLegalPersonProfile
                                           });
        }
    }
}
