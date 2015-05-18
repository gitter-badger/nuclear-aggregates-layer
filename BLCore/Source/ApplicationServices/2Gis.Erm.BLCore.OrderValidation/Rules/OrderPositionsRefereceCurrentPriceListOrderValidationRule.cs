using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// Проверить позиции заказа на принадлежность к актуальному прайс-листу
    /// </summary>
    public sealed class OrderPositionsRefereceCurrentPriceListOrderValidationRule : OrderValidationRuleBase<SingleValidationRuleContext>
    {
        private readonly IOrderReadModel _orderReadModel;
        private readonly IFinder _finder;

        public OrderPositionsRefereceCurrentPriceListOrderValidationRule(IOrderReadModel orderReadModel, IFinder finder)
        {
            _orderReadModel = orderReadModel;
            _finder = finder;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(SingleValidationRuleContext ruleContext)
        {
            long actualPriceId;
            if (!_orderReadModel.TryGetActualPriceIdForOrder(ruleContext.ValidationParams.OrderId, out actualPriceId))
            {
                return new[]
                    {
                               new OrderValidationMessage
                                   {
                        Type = MessageType.Error,
                                       OrderId = ruleContext.ValidationParams.OrderId,
                        MessageText = BLResources.OrderCheckOrderPositionsDoesntCorrespontToActualPrice,
                                   }
                           };
            }

            var orderInfos = _finder.Find(Specs.Find.ById<Order>(ruleContext.ValidationParams.OrderId))
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

            var results = new List<OrderValidationMessage>();

            foreach (var orderInfo in orderInfos)
            {
                foreach (var orderPosition in orderInfo.OrderPositions)
                {
                    if (orderPosition.BadPriceList)
                    {
                        var orderPositionDescription = GenerateDescription(false, EntityType.Instance.OrderPosition(), orderPosition.PositionName, orderPosition.Id);
                        var messageType = orderInfo.WorkflowStepId == OrderState.Approved ? MessageType.Warning : MessageType.Error;
                        results.Add(new OrderValidationMessage
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
                        var orderPositionDescription = GenerateDescription(false, EntityType.Instance.OrderPosition(), orderPosition.PositionName, orderPosition.Id);
                        results.Add(new OrderValidationMessage
                        {
                            Type = MessageType.Error,
                            OrderId = orderInfo.Id,
                            OrderNumber = orderInfo.Number,

                            MessageText = string.Format(CultureInfo.CurrentCulture, BLResources.OrderCheckOrderPositionCorrespontToInactivePosition, orderPositionDescription),
                        });
                    }
                }
            }

            return results;
        }
    }
}
