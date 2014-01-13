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
    /// В ERM реализовать проверку, заказов перед сборкой на связь позиции прайс листа, заказов попавших в сборку, с категориями позиций, которые поддерживает экспорт.
    /// </summary>
    public sealed class IsPositionSupportedByExportOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public IsPositionSupportedByExportOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(
            ValidateOrdersRequest request,
            Expression<Func<Order, bool>> filterPredicate,
            IList<OrderValidationMessage> messages)
        {
            var badPositions =
                _finder.Find(filterPredicate)
                       .SelectMany(x => x.OrderPositions)
                       .Where(y => y.IsActive && !y.IsDeleted)
                       .SelectMany(x => x.OrderPositionAdvertisements)
                       .Where(x => !x.Position.PositionCategory.IsSupportedByExport)
                       .Select(
                           x =>
                           new
                               {
                                   x.OrderPositionId,
                                   OrderPositionName = x.Position.Name,
                                   x.OrderPosition.OrderId,
                                   OrderNumber = x.OrderPosition.Order.Number
                               })
                       .ToArray();

            foreach (var position in badPositions)
            {
                var orderPositionDescription = GenerateDescription(EntityName.OrderPosition, position.OrderPositionName, position.OrderPositionId);

                messages.Add(
                    new OrderValidationMessage
                        {
                            Type = MessageType.Error,
                            OrderId = position.OrderId,
                            OrderNumber = position.OrderNumber,
                            MessageText =
                                string.Format(BLResources.PositionCategeryOfOrderPositionIsNotSupportedByExport, orderPositionDescription)
                        });
            }
        }
    }
}
