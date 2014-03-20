using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверка фирм на актуальность
    /// </summary>
    public sealed class FirmsOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public FirmsOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            var orders = _finder.Find(filterPredicate)
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

                var firmDescription = GenerateDescription(EntityName.Firm, order.FirmName, order.FirmId);
                messages.Add(new OrderValidationMessage
                               {
                                   Type = MessageType.Error,
                                   OrderId = order.OrderId,
                                   OrderNumber = order.OrderNumber,
                                   MessageText = string.Format(template, firmDescription)
                               });
            }
        }
    }
}