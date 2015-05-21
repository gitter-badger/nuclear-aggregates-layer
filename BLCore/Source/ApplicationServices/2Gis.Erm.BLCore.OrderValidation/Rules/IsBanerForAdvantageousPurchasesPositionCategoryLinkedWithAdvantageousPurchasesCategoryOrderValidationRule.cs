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
    /// Проверяет, чтобы для баннеров в рубрику "Выгодные покупки с 2ГИС" была указана эта рубрика
    /// </summary>
    public sealed class IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule : OrderValidationRuleBase<OrdinaryValidationRuleContext>
    {
        public const int BanerForAdvantageousPurchasesPositionCategoryId = 296; // Баннер в рубрике Выгодные покупки с 2ГИС
        public const int AdvantageousPurchasesCategoryId = 18599;

        private readonly IQuery _query;

        public IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule(IQuery query)
        {
            _query = query;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(OrdinaryValidationRuleContext ruleContext)
        {
            var badAdvertisemements = _query.For<Order>()
                                            .Where(ruleContext.OrdersFilterPredicate)
                                            .SelectMany(order => order.OrderPositions)
                                            .Where(orderPosition => orderPosition.IsActive && !orderPosition.IsDeleted)
                                            .SelectMany(orderPosition =>
                                                        orderPosition.OrderPositionAdvertisements
                                                                     .Where(opa => opa.CategoryId != AdvantageousPurchasesCategoryId &&
                                                                                   opa.Position.CategoryId == BanerForAdvantageousPurchasesPositionCategoryId)
                                                                     .Select(advertisement => new
                                                                         {
                                                                             OrderPositionId = orderPosition.Id,
                                                                             OrderPositionName = advertisement.Position.Name,
                                                                             OrderId = orderPosition.Order.Id,
                                                                             OrderNumber = orderPosition.Order.Number
                                                                         }))
                                            .ToArray();

            return badAdvertisemements.Select(x => new OrderValidationMessage
                                                       {
                                                           Type = MessageType.Error,
                                                           OrderId = x.OrderId,
                                                           OrderNumber = x.OrderNumber,
                                                           MessageText =
                                                               string.Format(
                                                                             BLResources.IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryError,
                                                                             GenerateDescription(ruleContext.IsMassValidation,
                                                                                                 EntityType.Instance.OrderPosition(),
                                                                                                 x.OrderPositionName,
                                                                                                 x.OrderPositionId))
                                                       });
        }
    }
}
