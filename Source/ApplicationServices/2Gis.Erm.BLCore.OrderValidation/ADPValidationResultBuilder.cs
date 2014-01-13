using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;

namespace DoubleGis.Erm.BLCore.OrderValidation
{
    public class ADPValidationResultBuilder
    {
        private readonly IDictionary<long, OrderStateStorage> _orderStates;
        private readonly ADPCheckMode _checkMode;
        private readonly OrderStateStorage _principalOrderStateStorage;

        private readonly HashSet<Tuple<long, long, long>> _dictionary = new HashSet<Tuple<long, long, long>>();
        private readonly List<OrderValidationMessage> _results = new List<OrderValidationMessage>();

        public bool IsCheckSpecific
        {
            get { return _checkMode == ADPCheckMode.SpecificOrder; }
        }

        public bool IsCheckMassive
        {
            get { return _checkMode == ADPCheckMode.Massive; }
        }

        public ADPValidationResultBuilder(IDictionary<long, OrderStateStorage> orderStates, ADPCheckMode checkMode, long orderId)
        {
            _orderStates = orderStates;
            _checkMode = checkMode;
            if (checkMode != ADPCheckMode.Massive)
            {
                _principalOrderStateStorage = orderStates[orderId];
            }
        }

        public void AddDependencyMessage(long principalOrderId, long principalOrderPositionId, long associatedOrderId, long associatedPositionId)
        {
            if (_checkMode == ADPCheckMode.OrderBeingCancelled &&
                principalOrderId == _principalOrderStateStorage.Id &&
                associatedOrderId != principalOrderId)
            {
                _dictionary.Add(new Tuple<long, long, long>(principalOrderPositionId, associatedOrderId, associatedPositionId));
            }
        }

        public void AddMessage(MessageType type, string message, long orderId)
        {
            _results.Add(new OrderValidationMessage
                {
                    OrderId = orderId,
                    OrderNumber = _orderStates[orderId].Number,
                    Type = type,
                    MessageText = message
                });
        }

        public void AddDeniedPositionsMessage(OrderPositionStateStorage firstPosition, OrderPositionStateStorage secondPosition)
        {
            string messageTemplate;

            switch (_checkMode)
            {
                case ADPCheckMode.Massive:
                case ADPCheckMode.SpecificOrder:
                    messageTemplate = BLResources.ADPCheckModeSpecificOrder_MessageTemplate;
                    break;
                case ADPCheckMode.OrderBeingReapproved:
                    messageTemplate = BLResources.ADPCheckModeOrderBeingReapproved_MessageTemplate;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            bool needRichDescription = _checkMode != ADPCheckMode.Massive;

            string message = string.Format(messageTemplate,
                                           firstPosition.GetDescription(needRichDescription),
                                           secondPosition.GetDescription(needRichDescription, firstPosition.Order != secondPosition.Order));

            AddMessage(MessageType.Error, message, firstPosition.Order.Id);
        }

        public IEnumerable<OrderValidationMessage> GetMessages()
        {
            var response = new List<OrderValidationMessage>();
            response.AddRange(_results);

            var temp = _dictionary
                .GroupBy(item => item.Item1)
                .Select(group => new
                    {
                        PrincipalOrderPositionId = group.Key,
                        AssociatedPositions = group.Select(item => new {OrderId = item.Item2, OrderPositionsId = item.Item3})
                                     .OrderBy(item => item.OrderId)
                                     .ThenBy(item => item.OrderPositionsId)
                    });
            foreach (var item in temp)
            {
                string template = BLResources.ADPValidation_Template;
                string appendTemplate = BLResources.ADPValidation_AppendTemplate;
                var builder = new StringBuilder();
                var principalOrder = _principalOrderStateStorage;
                builder.AppendFormat(template, principalOrder.PositionsIndexes[item.PrincipalOrderPositionId], item.PrincipalOrderPositionId);

                bool was = false;

                foreach (var position in item.AssociatedPositions)
                {
                    if (was)
                    {
                        builder.Append(BLResources.ListSeparator);
                    }
                    was = true;
                    var order = _orderStates[position.OrderId];
                    builder.AppendFormat(appendTemplate, order.PositionsIndexes[position.OrderPositionsId], position.OrderPositionsId, order.Number, position.OrderId);
                }

                response.Add(new OrderValidationMessage
                    {
                        Type = MessageType.Warning,
                        OrderId = _principalOrderStateStorage.Id,
                        OrderNumber = _principalOrderStateStorage.Number,
                        MessageText = builder.ToString()
                    });
            }

            return response;

        }
    }
}