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
    /// Проверяет, чтобы для баннеров в рубрику "Выгодные покупки с 2ГИС" была указана эта рубрика
    /// </summary>
    public sealed class IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        public const int BanerForAdvantageousPurchasesPositionCategoryId = 296; // Баннер в рубрике Выгодные покупки с 2ГИС
        public const int AdvantageousPurchasesCategoryId = 18599;

        private readonly IFinder _finder;

        public IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            var badAdvertisemements = _finder.Find(filterPredicate)
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

            foreach (var advertisemement in badAdvertisemements)
            {
                var orderPositionDescription = GenerateDescription(EntityName.OrderPosition, advertisemement.OrderPositionName, advertisemement.OrderPositionId);

                messages.Add(new OrderValidationMessage
                    {
                        Type = MessageType.Error,
                        OrderId = advertisemement.OrderId,
                        OrderNumber = advertisemement.OrderNumber,
                        MessageText = string.Format(BLResources.IsBanerForAdvantageousPurchasesPositionCategoryLinkedWithAdvantageousPurchasesCategoryError,
                                                    orderPositionDescription)
                    });
            }
        }
    }
}
