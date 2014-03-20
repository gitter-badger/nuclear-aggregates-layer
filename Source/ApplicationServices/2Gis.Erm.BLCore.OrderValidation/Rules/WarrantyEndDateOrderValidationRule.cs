using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class WarrantyEndDateOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public WarrantyEndDateOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            var badOrders =
                _finder.Find(filterPredicate)
                      .Select(
                          x =>
                          new
                              {
                                  Order = x,
                                  Profiles =
                              x.LegalPerson.LegalPersonProfiles.Where(
                                  y =>
                                  y.IsActive && !y.IsDeleted && y.OperatesOnTheBasisInGenitive != null
                                  && (OperatesOnTheBasisType)y.OperatesOnTheBasisInGenitive == OperatesOnTheBasisType.Warranty && y.WarrantyEndDate != null
                                  && y.WarrantyEndDate < x.SignupDate)
                              })
                      .Where(x => x.Profiles.Any())
                      .ToArray();

            foreach (var orderInfo in badOrders)
            {
                foreach (var profile in orderInfo.Profiles)
                {
                    messages.Add(
                        new OrderValidationMessage
                            {
                                Type = MessageType.Info,
                                OrderId = orderInfo.Order.Id,
                                OrderNumber = orderInfo.Order.Number,
                                MessageText = string.Format(BLResources.ProfileWarrantyEndDateIsLessThanSignDate, profile.Name)
                            });
                }
            }
        }
    }
}
