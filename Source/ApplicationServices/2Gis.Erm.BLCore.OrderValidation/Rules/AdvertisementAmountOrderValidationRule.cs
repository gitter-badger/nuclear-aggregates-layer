using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.Aggregates.Prices;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using MessageType = DoubleGis.Erm.BLCore.API.OrderValidation.MessageType;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules
{
    public sealed class AdvertisementAmountOrderValidationRule : OrderValidationRuleCommonPredicate
    {
        private readonly IFinder _finder;
        private readonly IPriceRepository _priceRepository;
        private readonly ICommonLog _logger;

        public AdvertisementAmountOrderValidationRule(IFinder finder, IPriceRepository priceRepository, ICommonLog logger)
        {
            _finder = finder;
            _priceRepository = priceRepository;
            _logger = logger;
        }

        protected override void ValidateInternal(ValidateOrdersRequest request, Expression<Func<Order, bool>> filterPredicate, IEnumerable<long> invalidOrderIds, IList<OrderValidationMessage> messages)
        {
            long organizationUnitId;
            long actualPriceId;
            Expression<Func<Order, bool>> orderFilter;
            Expression<Func<PricePosition, bool>> pricePositionFilter;

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

                long? firmId;
                orderFilter = GetFilterPredicateToGetLinkedOrders(_finder, request.OrderId.Value, out organizationUnitId, out firmId);

                var singleOrderCategoryPositionIds =
                    _finder.Find(Specs.Find.ById<Order>(request.OrderId.Value)) // request.OrderId is not null for individual check
                
                           .SelectMany(x => x.OrderPositions.Where(y => y.IsActive && !y.IsDeleted).SelectMany(y => y.OrderPositionAdvertisements))
                           .Select(x => x.Position.CategoryId)
                           .Distinct();

                pricePositionFilter =
                    x => x.IsActive && x.IsDeleted == false && x.Position.IsControlledByAmount && singleOrderCategoryPositionIds.Contains(x.Position.CategoryId);

                // Для единичной проверки берём прайс-лист, связанный с позициями проверяемого заказа
                var prices = _finder.Find(Specs.Find.ById<Order>(request.OrderId.Value)) // request.OrderId is not null for individual check
                                    .SelectMany(order => order.OrderPositions)
                                    .Where(Specs.Find.ActiveAndNotDeleted<OrderPosition>())
                                    .Select(orderPosition => orderPosition.PricePosition.PriceId)
                                    .Distinct()
                                    .ToArray();

                // Если не найдено прайс-листа для проверки, то и проверять нечего
                if (prices.Length == 0)
                {
                    _logger.WarnFormatEx("Для единичной проверки по заказу {0} не было найдено прайс-листов.", request.OrderId);
                    return;
                }

                // Заказ не должен иметь позиции из разных прайс-листов, но так случиться может.
                // Эта проверка не занимается проверкой прайс-листов. Берём первый и вперёд.
                actualPriceId = prices.First();

                if (prices.Length > 1)
                {
                    _logger.WarnFormatEx("Для единичной проверки по заказу {0} было {1} прайс-листов. Использован {2}",
                                         request.OrderId,
                                         prices.Length,
                                         actualPriceId);
                }
            }
            else
            {
                if (request.OrganizationUnitId == null)
                {
                    throw new ArgumentNullException("OrganizationUnitId");
                }

                orderFilter = filterPredicate;

                organizationUnitId = request.OrganizationUnitId.Value;

                pricePositionFilter = x => x.IsActive && x.IsDeleted == false && x.Position.IsControlledByAmount;

                // Для массовой проверки берём прайс-лист, актуальный прайс-лист на указанный в проверке месяц
                if (!_priceRepository.TryGetActualPriceId(organizationUnitId, request.Period.Start, out actualPriceId))
                {
                    throw new NotificationException(string.Format(BLResources.ActualPriceNotFound, organizationUnitId, request.Period.Start));
                }

                _logger.InfoFormatEx("Для массовой проверки по городу {0} за {1} выбран прайс-лист {2}", organizationUnitId, request.Period.Start, actualPriceId);
            }

            var pricePositions = _finder.Find(Specs.Find.ById<Price>(actualPriceId))
                                        .SelectMany(x => x.PricePositions)
                                        .Where(pricePositionFilter)
                                        .OrderBy(x => x.PositionId)
                                        .Select(
                                            x =>
                                            new
                                                {
                                                    x.MinAdvertisementAmount,
                                                    x.MaxAdvertisementAmount,
                                                    x.Position.PositionCategory.Name,
                                                    x.Position.CategoryId
                                                })
                                        .GroupBy(x => x.CategoryId)
                                        .ToDictionary(x => x.Key, x => x.ToList())
                                        .Select(x => new
                                            {
                                                CategoryId = x.Key,
                                                Name = x.Value.First().Name,
                                                MinAdvertisementAmount = x.Value.Max(y => y.MinAdvertisementAmount),
                                                MaxAdvertisementAmount = x.Value.Min(y => y.MaxAdvertisementAmount)
                                            })
                                        .ToArray();

            var categoryIds = pricePositions.Select(x => x.CategoryId).ToArray();

            var orderPositions = _finder.Find(orderFilter)
                                        .Where(x => x.DestOrganizationUnitId == organizationUnitId)
                                        .SelectMany(order => order.OrderPositions)
                                        .Where(
                                            orderPosition =>
                                            orderPosition.IsActive && !orderPosition.IsDeleted)
                                        .SelectMany(x => x.OrderPositionAdvertisements)
                                        .Where(x => categoryIds.Contains(x.Position.CategoryId))
                                        .Select(x =>
                                                new OrderPositionDto
                                                    {
                                                        OrderPositionId = x.OrderPosition.Id,
                                                        PositionName = x.Position.Name,
                                                        OrderId = x.OrderPosition.Order.Id,
                                                        CategoryId = x.Position.CategoryId,
                                                        BeginDistributionDate = x.OrderPosition.Order.BeginDistributionDate,
                                                        EndDistributionDateFact = x.OrderPosition.Order.EndDistributionDateFact,
                                                    })
                                        .GroupBy(x => x.CategoryId)
                                        .ToDictionary(x => x.Key, x => x.ToList());

            DateTime beginCheckPeriod;
            DateTime endCheckPeriod;
            if (IsCheckMassive)
            {
                beginCheckPeriod = request.Period.Start;
                endCheckPeriod = request.Period.End;
            }
            else
            {
                var orderToCheck = _finder.Find(Specs.Find.ById<Order>(request.OrderId.Value)).Single();
                beginCheckPeriod = orderToCheck.BeginDistributionDate;
                endCheckPeriod = orderToCheck.EndDistributionDateFact;
            }

            var advertisementDistributioins = new Dictionary<CategoryCompositeKey, List<OrderPositionDto>>();

            foreach (var sale in orderPositions.SelectMany(x => x.Value))
            {
                for (var i = sale.BeginDistributionDate; i <= sale.EndDistributionDateFact; i = i.AddMonths(1))
                {
                    if (i < beginCheckPeriod || i > endCheckPeriod)
                    {
                        continue;
                    }

                    var key = new CategoryCompositeKey(sale.CategoryId, i);
                    if (advertisementDistributioins.ContainsKey(key))
                    {
                        advertisementDistributioins[key].Add(sale);
                    }
                    else
                    {
                        advertisementDistributioins.Add(key, new List<OrderPositionDto> { sale });
                    }
                }
            }

            foreach (var pricePosition in pricePositions)
            {
                if (pricePosition.MinAdvertisementAmount == null)
                {
                    messages.Add(new OrderValidationMessage
                        {
                            Type = IsCheckMassive ? MessageType.Error : MessageType.Warning,
                            OrderNumber = string.Empty,
                            MessageText = string.Format(BLResources.PricePositionHasNoMinAdvertisementAmount, pricePosition.Name)
                        });

                    continue;
                }

                // проверим количество рекламы в каждом интересующем нас месяце
                for (var monthToCheck = beginCheckPeriod; monthToCheck <= endCheckPeriod; monthToCheck = monthToCheck.AddMonths(1))
                {
                    var key = new CategoryCompositeKey(pricePosition.CategoryId, monthToCheck);
                    if (!advertisementDistributioins.ContainsKey(key) && pricePosition.MinAdvertisementAmount > 0)
                    {
                        messages.Add(GetMessage(pricePosition.Name,
                                                pricePosition.MinAdvertisementAmount.Value,
                                                pricePosition.MaxAdvertisementAmount,
                                        0,
                                        0,
                                        0,
                                                monthToCheck));
                        continue;
                    }

                    if (!advertisementDistributioins.ContainsKey(key))
                    {
                        continue;
                    }

                    var invalidOrderPositionsAmount = !IsCheckMassive
                                                          ? 0
                                                          : advertisementDistributioins[key].Count(x => invalidOrderIds != null && invalidOrderIds.Contains(x.OrderId));
                    var totalOrderPositionsAmount = advertisementDistributioins[key].Count;
                    var validOrderPositionsAmount = totalOrderPositionsAmount - invalidOrderPositionsAmount;

                    if (validOrderPositionsAmount < pricePosition.MinAdvertisementAmount)
                    {
                        var message =
                            GetMessage(pricePosition.Name,
                                       pricePosition.MinAdvertisementAmount.Value,
                                       pricePosition.MaxAdvertisementAmount,
                                       invalidOrderPositionsAmount,
                                            validOrderPositionsAmount,
                                            totalOrderPositionsAmount,
                                       monthToCheck);

                        message.Type = IsCheckMassive ? MessageType.Error : MessageType.Warning;

                        messages.Add(message);
                    }
                    else if (pricePosition.MaxAdvertisementAmount != null &&
                             validOrderPositionsAmount > pricePosition.MaxAdvertisementAmount)
                    {
                        messages.Add(GetMessage(pricePosition.Name,
                                                pricePosition.MinAdvertisementAmount.Value,
                                                pricePosition.MaxAdvertisementAmount,
                                                invalidOrderPositionsAmount,
                                                validOrderPositionsAmount,
                                                totalOrderPositionsAmount,
                                                monthToCheck));
                    }
                }
            }
        }

        private static OrderValidationMessage GetMessage(string positionName,
                                                         int minAdvertisementAmount,
                                                         int? maxAdvertisementAmount,
                                                         int invalidOrderPositionsAmount,
                                                         int validOrderPositionsAmount,
                                                         int totalOrderPositionsAmount,
                                                         DateTime monthToCheck)
                                {
            var message = new OrderValidationMessage
                {
                                    Type = MessageType.Error,
                                    OrderNumber = string.Empty,
                };

            if (invalidOrderPositionsAmount != 0)
            {
                message.MessageText =
                                        string.Format(
                        BLResources.AdvertisementAmountErrorMessage,
                        positionName,
                        minAdvertisementAmount,
                        !maxAdvertisementAmount.HasValue ? BLResources.Unlimited : maxAdvertisementAmount.ToString(),
                                            validOrderPositionsAmount,
                                            totalOrderPositionsAmount,
                                            invalidOrderPositionsAmount,
                        monthToCheck.ToString("MMMM"));
                }
            else
            {
                message.MessageText =
                    string.Format(
                        BLResources.AdvertisementAmountShortErrorMessage,
                        positionName,
                        minAdvertisementAmount,
                        !maxAdvertisementAmount.HasValue ? BLResources.Unlimited : maxAdvertisementAmount.ToString(),
                        monthToCheck.ToString("MMMM"),
                        validOrderPositionsAmount);
            }

            return message;
        }

        private struct CategoryCompositeKey
        {
            public CategoryCompositeKey(long categoryId, DateTime month)
                : this()
            {
                CategoryId = categoryId;
                Month = month;
            }

            private long CategoryId { get; set; }
            private DateTime Month { get; set; }
        }

        private class OrderPositionDto
        {
            public long OrderPositionId { get; set; }
            public string PositionName { get; set; }
            public long OrderId { get; set; }
            public long CategoryId { get; set; }
            public DateTime BeginDistributionDate { get; set; }
            public DateTime EndDistributionDateFact { get; set; }
        }
    }
}
