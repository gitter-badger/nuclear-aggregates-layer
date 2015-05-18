using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.OrderValidation.Rules.Contexts;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;

using NuClear.Model.Common.Entities;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// В момент оформления позиции заказ, при использовании функционала "Добавить рубрику" определять кол-во уникальных рубрик у фирмы и выдавать ограничение, если общее кол-во рубрик превышает допустимое.
    /// При занесении рубрик в IR необходимо контролировать максимально допустимое кол-во.
    /// Проверка информационная. Отображается на всех этапах проверок заказа кроме финальной сборки.
    /// </summary>
    [UseCase(Duration = UseCaseDuration.Long)]
    public sealed class CategoriesForFirmAmountOrderValidationRule : OrderValidationRuleBase<HybridParamsValidationRuleContext>
    {
        private readonly IFinder _finder;

        public CategoriesForFirmAmountOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(HybridParamsValidationRuleContext ruleContext)
        {
            const int MaxCategoriesAlowedForFirm = 20;
            var currentFilter = ruleContext.OrdersFilterPredicate;
            long? firmId = null;

            if (!ruleContext.ValidationParams.IsMassValidation)
            {
                long organizationUnitId;
                currentFilter = GetFilterPredicateToGetLinkedOrders(_finder, ruleContext.ValidationParams.Single.OrderId, out organizationUnitId, out firmId);
            }

            var categoriesForFirms =
                _finder.Find(currentFilter)
                      .SelectMany(x => x.OrderPositions)
                      .Where(x => x.IsActive && !x.IsDeleted && (ruleContext.ValidationParams.IsMassValidation || (firmId.HasValue && x.Order.FirmId == firmId.Value)))
                      .SelectMany(x => x.OrderPositionAdvertisements)
                      .Where(
                          x =>
                          x.CategoryId != null)
                      .Select(
                          x => new { FirmId = x.OrderPosition.Order.FirmId, CategoryId = x.CategoryId.Value, FirmName = x.OrderPosition.Order.Firm.Name })
                      .ToArray()
                      .GroupBy(x => x.FirmId)
                      .Select(x => new
                          {
                              FirmId = x.Key, 

                              x.First().FirmName,
                              Categories = x,
                          })
                      .ToArray();

            return categoriesForFirms
                        .Where(x => x.Categories.Select(y => y.CategoryId).Distinct().Count() > MaxCategoriesAlowedForFirm)
                        .Select(x => 
                            new OrderValidationMessage
                            {
                                Type = MessageType.Warning,
                                MessageText =
                                    string.Format(
                                        BLResources.TooManyCategorieForFirm,
                                        GenerateDescription(ruleContext.ValidationParams.IsMassValidation, EntityType.Instance.Firm(), x.FirmName, x.FirmId),
                                        x.Categories.Select(y => y.CategoryId).Distinct().Count(),
                                        MaxCategoriesAlowedForFirm)
                            });
        }
    }
}
