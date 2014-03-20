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
    /// При оформлении продажи позиции связанной с категорией позиции:
    /// Выгодные покупки с 2ГИС;
    /// Баннер в рубрике Выгодные покупки с 2ГИС,
    /// необходимо проверять, что соответствующая рубрика "Выгодные покупки с 2ГИС" есть у фирмы.
    /// </summary>
    public sealed class IsThereAdvantageousPurchasesRubricOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private const int AdvantageousPurchasesPositionCategoryId = 14; // Выгодные покупки с 2ГИС;
        private const int BanerForAdvantageousPurchasesPositionCategoryId = 296; // Баннер в рубрике Выгодные покупки с 2ГИС
        private const int AdvantageousPurchasesCategoryId = 18599; // Выгодные покупки с 2ГИС;

        private readonly IFinder _finder;

        public IsThereAdvantageousPurchasesRubricOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            var badAdvertisemements =
                _finder.Find(filterPredicate)
                      .SelectMany(order => order.OrderPositions)
                       .Where(orderPosition =>
                              orderPosition.IsActive && !orderPosition.IsDeleted)
                       .SelectMany(op => op.OrderPositionAdvertisements)
                       .Where(opa => (opa.Position.CategoryId == AdvantageousPurchasesPositionCategoryId
                                      || opa.Position.CategoryId == BanerForAdvantageousPurchasesPositionCategoryId)
                                     && (opa.OrderPosition.Order.FirmId == null
                                         || !opa.OrderPosition.Order.Firm.FirmAddresses.Any(
                                             x =>
                                             x.IsActive && !x.IsDeleted &&
                                             x.CategoryFirmAddresses.Any(y => y.Category.Id == AdvantageousPurchasesCategoryId))))
                       .Select(opa =>
                          new
                              {
                                       OrderPositionId = opa.OrderPositionId,
                                       OrderPositionName = opa.Position.Name,
                                       OrderId = opa.OrderPosition.OrderId,
                                       OrderNumber = opa.OrderPosition.Order.Number,
                                       opa.OrderPosition.Order.FirmId,
                                       FirmName = opa.OrderPosition.Order.Firm.Name
                              })
                      .ToArray();

            foreach (var advertisemement in badAdvertisemements)
            {
                var orderPositionDescription = GenerateDescription(EntityName.OrderPosition,
                                                                   advertisemement.OrderPositionName,
                                                                   advertisemement.OrderPositionId);
                    var firmDescription = GenerateDescription(
                        EntityName.Firm, 
                        advertisemement.FirmName,
                        advertisemement.FirmId);

                    messages.Add(new OrderValidationMessage
                                   {
                                       Type = MessageType.Warning,
                                       OrderId = advertisemement.OrderId,
                                       OrderNumber = advertisemement.OrderNumber,
                                       MessageText = string.Format(
                                       BLResources.ThereIsNoAdvantageousPurchasesCategory, 
                                       firmDescription, 
                                       orderPositionDescription)
                                   });
                }
        }
    }
}
