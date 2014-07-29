using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    /// <summary>
    /// В момент оформления позиции заказ, при использовании функционала "Добавить рубрику" определять кол-во уникальных рубрик у фирмы и выдавать ограничение, если общее кол-во рубрик превышает допустимое.
    /// При занесении рубрик в IR необходимо контролировать максимально допустимое кол-во.
    /// Проверка информационная. Отображается на всех этапах проверок заказа кроме финальной сборки.
    /// </summary>
    [UseCase(Duration = UseCaseDuration.Long)]
    public sealed class CategoriesForFirmAmountOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;

        public CategoriesForFirmAmountOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            const int MaxCategoriesAlowedForFirm = 20;
            var currentFilter = filterPredicate;
            long? firmId = null;

            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (!IsCheckMassive)
            {
                if (request.OrderId == null)
                {
                    throw new ArgumentNullException("OrderId");
                }

                long organizationUnitId;
                currentFilter = GetFilterPredicateToGetLinkedOrders(_finder, request.OrderId.Value, out organizationUnitId, out firmId);
            }
            else
            {
                if (request.OrganizationUnitId == null)
                {
                    throw new ArgumentNullException("OrganizationUnitId");
                }
            }

            var categoriesForFirms =
                _finder.Find(currentFilter)
                      .SelectMany(x => x.OrderPositions)
                      .Where(x => x.IsActive && !x.IsDeleted && (IsCheckMassive || (firmId.HasValue && x.Order.FirmId == firmId.Value)))
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

            categoriesForFirms = categoriesForFirms.Where(x => x.Categories.Select(y => y.CategoryId).Distinct().Count() > MaxCategoriesAlowedForFirm).ToArray();

            foreach (var categoriesForFirm in categoriesForFirms)
            {
                var firmDescription = GenerateDescription(EntityName.Firm, categoriesForFirm.FirmName, categoriesForFirm.FirmId);

                messages.Add(
                    new OrderValidationMessage
                        {
                            Type = MessageType.Warning,
                            MessageText =
                                string.Format(
                                    BLResources.TooManyCategorieForFirm,
                                    firmDescription,
                                    categoriesForFirm.Categories.Select(y => y.CategoryId).Distinct().Count(),
                                    MaxCategoriesAlowedForFirm)
                        });
            }
        }
    }
}
