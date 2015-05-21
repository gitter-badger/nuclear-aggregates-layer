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
    /// В ERM реализовать проверку, заказов перед сборкой на связь позиции прайс листа, заказов попавших в сборку, с категориями позиций, которые поддерживает экспорт.
    /// </summary>
    public sealed class IsPositionSupportedByExportOrderValidationRule : OrderValidationRuleBase<OrdinaryValidationRuleContext>
    {
        private readonly IQuery _query;

        public IsPositionSupportedByExportOrderValidationRule(IQuery query)
        {
            _query = query;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(OrdinaryValidationRuleContext ruleContext)
        {
            return _query.For<Order>()
                          .Where(ruleContext.OrdersFilterPredicate)
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
                          .AsEnumerable()
                          .Select(x => new OrderValidationMessage
                                           {
                                               Type = MessageType.Error,
                                               OrderId = x.OrderId,
                                               OrderNumber = x.OrderNumber,
                                               MessageText =
                                                   string.Format(BLResources.PositionCategeryOfOrderPositionIsNotSupportedByExport,
                                                                 GenerateDescription(ruleContext.IsMassValidation, EntityType.Instance.OrderPosition(), x.OrderPositionName, x.OrderPositionId))
                                           });
        }
    }
}
