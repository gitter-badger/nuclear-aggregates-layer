using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using DoubleGis.Erm.BLCore.API.OrderValidation;

using NuClear.Tracing.API;

namespace DoubleGis.Erm.BLCore.OrderValidation.Rules.AssociatedAndDenied
{
    public static class ADPValidationInitializationHelper
    {
        public static IDictionary<long, OrderStateStorage> LoadOrderStates(ADPValidationQueryProvider queryProvider)
        {
            var orderStates = new Dictionary<long, OrderStateStorage>();

            var ordersData = queryProvider.GetOrdersQuery()
                .Select(order => new
                    {
                        OrderId = order.Id,
                        OrderNumber = order.Number,
                        OrderPositions = order.OrderPositions
                                     .Where(orderPosition => orderPosition.IsActive && !orderPosition.IsDeleted)
                                     .Select(orderPosition => orderPosition.Id)
                                     .OrderBy(x => x)
                    })
                .ToArray();

            foreach (var item in ordersData)
            {
                var orderStateStorage = new OrderStateStorage
                {
                    Id = item.OrderId,
                    Number = item.OrderNumber,
                    PositionsIndexes = new Dictionary<long, int>()
                };
                var index = 0;
                foreach (var orderPositionId in item.OrderPositions)
                {
                    orderStateStorage.PositionsIndexes[orderPositionId] = ++index;
                }

                orderStates.Add(item.OrderId, orderStateStorage);
            }
            return orderStates;
        }

        public static IDictionary<long, ADPValidator> LoadValidators(
            ITracer tracer,
            ADPCheckMode checkMode,
            long orderId,
            ADPValidationQueryProvider validationQueryProvider,
            IDictionary<long, OrderStateStorage> orderStates,
            IPriceConfigurationService priceConfigurationService,
            ADPValidationResultBuilder validationResultBuilder,
            IList<OrderValidationMessage> orderValidationMessages)
        {
            var validators = new Dictionary<long, ADPValidator>();

            #region Profiling
            long fetchTime = 0;
            long createTime = 0;

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            #endregion

            //1. Fetch OrderPositionAdvertisements
            var collection = (from orderPosition in validationQueryProvider.GetOrderPositionsQuery()
                              from opa in orderPosition.OrderPositionAdvertisements
                              join order in validationQueryProvider.GetOrdersQuery() on opa.OrderPosition.OrderId equals order.Id
                              select new
                                  {
                                      opa.OrderPosition.OrderId,
                                      opa.Position.Name,
                                      opa.CategoryId,
                                      opa.FirmAddressId,
                                      opa.PositionId,
                                      opa.OrderPositionId,
                                      opa.OrderPosition.PricePosition.PriceId,
                                      opa.OrderPosition.Order.FirmId,
                                      CompositePositionId = opa.OrderPosition.PricePosition.PositionId,
                                      IsChildPosition = opa.OrderPosition.PricePosition.Position.IsComposite &&
                                                        !opa.Position.IsComposite,
                                      ParentPositionName = opa.OrderPosition.PricePosition.Position.Name,
                                      opa.OrderPosition.Order.BeginReleaseNumber,
                                      EndReleaseNumber = opa.OrderPosition.Order.EndReleaseNumberFact,
                                      OrderPositionAdvertisementId = opa.Id
                                  })
                .ToArray();

            #region Profiling
            stopwatch.Stop();
            fetchTime += stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();
            #endregion

            //2. Group OrderPositionAdvertisements by OrderPosition and Position
            var positionStatesDictionary = new Dictionary<KeyValuePair<long, long>, OrderPositionStateStorage>();
            var positionsToLoad = new HashSet<long>();
            var pricesToLoad = new HashSet<long>();

            Func<long, bool> loadPricePositionDataForOrder =
                currentOrderId => (checkMode == ADPCheckMode.SpecificOrder && currentOrderId == orderId)
                                  || checkMode == ADPCheckMode.OrderBeingCancelled
                                  || checkMode == ADPCheckMode.OrderBeingReapproved
                                  || checkMode == ADPCheckMode.Massive;

            foreach (var item in collection)
            {
                int beginReleaseNumber = 0;
                int endReleaseNumber = 0;

                if (checkMode != ADPCheckMode.Massive)
                {
                    beginReleaseNumber = item.BeginReleaseNumber;
                    endReleaseNumber = item.EndReleaseNumber;
                } //otherwise we're considering only a single release

                var linkingObject = new OrderPositionStateStorage.LinkingObjectData(item.CategoryId ?? OrderPositionStateStorage.Missing,
                                                                                    item.FirmAddressId ?? OrderPositionStateStorage.Missing);
                OrderPositionStateStorage orderPositionState;
                var key = new KeyValuePair<long, long>(item.OrderPositionId, item.PositionId);
                if (!positionStatesDictionary.ContainsKey(key))
                {
                    orderPositionState = new OrderPositionStateStorage
                        {
                            OrderPositionId = item.OrderPositionId,
                            PositionId = item.PositionId,
                            Order = orderStates[item.OrderId],
                            PositionName = item.Name,
                            PriceId = item.PriceId,
                            BeginRelaseNumber = beginReleaseNumber,
                            EndReleaseNumber = endReleaseNumber,
                            Type = item.IsChildPosition ? PositionType.Child : PositionType.Simple,
                            OrderPositionAdvertisementId = item.OrderPositionAdvertisementId
                        };

                    positionStatesDictionary[key] = orderPositionState;

                    if (loadPricePositionDataForOrder(item.OrderId))
                    {
                        //Make a record so that we could fetch all required PricePositions at once
                        positionsToLoad.Add(item.PositionId);
                        pricesToLoad.Add(item.PriceId);
                    }

                    var firmId = (long)item.FirmId;
                    if (!validators.ContainsKey(firmId))
                    {
                        validators[firmId] = new ADPValidator(validationResultBuilder, orderStates, priceConfigurationService);
                    }

                    validators[firmId].AddOrderPositionState(orderPositionState);

                    //Now let's check if we're dealing with a child position
                    if (item.IsChildPosition)
                    {
                        var compositePositionKey = new KeyValuePair<long, long>(item.OrderPositionId, item.CompositePositionId);
                        if (!positionStatesDictionary.ContainsKey(compositePositionKey))
                        {
                            var compositePositionData = new OrderPositionStateStorage
                            {
                                OrderPositionId = item.OrderPositionId,
                                PositionId = item.CompositePositionId,
                                Order = orderStates[item.OrderId],
                                PriceId = item.PriceId,
                                PositionName = item.ParentPositionName,
                                BeginRelaseNumber = beginReleaseNumber,
                                EndReleaseNumber = endReleaseNumber,
                                Type = PositionType.Composite
                            };

                            positionStatesDictionary[compositePositionKey] = compositePositionData;
                            validators[firmId].AddOrderPositionState(compositePositionData);

                            if (loadPricePositionDataForOrder(item.OrderId))
                            {
                                //Make a record to fetch composite position's  rules etc.
                                positionsToLoad.Add(item.CompositePositionId);
                            }
                        }

                        orderPositionState.ParentPositionStateStorage = positionStatesDictionary[compositePositionKey];
                    }
                }
                else
                {
                    orderPositionState = positionStatesDictionary[key];
                }

                orderPositionState.AddLinkingObject(linkingObject);
                if (orderPositionState.Type == PositionType.Child)
                {
                    var compositePositionKey = new KeyValuePair<long, long>(item.OrderPositionId, item.CompositePositionId);
                    positionStatesDictionary[compositePositionKey].AddLinkingObject(linkingObject);
                }
            }

            #region Profiling
            stopwatch.Stop();
            createTime += stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();
            #endregion

            //3. Fetch Principal and Denied poisition rules
            priceConfigurationService.LoadConfiguration(pricesToLoad, positionsToLoad, orderValidationMessages);

            #region Profiling
            stopwatch.Stop();
            fetchTime += stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();
            #endregion

            //4. Determine which categories require parent information
            var categories = positionStatesDictionary.Values
                .SelectMany(item => item.LinkingObjects)
                .Where(item => item.CategoryId != OrderPositionStateStorage.Missing)
                .Select(item => item.CategoryId)
                .Distinct()
                .ToArray();

            #region Profiling
            stopwatch.Stop();
            createTime += stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();
            #endregion

            //5. Fetch IDs of categories' parents
            var parentCategories = validationQueryProvider.GetCategoryQuery()
                .Where(x => categories.Contains(x.Id) || x.ChildCategories.Any(child => categories.Contains(child.Id)))
                .Where(x => x.ParentId != null)
                .ToDictionary(x => x.Id, x => x.ParentId ?? 0);

            #region Profiling
            stopwatch.Stop();
            fetchTime += stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();
            #endregion

            foreach (var orderPositionState in positionStatesDictionary.Values)
            {
                orderPositionState.RegisterParentCategories(parentCategories);
            }

            #region Profiling
            stopwatch.Stop();
            createTime += stopwatch.ElapsedMilliseconds;
            tracer.DebugFormat("Проверка СЗП. Загружено Заказов: {0}", orderStates.Count);
            tracer.DebugFormat("Проверка СЗП. Загружено Фирм: {0}", validators.Count);
            tracer.DebugFormat("Проверка СЗП. Загрузка данных: {0:F3}", fetchTime / 1000D);
            tracer.DebugFormat("Проверка СЗП. Создание структур: {0:F3}", createTime / 1000D);
            #endregion

            return validators;
        }
    }
}