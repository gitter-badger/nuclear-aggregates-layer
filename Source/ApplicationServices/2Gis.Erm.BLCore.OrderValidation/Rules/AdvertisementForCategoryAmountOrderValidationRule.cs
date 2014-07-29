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
    /// Проверка на наличие позиции заказа с позицией прайс листа привязанной к категории позиции "Объявление в рубрике(Объявление под списком выдачи)" 
    /// более чем 2-х в одной и той же рубрике, [CategoryId] у которых совпадают.
    /// </summary>
    public sealed class AdvertisementForCategoryAmountOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private const int AdForRubricPositionCategoryId = 38;
        private const int AdsPerCategory = 2;

        private readonly IFinder _finder;

        public AdvertisementForCategoryAmountOrderValidationRule(IFinder finder)
        {
            _finder = finder;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            var currentFilter = filterPredicate;

            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            long organizationUnitId;
            if (!IsCheckMassive)
            {
                if (request.OrderId == null)
                {
                    throw new ArgumentNullException("OrderId");
                }

                long? firmId;
                
                currentFilter = GetFilterPredicateToGetLinkedOrders(_finder, request.OrderId.Value, out organizationUnitId, out firmId);
            }
            else
            {
                if (request.OrganizationUnitId == null)
                {
                    throw new ArgumentNullException("OrganizationUnitId");
                }

                organizationUnitId = request.OrganizationUnitId.Value;
            }

            var categories =
                _finder.Find(currentFilter)
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
            if (!IsCheckMassive)
            {
                categories.RemoveAll(x => x.All(y => y.OrderId != request.OrderId));
            }

            var advertisementDistributioins = new Dictionary<CategoryCompositeKey, int>();

            foreach (var sale in categories.SelectMany(category => category))
            {
                for (var i = sale.BeginDistributionDate; i <= sale.EndDistributionDateFact; i = i.AddMonths(1))
                {
                    if (IsCheckMassive && request.Period.Start != i)
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

            foreach (var category in categories)
            {
                var categoryDescription = GenerateDescription(EntityName.Category, category.First().Name, category.Key);

                messages.Add(
                             new OrderValidationMessage
                                 {
                                     Type = IsCheckMassive ? MessageType.Error : MessageType.Warning,
                                     MessageText =
                                         string.Format(BLResources.TooManyAdvertisementForCategory,
                                                       categoryDescription,
                                                       advertisementDistributioins.Where(x => x.Key.CategoryId == category.Key).Max(x => x.Value),
                                                       AdsPerCategory)
                                 });
            }
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
