using System;
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
    /// Проверка на наличие позиции заказа с позицией прайс листа привязанной к категории позиции "Объявление в рубрике(Объявление под списком выдачи)" 
    /// более чем 2-х в одной и той же рубрике, [CategoryId] у которых совпадают.
    /// </summary>
    public sealed class AdvertisementForCategoryAmountOrderValidationRule : OrderValidationRuleBase<HybridParamsValidationRuleContext>
    {
        private const int AdForRubricPositionCategoryId = 38;
        private const int AdsPerCategory = 2;

        private readonly IQuery _query;

        public AdvertisementForCategoryAmountOrderValidationRule(IQuery query)
        {
            _query = query;
        }

        protected override IEnumerable<OrderValidationMessage> Validate(HybridParamsValidationRuleContext ruleContext)
        {
            var currentFilter = ruleContext.OrdersFilterPredicate;
            
            long organizationUnitId;
            if (!ruleContext.ValidationParams.IsMassValidation)
            {
                long? firmId;
                currentFilter = GetFilterPredicateToGetLinkedOrders(_query, ruleContext.ValidationParams.Single.OrderId, out organizationUnitId, out firmId);
            }
            else
            {
                organizationUnitId = ruleContext.ValidationParams.Mass.OrganizationUnitId;
            }

            var categories =
                _query.For<Order>()
                      .Where(currentFilter)
                      .Where(x => x.DestOrganizationUnitId == organizationUnitId)
                      .SelectMany(order => order.OrderPositions)
                      .Where(
                             orderPosition =>
                             orderPosition.IsActive && !orderPosition.IsDeleted)
                      .SelectMany(
                                  orderPosition =>
                                  orderPosition.OrderPositionAdvertisements.Where(
                                                                                  x =>
                                                                                  x.CategoryId != null &&
                                                                                  x.Position.CategoryId == AdForRubricPositionCategoryId)
                                               .Select(x => new
                                                   {
                                                       x.OrderPosition.OrderId,
                                                       x.OrderPosition.Order.BeginDistributionDate,
                                                       x.OrderPosition.Order.EndDistributionDateFact,
                                                       CategoryId = x.CategoryId.Value,
                                                       x.Category.Name
                                                   }))
                      .GroupBy(x => x.CategoryId)
                      .Where(x => x.Count() > AdsPerCategory)
                      .ToList();

            // Для единичной проверки исключим продажи, которые не касаются проверяемого заказа
            if (!ruleContext.ValidationParams.IsMassValidation)
            {
                categories.RemoveAll(x => x.All(y => y.OrderId != ruleContext.ValidationParams.Single.OrderId));
            }

            var advertisementDistributioins = new Dictionary<CategoryCompositeKey, int>();

            foreach (var sale in categories.SelectMany(category => category))
            {
                for (var i = sale.BeginDistributionDate; i <= sale.EndDistributionDateFact; i = i.AddMonths(1))
                {
                    if (ruleContext.ValidationParams.IsMassValidation && ruleContext.ValidationParams.Mass.Period.Start != i)
                    {
                        continue;
                    }

                    var key = new CategoryCompositeKey(sale.CategoryId, i);
                    if (advertisementDistributioins.ContainsKey(key))
                    {
                        advertisementDistributioins[key]++;
                    }
                    else
                    {
                        advertisementDistributioins.Add(key, 1);
                    }
                }
            }

            var overSales = advertisementDistributioins.Where(x => x.Value > AdsPerCategory).Select(x => x.Key).ToArray();

            // Исключаем те рубрики, по которым не было избытка продаж в определенных месяцах
            categories.RemoveAll(x => overSales.All(y => y.CategoryId != x.Key));

            return categories.Select(x => new OrderValidationMessage
                                              {
                                                  Type = ruleContext.ValidationParams.IsMassValidation ? MessageType.Error : MessageType.Warning,
                                                  MessageText =
                                                      string.Format(BLResources.TooManyAdvertisementForCategory,
                                                                    GenerateDescription(ruleContext.ValidationParams.IsMassValidation, EntityType.Instance.Category(), x.First().Name, x.Key),
                                                                    advertisementDistributioins.Where(ad => ad.Key.CategoryId == x.Key).Max(ad => ad.Value),
                                                                    AdsPerCategory)
                                              });
        }

        private struct CategoryCompositeKey
        {
            public CategoryCompositeKey(long categoryId, DateTime month) : this()
            {
                CategoryId = categoryId;
                Month = month;
            }

            public long CategoryId { get; private set; }
            private DateTime Month { get; set; }
        }
    }
}
