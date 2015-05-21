using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Storage;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверка фирм на актуальность
    /// </summary>
    public sealed class FirmsOrderValidationRule : OrderValidationRuleBase<OrdinaryValidationRuleContext>
    {
        private readonly IQuery _query;

        public FirmsOrderValidationRule(IQuery query)
        {
            _query = query;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(OrdinaryValidationRuleContext ruleContext)
        {
            var orders = _query.For<Order>()
                .Where(ruleContext.OrdersFilterPredicate)
                .Where(order => order.Firm.ClosedForAscertainment || !order.Firm.IsActive || order.Firm.IsDeleted)
                .GroupBy(order => order.FirmId)
                .Select(group => group.FirstOrDefault())
                .Select(order => new
                                     {
                                         FirmId = order.Firm.Id,
                                         FirmName = order.Firm.Name,
                                         FirmIsDeleted = order.Firm.IsDeleted,
                                         FirmIsClosed = !order.Firm.IsActive,
                                         FirmIsClosedForAscertainment = order.Firm.ClosedForAscertainment,
                                         OrderId = order.Id,
                                         OrderNumber = order.Number
                                     })
                .ToArray();

            var results = new List<OrderValidationMessage>();

            foreach (var order in orders)
            {
                var template = string.Empty;
                if (order.FirmIsDeleted)
                {
                    template = BLResources.FirmIsDeleted;
                }
                else if (order.FirmIsClosed)
                {
                    template = BLResources.FirmIsPermanentlyClosed;
                }
                else if (order.FirmIsClosedForAscertainment)
                {
                    template = BLResources.OrderFirmHiddenForAscertainmentTemplate;
                }

                var firmDescription = GenerateDescription(ruleContext.IsMassValidation, EntityType.Instance.Firm(), order.FirmName, order.FirmId);
                results.Add(new OrderValidationMessage
                               {
                                   Type = MessageType.Error,
                                   OrderId = order.OrderId,
                                   OrderNumber = order.OrderNumber,
                                   MessageText = string.Format(template, firmDescription)
                               });
            }

            return results;
        }
    }
}