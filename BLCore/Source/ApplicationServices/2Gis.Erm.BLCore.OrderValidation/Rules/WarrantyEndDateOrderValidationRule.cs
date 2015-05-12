using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class WarrantyEndDateOrderValidationRule : OrderValidationRuleBase<OrdinaryValidationRuleContext>
    {
        private readonly IFinder _finder;

        public WarrantyEndDateOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(OrdinaryValidationRuleContext ruleContext)
        {
            var badOrders = _finder.Find(ruleContext.OrdersFilterPredicate)
                       .Select(x => new
                           {
                               Order = x,
                               ProfileNames = x.LegalPerson.LegalPersonProfiles
                                               .Where(y => y.IsActive && !y.IsDeleted
                                                           && y.OperatesOnTheBasisInGenitive != null
                                                           && y.OperatesOnTheBasisInGenitive == OperatesOnTheBasisType.Warranty
                                                           && y.WarrantyEndDate != null 
                                                           && y.WarrantyEndDate < x.SignupDate)
                                               .Select(profile => profile.Name)
                           })
                       .Where(x => x.ProfileNames.Any())
                       .ToArray();

            return from orderInfo in badOrders
                   from profileName in orderInfo.ProfileNames
                   select new OrderValidationMessage
                        {
                            Type = MessageType.Info,
                            OrderId = orderInfo.Order.Id,
                            OrderNumber = orderInfo.Order.Number,
                            MessageText = string.Format(BLResources.ProfileWarrantyEndDateIsLessThanSignDate, profileName)
                              };
        }
    }
}
