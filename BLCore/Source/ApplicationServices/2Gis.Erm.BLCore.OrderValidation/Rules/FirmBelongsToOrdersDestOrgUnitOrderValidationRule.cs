using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;

using NuClear.Storage;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверить соответствие города назначения отделению организации за которым закреплена фирма, выбранная в заказ
    /// </summary>
    public sealed class FirmBelongsToOrdersDestOrgUnitOrderValidationRule : OrderValidationRuleBase<OrdinaryValidationRuleContext>
    {
        private readonly IFinder _finder;

        public FirmBelongsToOrdersDestOrgUnitOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(OrdinaryValidationRuleContext ruleContext)
        {
            return _finder.Find(ruleContext.OrdersFilterPredicate)
                          .Where(order => order.Firm.OrganizationUnitId != order.DestOrganizationUnitId)
                          .Select(order => new { order.Id, order.Number, order.FirmId })
                          .AsEnumerable()
                          .Select(x => new OrderValidationMessage
                                           {
                                               Type = MessageType.Error,
                                               MessageText = string.Format(BLResources.OrdersCheckDestOrganizationUnitDoesntMatchFirmsOne, x.Number),
                                               OrderId = x.Id,
                                               OrderNumber = x.Number
                                           });
        }
    }
}