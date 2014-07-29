using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверить позиции заказа на принадлежность к актуальному прайс-листу
    /// </summary>
    public sealed class OrderPositionsRefereceCurrentPriceListOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly IFinder _finder;

        public OrderPositionsRefereceCurrentPriceListOrderValidationRule(IOrderReadModel orderReadModel, IFinder finder)
        {
            _orderReadModel = orderReadModel;
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            if (request.OrderId == null)
            {
                throw new ArgumentException("request.OrderId");
            }

            long actualPriceId;
            if (!_orderReadModel.TryGetActualPriceIdForOrder(request.OrderId.Value, out actualPriceId))
            {
                messages.Add(new OrderValidationMessage
                    {
                        Type = MessageType.Error,
                        OrderId = request.OrderId.Value,
                        MessageText = BLResources.OrderCheckOrderPositionsDoesntCorrespontToActualPrice,
                    });

                return;
            }

            // FIXME {all, 12.12.2013}: Возможно, этот запрос возвращает слишком много данных,: он возвращает и валидные и невалидные заказы. Есть мнение, что, если добавить фильтр - работать будет лучше.
            var orderInfos = _finder.Find(filterPredicate)
                .Select(x => new
                    {
                        x.Id,
                        x.Number,
                        x.WorkflowStepId,

                        OrderPositions = x.OrderPositions
                                 .Where(y => y.IsActive && !y.IsDeleted)
                                 .Select(y => new
                                     {
                                         y.Id,
                                         PositionId = y.PricePosition.Position.Id,
                                         PositionName = y.PricePosition.Position.Name,
                                         BadPriceList = y.PricePosition.PriceId != actualPriceId,
                                         PricePositionIsNotActive = !y.PricePosition.IsActive || y.PricePosition.IsDeleted
                                     })
                    })
                .ToArray();

            foreach (var orderInfo in orderInfos)
            {
                foreach (var orderPosition in orderInfo.OrderPositions)
                {
                    if (orderPosition.BadPriceList)
                    {
                    var orderPositionDescription = GenerateDescription(EntityName.OrderPosition, orderPosition.PositionName, orderPosition.Id);
                        var messageType = orderInfo.WorkflowStepId == (int)OrderState.Approved ? MessageType.Warning : MessageType.Error;
                        messages.Add(new OrderValidationMessage
                        {
                                Type = messageType,
                            OrderId = orderInfo.Id,
                            OrderNumber = orderInfo.Number,
                            AdditionalInfo = { { "HasOutdatedPricePositions", true } },
                            MessageText = string.Format(CultureInfo.CurrentCulture, BLResources.OrderCheckOrderPositionDoesntCorrespontToActualPrice, orderPositionDescription),
                        });
                    }

                    if (!orderPosition.BadPriceList && orderPosition.PricePositionIsNotActive)
                    {
                        var orderPositionDescription = GenerateDescription(EntityName.OrderPosition, orderPosition.PositionName, orderPosition.Id);
                        messages.Add(new OrderValidationMessage
                        {
                            Type = MessageType.Error,
                            OrderId = orderInfo.Id,
                            OrderNumber = orderInfo.Number,

                            MessageText = string.Format(CultureInfo.CurrentCulture, BLResources.OrderCheckOrderPositionCorrespontToInactivePosition, orderPositionDescription),
                        });
                    }
                }
            }
        }
    }
}
