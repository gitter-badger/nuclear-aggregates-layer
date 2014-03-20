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
    public sealed class PlatformTypeOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public PlatformTypeOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            var ordersWithErrors = _finder.Find(filterPredicate)
                                          .Select(x => new
                                              {
                                                  x.Id,
                                                  x.Number,
                                                  OrderPositionsAdvertisements =
                                                           x.OrderPositions
                                                            .Where(y => y.IsActive && !y.IsDeleted)
                                                            .SelectMany(y => y.OrderPositionAdvertisements)
                                                            .Select(y => new
                                                                {
                                                                    y.Position.Name,
                                                                    y.Position.Platform.IsSupportedByExport
                                                                })
                                                            .Where(y => !y.IsSupportedByExport)
                                              })
                                          .Where(y => y.OrderPositionsAdvertisements.Any())
                                          .ToArray();

            foreach (var order in ordersWithErrors)
            {
                foreach (var orderPositionAdvertisement in order.OrderPositionsAdvertisements)
                {
                    messages.Add(new OrderValidationMessage
                        {
                            Type = MessageType.Error,
                            OrderId = order.Id,
                            OrderNumber = order.Number,
                            MessageText = string.Format(BLResources.RegisteredPositionIsNotSupportedByExport, orderPositionAdvertisement.Name)
                        });
                }
            }
        }
    }
}