using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

using DoubleGis.Erm.BLCore.Aggregates.Positions;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel.DTO;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel
{
    public sealed class OrderReadModel : IOrderReadModel
    {
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IFinder _finder;
        private readonly ISecureFinder _secureFinder;
        private readonly IUserContext _userContext;

        public OrderReadModel(IFinder finder, ISecureFinder secureFinder, ISecurityServiceEntityAccess entityAccessService, IUserContext userContext)
        {
            _finder = finder;
            _secureFinder = secureFinder;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
        }

        public IReadOnlyDictionary<long, byte[]> GetOrdersCurrentVersions(Expression<Func<Order, bool>> ordersPredicate)
        {
            return _finder.FindAll<Order>().Where(ordersPredicate).ToDictionary(x => x.Id, x => x.Timestamp);
        }

        public IReadOnlyDictionary<long, IEnumerable<long>> GetRelatedOrdersByFirm(IEnumerable<long> orderIds)
        {
            return _finder.Find(Specs.Find.ByIds<Order>(orderIds))
                          .Select(o => new
                                            {
                                                OrderId = o.Id,
                                                RelatedOrderIds = o.Firm.Orders.Where(x => x.Id != o.Id).Select(x => x.Id)
                                            })
                          .ToDictionary(x => x.OrderId, x => x.RelatedOrderIds);
        }

        public IEnumerable<OrderReleaseInfo> GetOrderReleaseInfos(long organizationUnitId, TimePeriod period)
        {
            return _finder.Find(OrderSpecs.Orders.Find.ForRelease(organizationUnitId, period) && Specs.Find.ActiveAndNotDeleted<Order>())
                          .Select(o => new OrderReleaseInfo
                                           {
                                               OrderId = o.Id,
                                               OrderNumber = o.Number,
                                               AccountId = o.AccountId
                                                           ?? o.BranchOfficeOrganizationUnit.Accounts
                                                               .Where(x => x.IsActive && !x.IsDeleted && x.LegalPersonId == o.LegalPersonId)
                                                               .Select(x => (long?)x.Id)
                                                               .FirstOrDefault(),
                                               PriceId = o.OrderPositions.Select(p => p.PricePosition.PriceId).FirstOrDefault(),
                                               AmountToWithdrawSum = o.OrderReleaseTotals
                                                                      .Where(x => x.ReleaseBeginDate == period.Start && x.ReleaseEndDate == period.End)
                                                                      .Select(x => x.AmountToWithdraw)
                                                                      .FirstOrDefault(),
                                               OrderPositions = o.OrderPositions.Where(op => op.IsActive && !op.IsDeleted)
                                                                 .Select(op => new OrderPositionReleaseInfo
                                                                                   {
                                                                                       OrderPositionId = op.Id,
                                                                                       AmountToWithdraw = op.ReleasesWithdrawals.Where(rw =>
                                                                                                                                       rw.ReleaseBeginDate == period.Start &&
                                                                                                                                       rw.ReleaseEndDate == period.End)
                                                                                                            .Select(rw => rw.AmountToWithdraw)
                                                                                                            .FirstOrDefault(),
                                                                                       IsPlannedProvision =
                                                                                           SalesModelUtil.PlannedProvisionSalesModels.Contains(op.PricePosition.Position.SalesModel)
                                                                                   })
                                           })
                          .ToArray();
        }

        public IEnumerable<long> GetOrderIdsForRelease(long organizationUnitId, TimePeriod period)
        {
            return _finder.Find(OrderSpecs.Orders.Find.ForRelease(organizationUnitId, period) &&
                                Specs.Find.ActiveAndNotDeleted<Order>())
                          .Select(x => x.Id)
                          .ToArray();
        }

        public OrderValidationAdditionalInfo[] GetOrderValidationAdditionalInfos(IEnumerable<long> orderIds)
        {
            var orderInfos = _finder.Find(Specs.Find.ByIds<Order>(orderIds))
                                    .Select(o => new
                                        {
                                            o.Id,
                                            o.Number,
                                            o.OwnerCode,
                                            SourceOrganizationUnitName = o.SourceOrganizationUnit.Name,
                                            DestOrganizationUnitName = o.DestOrganizationUnit.Name,
                                            FirmName = o.Firm.Name,
                                            LegalPersonName = o.LegalPerson.ShortName
                                        })
                                    .ToArray();

            // есть вариант дальнейшее вытягивание инфы по пользователю делать через ISecurityServiceUserIdentifier.GetUserInfo(int? userCode),
            // но т.к. это будет делаться последовательными запросами по одной записи пока от этого отказались
            var userInfos = _finder.Find(Specs.Find.ByIds<User>(orderInfos.Select(i => i.OwnerCode)))
                                   .Select(u => new
                                       {
                                           u.Id,
                                           u.DisplayName
                                       });
            return orderInfos.GroupJoin(userInfos,
                                        orderInfo => orderInfo.OwnerCode,
                                        userInfo => userInfo.Id,
                                        (order, users) => new OrderValidationAdditionalInfo
                                            {
                                                Id = order.Id,
                                                Number = order.Number,
                                                SourceOrganizationUnitName = order.SourceOrganizationUnitName,
                                                DestOrganizationUnitName = order.DestOrganizationUnitName,
                                                FirmName = order.FirmName,
                                                LegalPersonName = order.LegalPersonName,
                                                OwnerName = users.Select(u => u.DisplayName).FirstOrDefault() ?? string.Empty,
                                            })
                             .ToArray();
        }

        public IEnumerable<Order> GetOrdersCompletelyReleasedBySourceOrganizationUnit(long sourceOrganizationUnitId)
        {
            return _finder
                .Find(OrderSpecs.Orders.Find.CompletelyReleasedByOrganizationUnit(sourceOrganizationUnitId))
                .ToArray();
        }

        public IEnumerable<OrderWithDummyAdvertisementDto> GetOrdersWithDummyAdvertisement(long organizationUnitId, long ownerCode, bool includeOwnerDescendants)
        {
            var dummyAdvertisements =
                _finder.Find<AdvertisementTemplate>(x => !x.IsDeleted).Select(x => x.DummyAdvertisementId).Where(x => x.HasValue).ToArray();

            var userDescendantsQuery = _finder.FindAll<UsersDescendant>();

            var orderInfos = _finder.Find<Order>(
                                                 x =>
                                                 x.IsActive && !x.IsDeleted && (x.OwnerCode == ownerCode || (includeOwnerDescendants &&
                                                                                                             userDescendantsQuery.Any(
                                                                                                                                      ud =>
                                                                                                                                      ud.AncestorId == ownerCode &&
                                                                                                                                      ud.DescendantId ==
                                                                                                                                      x.OwnerCode))) &&
                                                 (x.SourceOrganizationUnitId == organizationUnitId || x.DestOrganizationUnitId == organizationUnitId) &&
                                                 x.OrderPositions.Any(
                                                                      y =>
                                                                      y.IsActive && !y.IsDeleted &&
                                                                      y.OrderPositionAdvertisements.Any(z => dummyAdvertisements.Contains(z.AdvertisementId))))
                                    .Select(x => new
                                        {
                                            x.Id,
                                            DestOrganizationUnitName = x.DestOrganizationUnit.Name,
                                            SourceOrganizationUnitName = x.SourceOrganizationUnit.Name,
                                            FirmName = x.Firm.Name,
                                            LegalPersonName = x.LegalPerson.LegalName,
                                            x.Number,
                                            x.BeginDistributionDate,
                                            x.WorkflowStepId,
                                            x.OwnerCode
                                        }).ToArray();

            var userInfos = _finder.Find(Specs.Find.ByIds<User>(orderInfos.Select(x => x.OwnerCode)))
                                   .Select(u => new
                                       {
                                           u.Id,
                                           u.DisplayName
                                       });

            return orderInfos.GroupJoin(userInfos,
                                        orderInfo => orderInfo.OwnerCode,
                                        userInfo => userInfo.Id,
                                        (order, users) => new OrderWithDummyAdvertisementDto
                                            {
                                                Id = order.Id,
                                                Number = order.Number,
                                                SourceOrganizationUnitName = order.SourceOrganizationUnitName,
                                                DestOrganizationUnitName = order.DestOrganizationUnitName,
                                                OwnerName = users.Select(u => u.DisplayName).FirstOrDefault() ?? string.Empty,
                                                BeginDistributionDate = order.BeginDistributionDate,
                                                WorkflowStep =
                                                    (order.WorkflowStepId).ToStringLocalized(EnumResources.ResourceManager,
                                                                                                         CultureInfo.CurrentCulture),
                                                FirmName = order.FirmName,
                                            })
                             .ToArray();
        }

        public IDictionary<long, string> PickInactiveOrDeletedOrderPositionNames(IEnumerable<long> orderPositionIds)
        {
            return _finder.Find(Specs.Find.ByIds<OrderPosition>(orderPositionIds) && Specs.Find.InactiveOrDeletedEntities<OrderPosition>())
                          .Select(x => new
                                           {
                                               Id = x.Id,
                                               Name = x.PricePosition.Position.Name
                                           })
                          .ToDictionary(x => x.Id, y => y.Name);
        }

        public IEnumerable<long> GetExistingOrderPositionIds(IEnumerable<long> orderPositionIds)
        {
            return _finder.Find(Specs.Find.ByIds<OrderPosition>(orderPositionIds))
                          .Select(x => x.Id)
                          .ToArray();
        }

        public Dictionary<long, Dictionary<PlatformEnum, decimal>> GetOrderPlatformDistributions(
            IEnumerable<long> orderIds,
            DateTime startPeriodDate,
            DateTime endPeriodDate)
        {
            return _finder.Find<Order>(x => orderIds.Contains(x.Id)).Select(x => new
                {
                    x.Id,
                    distributions = x.OrderPositions.Where(y => y.IsActive && !y.IsDeleted)
                                     .SelectMany(
                                                 y =>
                                                 y.ReleasesWithdrawals.Where(
                                                                             z => z.ReleaseBeginDate == startPeriodDate && z.ReleaseEndDate == endPeriodDate))
                                     .SelectMany(z => z.ReleasesWithdrawalsPositions)
                                     .GroupBy(y => y.Platform.DgppId)
                                     .Select(y => new
                                         {
                                             Platform = (PlatformEnum)y.Key,
                                             AmountToWithdraw = y.Sum(z => z.AmountToWithdraw)
                                         })
                }).ToDictionary(x => x.Id, x => x.distributions.ToDictionary(y => y.Platform, y => y.AmountToWithdraw));
        }

        public long? EvaluateOrderPlatformId(long orderId)
        {
            var platformIds = _finder.Find(Specs.Find.ById<Order>(orderId))
                          .SelectMany(o => o.OrderPositions)
                          .Where(item => item.IsActive && !item.IsDeleted)
                          .Select(item => item.PricePosition.Position.PlatformId)
                          .Distinct()
                          .ToArray(); 
            
            var platformId = platformIds.Count() > 1
                                 ? _finder.Find<Platform.Model.Entities.Erm.Platform>(x => x.DgppId == (long)PlatformEnum.Independent).Single().Id
                                 : platformIds.FirstOrDefault();

           return platformId == 0 ? null : platformId as long?;
        }

        public OrderNumberDto EvaluateOrderNumbers(string orderNumber, string orderRegionalNumber, long? orderPlatformId)
        {
            const string MobilePostfix = "-Mobile";
            const string APIPostfix = "-API";
            const string OnlinePostfix = "-Online";

            var orderNumberRegex = new Regex(@"-[a-zA-Z]+", RegexOptions.Singleline | RegexOptions.Compiled);
            var numberMatch = orderNumberRegex.Match(orderNumber);
            if (!string.IsNullOrEmpty(orderRegionalNumber))
            {
                // Если один из номеров удовлетворяет формату, а второй задан и не удовлетворяет - это ошибка
                var regionalNumberMatch = orderNumberRegex.Match(orderRegionalNumber);
                var isNumbersFormatMatch = (numberMatch.Success && regionalNumberMatch.Success) || (!numberMatch.Success && !regionalNumberMatch.Success);
                if (!isNumbersFormatMatch)
                {
                    throw new ArgumentException(BLResources.OrderNumberAndRegionalNumberFormatsDoesNotMatch);
                }
            }

            // todo {all, 2013-07-24}: Если в рамках задачи ERM-104 свершится отказ от колонки DgppId, здесь не потребуется выборка
            //                         Кроме того, этот метод перестанет контактировать с хранилищем данных и его можно будет убрать из репозитория
            var orderPlatformType = orderPlatformId.HasValue
                                        ? (PlatformEnum?)_finder.Find(Specs.Find.ById<Platform.Model.Entities.Erm.Platform>(orderPlatformId.Value)).Single().DgppId
                                        : null;

            // Имеем схему вариантов (есть/нет суффикс платформы, есть/нет платформа):
            OrderNumberStates orderState = 0;
            orderState |= orderPlatformId.HasValue ? OrderNumberStates.HasPlatform : (OrderNumberStates)0;
            orderState |= numberMatch.Success ? OrderNumberStates.HasSuffix : (OrderNumberStates)0;

            switch (orderState)
            {
                case OrderNumberStates.HasSuffixHasPlatform:
                    // Если постфикс для платформы задан - обновляем
                    switch (orderPlatformType)
                    {
                        case PlatformEnum.Mobile:
                            return new OrderNumberDto
                            {
                                Number = orderNumberRegex.Replace(orderNumber, MobilePostfix),
                                RegionalNumber = string.IsNullOrEmpty(orderRegionalNumber)
                                                     ? null
                                                     : orderNumberRegex.Replace(orderRegionalNumber, MobilePostfix)
                            };

                        case PlatformEnum.Api:
                            return new OrderNumberDto
                            {
                                Number = orderNumberRegex.Replace(orderNumber, APIPostfix),
                                RegionalNumber = string.IsNullOrEmpty(orderRegionalNumber)
                                                     ? null
                                                     : orderNumberRegex.Replace(orderRegionalNumber, APIPostfix)
                            };
                        case PlatformEnum.Online:
                            return new OrderNumberDto
                            {
                                Number = orderNumberRegex.Replace(orderNumber, OnlinePostfix),
                                RegionalNumber = string.IsNullOrEmpty(orderRegionalNumber)
                                                     ? null
                                                     : orderNumberRegex.Replace(orderRegionalNumber, OnlinePostfix)
                            };
                        default:
                            return new OrderNumberDto
                            {
                                Number = orderNumberRegex.Replace(orderNumber, string.Empty),
                                RegionalNumber = string.IsNullOrEmpty(orderRegionalNumber)
                                                     ? null
                                                     : orderNumberRegex.Replace(orderRegionalNumber, string.Empty)
                            };
                    }

                case OrderNumberStates.NoSuffixHasPlatform:
                    // Если постфикс для платформы не задан - добавляем
                    switch (orderPlatformType)
                    {
                        case PlatformEnum.Mobile:
                            return new OrderNumberDto
                            {
                                Number = orderNumber + MobilePostfix,
                                RegionalNumber = string.IsNullOrEmpty(orderRegionalNumber)
                                                     ? null
                                                     : orderRegionalNumber + MobilePostfix
                            };
                        case PlatformEnum.Api:
                            return new OrderNumberDto
                            {
                                Number = orderNumber + APIPostfix,
                                RegionalNumber = string.IsNullOrEmpty(orderRegionalNumber)
                                                     ? null
                                                     : orderRegionalNumber + APIPostfix
                            };
                        case PlatformEnum.Online:
                            return new OrderNumberDto
                            {
                                Number = orderNumber + OnlinePostfix,
                                RegionalNumber = string.IsNullOrEmpty(orderRegionalNumber)
                                                     ? null
                                                     : orderRegionalNumber + OnlinePostfix
                            };
                        default:
                            return new OrderNumberDto
                            {
                                Number = orderNumber,
                                RegionalNumber = orderRegionalNumber
                            };
                    }

                case OrderNumberStates.HasSuffixNoPlatform:
                    // Если постфикс для платформы задан - убираем его
                    return new OrderNumberDto
                    {
                        Number = orderNumberRegex.Replace(orderNumber, string.Empty),
                        RegionalNumber = string.IsNullOrEmpty(orderRegionalNumber)
                                             ? null
                                             : orderNumberRegex.Replace(orderRegionalNumber, string.Empty)
                    };

                case OrderNumberStates.NoSuffixNoPlatform:
                default:
                    return new OrderNumberDto
                    {
                        Number = orderNumber,
                        RegionalNumber = orderRegionalNumber
                    };
            }
        }

        public IEnumerable<Order> GetActiveOrdersForLegalPerson(long legalPersonId)
        {
            return
                _finder
                    .Find(Specs.Find.ActiveAndNotDeleted<Order>()
                          && OrderSpecs.Orders.Find.ForLegalPerson(legalPersonId))
                    .ToArray();
        }

        public Order GetOrderByBill(long billId)
        {
            return _finder.Find<Order>(x => x.IsActive && !x.IsDeleted && x.Bills.Any(y => y.Id == billId)).FirstOrDefault();
        }

        public OrderWithBillsDto GetOrderWithBills(long orderId)
        {
            return
                _finder.Find(Specs.Find.ById<Order>(orderId))
                       .Select(x => new OrderWithBillsDto { Order = x, Bills = x.Bills.Where(y => y.IsActive && !y.IsDeleted) })
                       .SingleOrDefault();
        }

        public OrderPosition[] GetPositions(long orderId)
        {
            // сортируем от меньшего к большему для лучшей точности вычислений
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .SelectMany(x => x.OrderPositions)
                          .Where(Specs.Find.ActiveAndNotDeleted<OrderPosition>())
                          .OrderBy(x => x.PricePerUnit)
                          .ToArray();
        }

        // todo {all, 2013-07-24}: метод не контактирует с репозиторием
        public RecalculatedOrderPositionDataDto Recalculate(
            int amount,
            decimal pricePerUnit,
            decimal pricePerUnitWithVat,
            int orderReleaseCountFact,
            bool calculateInPercent,
            decimal newDiscountPercent,
            decimal newDiscountSum)
        {
            var result = new RecalculatedOrderPositionDataDto();

            result.ShipmentPlan = amount * orderReleaseCountFact;
            result.PayablePrice = pricePerUnit * result.ShipmentPlan;
            if (result.PayablePrice == 0m)
            {
                result.DiscountSum = 0m;
                result.DiscountPercent = 0m;
                result.PayablePlan = 0m;
                result.PayablePlanWoVat = 0m;
                return result;
            }

            var payablePriceWithVat = Math.Round(pricePerUnitWithVat * result.ShipmentPlan, 2, MidpointRounding.ToEven);

            // discounts
            var exactDiscountSum = calculateInPercent ? (payablePriceWithVat * newDiscountPercent) / 100m : Math.Min(payablePriceWithVat, newDiscountSum);
            result.DiscountSum = Math.Round(exactDiscountSum, 2, MidpointRounding.ToEven);
            result.DiscountPercent = (exactDiscountSum * 100m) / payablePriceWithVat;

            result.PayablePlan = Math.Round(payablePriceWithVat - result.DiscountSum, 2, MidpointRounding.ToEven);
            result.PayablePlanWoVat = (pricePerUnit != 0m)
                                          ? result.PayablePlan / (pricePerUnitWithVat / pricePerUnit)
                                          : 0m;

            result.PayablePlanWoVat = Math.Round(result.PayablePlanWoVat, 2, MidpointRounding.ToEven);

            return result;
        }

        public Note GetLastNoteForOrder(long orderId, DateTime sinceDate)
        {
            var orderTypeId = EntityType.Instance.Order().Id;
            return _finder.Find<Note>(x => x.ParentId == orderId && x.ParentType == orderTypeId && x.ModifiedOn > sinceDate)
                          .OrderByDescending(x => x.ModifiedOn)
                          .FirstOrDefault();
        }

        public bool IsOrganizationUnitsBothBranches(long sourceOrganizationUnitId, long destOrganizationUnitId)
        {
            if (sourceOrganizationUnitId == destOrganizationUnitId)
            {
                throw new InvalidOperationException("sourceOrganizationUnitId and destOrganizationUnitId should be not equal.");
            }

            var contributionTypes = GetBranchOfficesContributionTypes(sourceOrganizationUnitId, destOrganizationUnitId);

            var destContribType = contributionTypes[destOrganizationUnitId];
            var sourceContribType = contributionTypes[sourceOrganizationUnitId];

            if (!destContribType.HasValue)
            {
                throw new ArgumentException(BLResources.SpecifiedDestOrgUnitHaventBranchOfficeOrgUnitForRegSales);
            }

            if (!sourceContribType.HasValue)
            {
                throw new ArgumentException(BLResources.SpecifiedSourceOrgUnitHaventBranchOfficeOrgUnitForRegSales);
            }

            return destContribType.Value == ContributionTypeEnum.Branch && sourceContribType.Value == ContributionTypeEnum.Branch;
        }

        /// <summary>
        /// Проверка на связь "Филиал-филиал".
        /// </summary>
        public bool CheckIsBranchToBranchOrder(long sourceOrganizationUnitId, long destOrganizationUnitId, bool throwOnAbsentContribType)
        {
            if (sourceOrganizationUnitId == destOrganizationUnitId)
            {
                throw new ArgumentException("sourceOrganizationUnitId and destOrganizationUnitId should be not equal.");
            }

            var contributionTypes = GetBranchOfficesContributionTypes(sourceOrganizationUnitId, destOrganizationUnitId);
            var destContribType = contributionTypes[destOrganizationUnitId];
            var sourceContribType = contributionTypes[sourceOrganizationUnitId];

            if (throwOnAbsentContribType && destContribType == null)
            {
                throw new ArgumentException(BLResources.SpecifiedDestOrgUnitHaventBranchOfficeOrgUnitForRegSales);
            }

            if (throwOnAbsentContribType && sourceContribType == null)
            {
                throw new ArgumentException(BLResources.SpecifiedSourceOrgUnitHaventBranchOfficeOrgUnitForRegSales);
            }

            return destContribType.HasValue && destContribType.Value == ContributionTypeEnum.Branch &&
                   sourceContribType.HasValue && sourceContribType.Value == ContributionTypeEnum.Branch;
        }

        public bool IsBranchToBranchOrder(Order order)
        {
            return CheckIsBranchToBranchOrder(order.SourceOrganizationUnitId, order.DestOrganizationUnitId, true);
        }

        public bool TryGetActualPriceIdForOrder(long orderId, out long actualPriceId)
        {
            var orderInfo = _finder.Find(Specs.Find.ById<Order>(orderId))
                                   .Select(x => new
                                       {
                                           x.DestOrganizationUnitId,
                                           x.BeginDistributionDate
                                       }).FirstOrDefault();

            if (orderInfo == null)
            {
                actualPriceId = 0;
                return false;
            }

            return TryGetActualPriceId(orderInfo.DestOrganizationUnitId, orderInfo.BeginDistributionDate, out actualPriceId);
        }

        public bool TryGetActualPriceId(long organizationUnitId, DateTime beginDistributionDate, out long actualPriceId)
        {
            var priceId = _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId))
                                 .SelectMany(
                                             x =>
                                             x.Prices.Where(
                                                            y => y.IsActive && !y.IsDeleted && y.IsPublished && y.BeginDate <= beginDistributionDate))
                                 .OrderByDescending(y => y.BeginDate)
                                 .Select(price => price.Id).FirstOrDefault();

            actualPriceId = priceId;
            return priceId != 0;
        }

        public Order GetOrderSecure(long orderId)
        {
            return _secureFinder.FindOne(Specs.Find.ById<Order>(orderId));
        }

        public OrderLinkingObjectsDto GetOrderLinkingObjectsDto(long orderId)
        {
            var dto = _finder.Find(Specs.Find.ById<Order>(orderId))
                             .Select(order => new OrderLinkingObjectsDto
                                                  {
                                                      FirmId = order.FirmId,
                                                      DestOrganizationUnitId = order.DestOrganizationUnitId,
                                                      BeginDistributionDate = order.BeginDistributionDate,
                                                      EndDistributionDatePlan = order.EndDistributionDatePlan,
                                                      ReleaseCountFact = order.ReleaseCountFact,
                                                      ReleaseCountPlan = order.ReleaseCountPlan,
                                                  })
                             .SingleOrDefault();

            if (dto == null)
            {
                throw new EntityNotFoundException(typeof(Order), orderId);
            }

            return dto;
        }

        public bool OrderPriceWasPublished(long organizationUnitId, DateTime orderBeginDistributionDate)
        {
            return _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId))
                          .SelectMany(unit => unit.Prices)
                          .Where(Specs.Find.ActiveAndNotDeleted<Price>())
                          .Any(price => price.IsPublished && price.BeginDate <= orderBeginDistributionDate);
        }

        public OrderForProlongationDto GetOrderForProlongationInfo(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId)).Select(x => new OrderForProlongationDto
                {
                    OrderId = x.Id,
                    OrderType = x.OrderType,
                    SourceOrganizationUnitId = x.SourceOrganizationUnitId,
                    DestOrganizationUnitId = x.DestOrganizationUnitId,
                    FirmId = x.FirmId,
                    EndDistributionDateFact = x.EndDistributionDateFact,
                    ReleaseCountFact = x.ReleaseCountFact,
                    Positions = x.OrderPositions
                                 .Where(y => y.IsActive && !y.IsDeleted)
                                 .Select(y =>
                                         new OrderPositionForProlongationDto
                                             {
                                                 CalculateDiscountViaPercent = y.CalculateDiscountViaPercent,
                                                 PositionId = y.PricePosition.PositionId,
                                                 DiscountPercent = y.DiscountPercent,
                                                 DiscountSum = y.DiscountSum,
                                                 Amount = y.Amount
                                             })
                }).Single();
        }

        public OrderState GetOrderState(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId)).Select(x => x.WorkflowStepId).Single();
        }

        public OrderType GetOrderType(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId)).Select(x => x.OrderType).Single();
        }

        public OrderPositionWithAdvertisementsDto[] GetOrderPositionsWithAdvertisements(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .SelectMany(order => order.OrderPositions)
                          .Where(Specs.Find.ActiveAndNotDeleted<OrderPosition>())
                          .Select(position => new OrderPositionWithAdvertisementsDto
                              {
                                  OrderPosition = position,
                                  Advertisements = position.OrderPositionAdvertisements,
                                  IsComposite = position.PricePosition.Position.IsComposite,
                                  BindingObjectType = position.PricePosition.Position.BindingObjectTypeEnum
                              })
                          .ToArray();
        }

        public IEnumerable<OrderPosition> GetOrderPositions(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .SelectMany(order => order.OrderPositions)
                          .Where(Specs.Find.ActiveAndNotDeleted<OrderPosition>())
                          .ToList();
        }

        public IDictionary<long, string> GetOrderOrganizationUnitsSyncCodes(params long[] organizationUnitId)
        {
            return _finder.Find<OrganizationUnit>(unit => organizationUnitId.Contains(unit.Id))
                          .ToDictionary(unit => unit.Id, unit => unit.SyncCode1C);
        }

        public IEnumerable<RelatedOrderDescriptor> GetRelatedOrdersToCreateBill(long orderId)
        {
            var modelOrder = _finder.Find<Order>(o => o.Id == orderId && o.IsActive && !o.IsDeleted).Single();
            var relatedOrders = (from order in _finder.FindAll<Order>()
                                 join sou in _finder.FindAll<OrganizationUnit>() on order.SourceOrganizationUnitId equals sou.Id
                                 join dou in _finder.FindAll<OrganizationUnit>() on order.DestOrganizationUnitId equals dou.Id
                                 join bill in _finder.Find<Bill>(b => b.IsActive && !b.IsDeleted) on order.Id equals bill.OrderId into
                                     orderBills
                                 from orderBill in orderBills.DefaultIfEmpty()
                                 where
                                     order.IsActive && !order.IsDeleted && order.LegalPersonId == modelOrder.LegalPersonId
                                     && order.BranchOfficeOrganizationUnitId == modelOrder.BranchOfficeOrganizationUnitId
                                     && order.BeginDistributionDate == modelOrder.BeginDistributionDate
                                     && order.EndDistributionDatePlan == modelOrder.EndDistributionDatePlan && orderBill == null && order.Id != modelOrder.Id
                                 orderby dou.Name ascending
                                 select
                                     new RelatedOrderDescriptor
                                         {
                                             Id = order.Id,
                                             Number = order.Number,
                                             BeginDistributionDate = order.BeginDistributionDate,
                                             EndDistributionDate = order.EndDistributionDatePlan,
                                             SourceOrganizationUnit = sou.Name,
                                             DestinationOrganizationUnit = dou.Name
                                         }).ToArray();
            return relatedOrders;
        }

        public IEnumerable<RelatedOrderDescriptor> GetRelatedOrdersForPrintJointBill(long orderId)
        {
            RelatedOrderDescriptor[] relatedOrders = null;
            var modelOrderEntries = (from order in _finder.FindAll<Order>()
                                     join bill in _finder.Find<Bill>(b => b.IsActive && !b.IsDeleted) on order.Id equals bill.OrderId into orderBills
                                     where order.Id == orderId && order.IsActive && !order.IsDeleted
                                     select order).ToArray();

            if (modelOrderEntries.Length > 0)
            {
                var modelOrder = modelOrderEntries[0];
                var modelOrderBillsCount = modelOrderEntries.Length;

                relatedOrders = (from order in _finder.FindAll<Order>()
                                 join sou in _finder.FindAll<OrganizationUnit>() on order.SourceOrganizationUnitId equals sou.Id
                                 join dou in _finder.FindAll<OrganizationUnit>() on order.DestOrganizationUnitId equals dou.Id
                                 join bill in _finder.Find<Bill>(b => b.IsActive && !b.IsDeleted) on order.Id equals bill.OrderId
                                 join modelBill in _finder.Find<Bill>(b => b.IsActive && !b.IsDeleted && b.OrderId == orderId) on
                                     new { bill.BeginDistributionDate, bill.EndDistributionDate } equals
                                     new { modelBill.BeginDistributionDate, modelBill.EndDistributionDate } into orderBills
                                 where
                                     order.IsActive && !order.IsDeleted && order.PayablePlan > 0m && order.LegalPersonId == modelOrder.LegalPersonId
                                     && order.BranchOfficeOrganizationUnitId == modelOrder.BranchOfficeOrganizationUnitId
                                     && order.BeginDistributionDate == modelOrder.BeginDistributionDate
                                     && order.EndDistributionDatePlan == modelOrder.EndDistributionDatePlan && modelOrderBillsCount == orderBills.Count()
                                     && order.Id != modelOrder.Id
                                 orderby dou.Name ascending
                                 select
                                     new RelatedOrderDescriptor
                                         {
                                             Id = order.Id,
                                             Number = order.Number,
                                             BeginDistributionDate = order.BeginDistributionDate,
                                             EndDistributionDate = order.EndDistributionDatePlan,
                                             SourceOrganizationUnit = sou.Name,
                                             DestinationOrganizationUnit = dou.Name
                                         }).ToArray();

                relatedOrders = relatedOrders.Distinct(new StrictKeyEqualityComparer<RelatedOrderDescriptor, long>(d => d.Id)).ToArray();
            }

            return relatedOrders;
        }

        public OrderInfoToCheckOrderBeginDistributionDate GetOrderInfoToCheckOrderBeginDistributionDate(long orderId)
        {
            return
                _finder.Find<Order>(x => x.Id == orderId)
                       .Select(
                               x =>
                               new OrderInfoToCheckOrderBeginDistributionDate
                                   {
                                       OrderId = x.Id,
                                       BeginDistributionDate = x.BeginDistributionDate,
                                       SourceOrganizationUnitId = x.SourceOrganizationUnitId,
                                       DestinationOrganizationUnitId = x.DestOrganizationUnitId
                                   })
                       .SingleOrDefault();
        }

        public IEnumerable<OrderPayablePlanInfo> GetPayablePlans(long[] orderIds)
        {
            return
                _finder.Find<Order>(o => o.IsActive && !o.IsDeleted && orderIds.Contains(o.Id))
                       .Select(o => new OrderPayablePlanInfo { OrderId = o.Id, PayablePlan = o.PayablePlan })
                       .ToArray();
        }

        public OrderInfoToGetInitPayments GetOrderInfoForInitPayments(long orderId)
        {
            // Здесь нужен finder небезопасный
            return
                _finder.Find<Order>(x => x.IsActive && !x.IsDeleted && x.Id == orderId)
                       .Select(
                               x =>
                               new OrderInfoToGetInitPayments
                                   {
                                       ReleaseCountPlan = x.ReleaseCountPlan,
                                       BeginDistributionDate = x.BeginDistributionDate,
                                       EndDistributionDate = x.EndDistributionDatePlan,
                                       SignupDate = x.SignupDate,
                                       PayablePlan = x.PayablePlan,
                                       IsOnRegistration = x.WorkflowStepId == OrderState.OnRegistration,
                                       BillsCount = x.Bills.Count(y => y.IsActive && !y.IsDeleted),
                                   })
                       .Single();
        }

        public IEnumerable<RecipientDto> GetRecipientsForAutoMailer(DateTime startDate, DateTime endDate, bool includeRegional)
        {
            var startDateForNotActualClientPeriod = startDate.Date.AddDays(-1);
            var endDateForNotActualClientPeriod = startDate.Date.AddMilliseconds(-1);

            var apiPlatformId = _finder.Find<Platform.Model.Entities.Erm.Platform>(x => x.DgppId == (int)PlatformEnum.Api).Select(x => x.Id).Single();

            // Получаем текущих рекламодателей
            var actualRecepientsInfo = _finder.Find<Order>(x => x.IsActive && !x.IsDeleted &&
                                                                x.OrderType != OrderType.SelfAds && x.OrderType != OrderType.SocialAds &&
                                                                (includeRegional || x.SourceOrganizationUnitId == x.DestOrganizationUnitId) &&
                                                                (x.WorkflowStepId == OrderState.Approved ||
                                                                 x.WorkflowStepId == OrderState.Archive ||
                                                                 x.WorkflowStepId == OrderState.OnTermination) &&
                                                                x.BeginDistributionDate <= endDate && x.EndDistributionDateFact >= startDate)
                                              .Select(x => new
                                                  {
                                                      BranchName = x.Firm.OrganizationUnit.Name,
                                                      FirmCode = x.Firm.Id,
                                                      FirmName = x.Firm.Name,
                                                      Contact = x.Firm.Client.Contacts.Where(y => y.IsActive && !y.IsDeleted && !y.IsFired &&
                                                                                                  y.AccountRole == AccountRole.MakingDecisions)
                                                                 .OrderByDescending(y => y.WorkEmail)
                                                                 .FirstOrDefault(),
                                                      IsClientActually = true,
                                                      ContainsApiPlatform = x.OrderPositions
                                                                             .Any(y => y.IsActive && !y.IsDeleted &&
                                                                                       y.OrderPositionAdvertisements
                                                                                        .Any(z => z.Position.PlatformId == apiPlatformId)),
                                                      BranchOfficeOrganizationUnit =
                                                               x.Firm.OrganizationUnit.BranchOfficeOrganizationUnits
                                                                .Where(y => y.IsPrimary)
                                                                .Select(y => new { y.ChiefNameInGenitive, y.Email })
                                                                .FirstOrDefault(),
                                                      x.Firm.OwnerCode,
                                                  })
                                              .ToArray()
                                              .GroupBy(x => x.FirmCode)
                                              .Select(lst => new
                                                  {
                                                      FirmCode = lst.Key,
                                                      Value = lst.FirstOrDefault(),
                                                      AreThereAnyApiPlatform = lst.Any(y => y.ContainsApiPlatform)
                                                  })
                                              .ToArray();

            // Получаем бывших рекламодателей
            var notActualRecepientsInfo = _finder.Find<Order>(x => x.IsActive && !x.IsDeleted &&
                                                                   x.OrderType != OrderType.SelfAds && x.OrderType != OrderType.SocialAds &&
                                                                   (includeRegional || x.SourceOrganizationUnitId == x.DestOrganizationUnitId) &&
                                                                   (x.WorkflowStepId == OrderState.Archive ||
                                                                    x.WorkflowStepId == OrderState.OnTermination) &&
                                                                   x.EndDistributionDateFact >= startDateForNotActualClientPeriod &&
                                                                   x.EndDistributionDateFact <= endDateForNotActualClientPeriod)
                                                 .Select(x => new
                                                     {
                                                         BranchName = x.Firm.OrganizationUnit.Name,
                                                         FirmCode = x.Firm.Id,
                                                         FirmName = x.Firm.Name,
                                                         Contact = x.Firm.Client.Contacts.Where(y => y.IsActive && !y.IsDeleted && !y.IsFired &&
                                                                                                     y.AccountRole == AccountRole.MakingDecisions)
                                                                    .OrderByDescending(y => y.WorkEmail)
                                                                    .FirstOrDefault(),
                                                         IsClientActually = false,
                                                         ContainsApiPlatform = x.OrderPositions
                                                                                .Any(y => y.IsActive && !y.IsDeleted &&
                                                                                          y.OrderPositionAdvertisements
                                                                                           .Any(z => z.Position.PlatformId == apiPlatformId)),
                                                         BranchOfficeOrganizationUnit =
                                                                  x.Firm.OrganizationUnit.BranchOfficeOrganizationUnits
                                                                   .Where(y => y.IsPrimary)
                                                                   .Select(y => new { y.ChiefNameInGenitive, y.Email })
                                                                   .FirstOrDefault(),
                                                         x.Firm.OwnerCode,
                                                     })
                                                 .ToArray()
                                                 .GroupBy(x => x.FirmCode)
                                                 .Select(lst => new
                                                     {
                                                         FirmCode = lst.Key,
                                                         Value = lst.FirstOrDefault(),
                                                         AreThereAnyApiPlatform = lst.Any(y => y.ContainsApiPlatform)
                                                     })
                                                 .ToArray();

            var recepientsInfo = actualRecepientsInfo.Union(notActualRecepientsInfo.Where(x => actualRecepientsInfo.All(y => y.FirmCode != x.FirmCode)))
                                                     .ToArray();

            var ownerCodes = recepientsInfo.Select(x => x.Value.OwnerCode).Distinct().ToArray();

            var userProfiles = _finder.Find<User>(x => x.IsActive && !x.IsDeleted && ownerCodes.Contains(x.Id)).SelectMany(x => x.UserProfiles).ToArray();

            var result = recepientsInfo
                .Select(x => new RecipientDto
                    {
                        BranchName = x.Value.BranchName,
                        ContactClientEmail = x.Value.Contact == null ? null : x.Value.Contact.WorkEmail,
                        ContactClientName = x.Value.Contact == null ? null : x.Value.Contact.FullName,
                        ContactClientSex = x.Value.Contact == null ? null : x.Value.Contact.GenderCode.ToString(),
                        FirmCode = x.FirmCode,
                        FirmName = x.Value.FirmName,
                        IsClientActually = x.Value.IsClientActually,
                        IsOrdersOfWebAPI = x.AreThereAnyApiPlatform,
                        LegalEntityBranchDirectorName = x.Value.BranchOfficeOrganizationUnit == null
                                                            ? null
                                                            : x.Value.BranchOfficeOrganizationUnit.ChiefNameInGenitive,
                        LegalEntityBranchEmail = x.Value.BranchOfficeOrganizationUnit == null ? null : x.Value.BranchOfficeOrganizationUnit.Email,
                        CuratorEmail = userProfiles.FirstOrDefault(y => y.UserId == x.Value.OwnerCode) == null
                                           ? null
                                           : userProfiles.First(y => y.UserId == x.Value.OwnerCode).Email
                    })
                .OrderBy(x => x.BranchName).ThenByDescending(x => x.IsClientActually).ThenBy(x => x.FirmCode)
                .ToArray();

            return result;
        }

        public ReleaseNumbersDto CalculateReleaseNumbers(long organizationUnitId, DateTime rawBeginDistributuionDate, int releaseCountPlan, int releaseCountFact)
        {
            var firstEmitDate = _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId))
                                       .Select(x => (DateTime?)x.FirstEmitDate)
                                       .FirstOrDefault();

            if (firstEmitDate == null)
            {
                throw new NotificationException(BLResources.DestinationOrganizationUnitHasNotFirstReleaseDate);
            }

            var beginReleaseNumber = rawBeginDistributuionDate.MonthDifference(firstEmitDate.Value) + 1;
            var endReleaseNumberPlan = beginReleaseNumber + (releaseCountPlan - 1);
            var endReleaseNumberFact = beginReleaseNumber + (releaseCountFact - 1);

            return new ReleaseNumbersDto
                {
                    BeginReleaseNumber = beginReleaseNumber,
                    EndReleaseNumberPlan = endReleaseNumberPlan,
                    EndReleaseNumberFact = endReleaseNumberFact,
                };
        }

        public void UpdateOrderReleaseNumbers(Order order)
        {
            var releaseNumbersDto = CalculateReleaseNumbers(order.DestOrganizationUnitId,
                                                            order.BeginDistributionDate,
                                                            order.ReleaseCountPlan,
                                                            order.ReleaseCountFact);

            order.BeginReleaseNumber = releaseNumbersDto.BeginReleaseNumber;
            order.EndReleaseNumberPlan = releaseNumbersDto.EndReleaseNumberPlan;
            order.EndReleaseNumberFact = releaseNumbersDto.EndReleaseNumberFact;
        }

        public DistributionDatesDto CalculateDistributionDates(DateTime rawBeginDistributuionDate, int releaseCountPlan, int releaseCountFact)
        {
            var beginDistributionDate = rawBeginDistributuionDate.GetFirstDateOfMonth();
            var endDistributionDatePlan = beginDistributionDate.AddMonths(releaseCountPlan - 1).GetLastDateOfMonth();
            var endDistributionDateFact = beginDistributionDate.AddMonths(releaseCountFact - 1).GetLastDateOfMonth();

            return new DistributionDatesDto
                {
                    BeginDistributionDate = beginDistributionDate,
                    EndDistributionDatePlan = endDistributionDatePlan,
                    EndDistributionDateFact = endDistributionDateFact,
                };
        }

        public void UpdateOrderDistributionDates(Order order)
        {
            var distributionDatesDto = CalculateDistributionDates(order.BeginDistributionDate, order.ReleaseCountPlan, order.ReleaseCountFact);

            order.BeginDistributionDate = distributionDatesDto.BeginDistributionDate;
            order.EndDistributionDatePlan = distributionDatesDto.EndDistributionDatePlan;
            order.EndDistributionDateFact = distributionDatesDto.EndDistributionDateFact;
        }

        // COMMENT {all, 18.12.2014}: это PayablePriceWithVat
        public decimal GetPayablePlanSum(long orderId, int releaseCount)
        {
            return _finder.Find<OrderPosition>(x => x.OrderId == orderId)
                          .Where(Specs.Find.ActiveAndNotDeleted<OrderPosition>())
                          .Sum(x => (decimal?)(x.PricePerUnitWithVat * x.Amount * releaseCount)) ?? 0m;
        }

        public OrderFinancialInfo GetFinancialInformation(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .Select(x => new OrderFinancialInfo
                              {
                                  DiscountSum = x.DiscountSum,
                                  ReleaseCountFact = x.ReleaseCountFact,
                                  DiscountInPercent = x.OrderPositions
                                                       .Where(y => !y.IsDeleted && y.IsActive)
                                                                    .All(y => y.CalculateDiscountViaPercent)
                              })
                          .Single();
        }

        public OrderCompletionState GetOrderCompletionState(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .Select(order => new OrderCompletionState
                              {
                                  LegalPerson = order.LegalPersonId != null,
                                  BranchOfficeOrganizationUnit = order.BranchOfficeOrganizationUnitId != null
                              })
                          .Single();
        }

        public OrderDeactivationPosibility IsOrderDeactivationPossible(long orderId)
        {
            var orderInfo = _finder.Find(Specs.Find.ById<Order>(orderId))
                                   .Select(x => new
                                       {
                                           x.Id,
                                           x.Number,
                                           x.OwnerCode,
                                           x.IsActive,
                                           x.BargainId,
                                           x.WorkflowStepId,
                                           x.IsTerminated,
                                           x.TerminationReason,
                                           x.Comment
                                       }).SingleOrDefault();

            if (null == orderInfo)
            {
                return new OrderDeactivationPosibility { IsDeactivationAllowed = false, DeactivationDisallowedReason = BLResources.EntityNotFound };
            }

            var orderDto = new Order { Id = orderInfo.Id, OwnerCode = orderInfo.OwnerCode };
            var orderNumber = string.Format("{0} - {1}", MetadataResources.Order, orderInfo.Number);

            // на текущий момент интересует только возможность деактивировать
            var isDeleteAllowed = _entityAccessService.HasEntityAccess(EntityAccessTypes.Delete,
                                                                       EntityType.Instance.Order(),
                                                                       _userContext.Identity.Code,
                                                                       orderDto.Id,
                                                                       orderDto.OwnerCode,
                                                                       null);
            if (!isDeleteAllowed)
            {
                return new OrderDeactivationPosibility
                    {
                        EntityCode = orderNumber,
                        IsDeactivationAllowed = false,
                        DeactivationDisallowedReason = BLResources.EntityAccessDenied
                    };
            }

            // заказ уже закрыт
            if (!orderInfo.IsActive)
            {
                return new OrderDeactivationPosibility
                    {
                        EntityCode = orderNumber,
                        IsDeactivationAllowed = false,
                        DeactivationDisallowedReason = BLResources.OrderHasAlreadyBeenClosed
                    };
            }

            if (!(orderInfo.WorkflowStepId == OrderState.OnRegistration || orderInfo.WorkflowStepId == OrderState.Rejected))
            {
                return new OrderDeactivationPosibility
                    {
                        EntityCode = orderNumber,
                        IsDeactivationAllowed = false,
                        DeactivationDisallowedReason = BLResources.OrderCloseIncorrectState
                    };
            }

            // Выбрать причину из списка причин досрочного расторжения
            if (((orderInfo.TerminationReason == OrderTerminationReason.RejectionOther) ||
                 (orderInfo.TerminationReason == OrderTerminationReason.TemporaryRejectionOther)) && string.IsNullOrEmpty(orderInfo.Comment))
            {
                return new OrderDeactivationPosibility
                    {
                        EntityCode = orderNumber,
                        IsDeactivationAllowed = false,
                        DeactivationDisallowedReason = BLResources.OrderDeleteTerminationCommentRequired
                    };
            }

            if (orderInfo.BargainId.HasValue)
            {
                var hasOtherOrdersInBargain = _finder.Find(Specs.Find.ById<Order>(orderId))
                                                     .Select(x => x.Bargain.Orders.Any(y => y.IsActive && !y.IsDeleted && y.Id != orderInfo.Id))
                                                     .Single();

                if (!hasOtherOrdersInBargain)
                {
                    return new OrderDeactivationPosibility
                        {
                            EntityCode = orderNumber,
                            IsDeactivationAllowed = true,
                            DeactivationConfirmation = BLResources.OrderCloseWithBargainDeletionConformation
                        };
                }
            }

            return new OrderDeactivationPosibility
                {
                    EntityCode = orderNumber,
                    IsDeactivationAllowed = true,
                    DeactivationConfirmation = BLResources.OrderCloseConformation
                };
        }

        public OrderStateValidationInfo GetOrderStateValidationInfo(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .Select(x => new OrderStateValidationInfo
                              {
                                  LegalPersonId = x.LegalPersonId,
                                  BranchOfficeOrganizationUnitId = x.BranchOfficeOrganizationUnitId,
                                  SourceOrganizationUnitId = x.SourceOrganizationUnitId,
                                  DestOrganizationUnitId = x.DestOrganizationUnitId,
                                  HasDocumentsDebt = x.HasDocumentsDebt,
                                  AnyPositions = x.OrderPositions.Any(y => y.IsActive && !y.IsDeleted)
                              })
                          .SingleOrDefault();
        }

        public bool IsOrderForOrganizationUnitsPairExist(long orderId, long sourceOrganizationUnitId, long destOrganizationUnitId)
        {
            return
                _finder.Find(Specs.Find.ById<Order>(orderId) &&
                             OrderSpecs.Orders.Find.ForOrganizationUnitsPair(sourceOrganizationUnitId, destOrganizationUnitId))
                       .Any();
        }

        public IEnumerable<Order> GetOrdersForDeal(long dealId)
        {
            return _finder.Find<Order>(x => x.DealId == dealId && x.IsActive && !x.IsDeleted).ToArray();
        }

        public OrderPositionAdvertisementLinksDto GetOrderPositionAdvertisementLinksInfo(long orderPositionId)
        {
            return _finder.Find(Specs.Find.ById<OrderPosition>(orderPositionId))
                          .Select(position => new OrderPositionAdvertisementLinksDto
                              {
                                  AdvertisementLinks = position.OrderPositionAdvertisements,
                                  BindingType = position.PricePosition.Position.BindingObjectTypeEnum,
                                  OrderWorkflowState = position.Order.WorkflowStepId,
                                  OrderId = position.Order.Id
                              })
                          .Single();
        }

        public OrderUsageDto GetOrderUsage(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .Select(order => new OrderUsageDto { Order = order, AnyLocks = order.Locks.Any(@lock => !@lock.IsDeleted) })
                          .Single();
        }

        public OrderDiscountsDto GetOrderDiscounts(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .Select(x =>
                                  new OrderDiscountsDto
                                      {
                                          CalculateDiscountViaPercent = x.OrderPositions
                                                                         .Where(y => !y.IsDeleted && y.IsActive)
                                                                         .All(y => y.CalculateDiscountViaPercent),
                                          DiscountPercent = x.DiscountPercent.HasValue ? x.DiscountPercent.Value : 0M,
                                          DiscountSum = x.DiscountSum.HasValue ? x.DiscountSum.Value : 0M
                                      }).Single();
        }

        public Order GetOrderUnsecure(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId)).SingleOrDefault();
        }

        public IEnumerable<SubPositionDto> GetSelectedSubPositions(long orderPositionId)
        {
            return _finder.Find<OrderPosition>(x => x.Id == orderPositionId)
                          .SelectMany(x => x.OrderPositionAdvertisements)
                          .Select(x => new SubPositionDto
                              {
                                  PositionId = x.PositionId,
                                  PlatformId = x.Position.PlatformId
                              })
                          .Distinct()
                          .ToArray();
        }

        public VatRateDetailsDto GetVatRateDetails(long? sourceOrganizationUnitId, long destOrganizationUnitId)
        {
            // TODO {all, 19.03.2015}: вынести в конфиг
            const decimal DefaultVatRate = 18M;

            var sourceVat = DefaultVatRate;
            if (sourceOrganizationUnitId.HasValue)
            {
                sourceVat = _finder.Find(OrganizationUnitSpecs.Select.VatRate(),
                                                                    Specs.Find.ById<OrganizationUnit>(sourceOrganizationUnitId.Value))
                                   .Single();
            }

            var destVat = _finder.Find(OrganizationUnitSpecs.Select.VatRate(),
                                                                  Specs.Find.ById<OrganizationUnit>(destOrganizationUnitId))
                                 .Single();

            return DetermineVatRate(sourceVat, destVat);
        }

        public VatRateDetailsDto GetVatRateDetails(long orderId)
        {
            var orderVatInfo = _finder.Find(Specs.Find.ById<Order>(orderId))
                                    .Select(item => new
                                                        {

                                                            OrderBranchOfficeOrganizationUnitVatRate = item.BranchOfficeOrganizationUnit.BranchOffice.BargainType.VatRate,
                                                            SourceVatRate = item.BranchOfficeOrganizationUnit.BranchOffice.BargainType.VatRate,
                                                            DestVatRate = item.DestOrganizationUnit.BranchOfficeOrganizationUnits
                                                                              .FirstOrDefault(unit => unit.IsPrimaryForRegionalSales)
                                                                              .BranchOffice
                                                                              .BargainType
                                                                              .VatRate,
                                                            item.SourceOrganizationUnitId,
                                                            item.DestOrganizationUnitId,
                                                        })
                                    .Single();

            // Для местного заказа Ндс источника и назначения определяем по юр. лицу исполнителя, указанному в заказе
            var sourceVatRate = orderVatInfo.SourceOrganizationUnitId == orderVatInfo.DestOrganizationUnitId
                                    ? orderVatInfo.OrderBranchOfficeOrganizationUnitVatRate
                                    : orderVatInfo.SourceVatRate;

            var destVatRate = orderVatInfo.SourceOrganizationUnitId == orderVatInfo.DestOrganizationUnitId
                                    ? orderVatInfo.OrderBranchOfficeOrganizationUnitVatRate
                                    : orderVatInfo.DestVatRate;


            return DetermineVatRate(sourceVatRate, destVatRate);
        }

        private VatRateDetailsDto DetermineVatRate(decimal sourceOrgUnitVatRate, decimal destOrgUnitVatRate)
        {
            if (sourceOrgUnitVatRate == decimal.Zero)
            {
                // Город источник - франчайзи
                return new VatRateDetailsDto
                           {
                               VatRate = destOrgUnitVatRate,
                               ShowVat = false
                           };
            }

            // Город источник - филиал
            return new VatRateDetailsDto
                       {
                           VatRate = sourceOrgUnitVatRate,
                           ShowVat = true
                       };
        }

        public long GetOrderOwnerCode(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId)).Select(x => x.OwnerCode).Single();
        }

        public IReadOnlyCollection<Bargain> GetNonClosedClientBargains()
        {
            return _finder.Find(OrderSpecs.Bargains.Find.NonClosed && OrderSpecs.Bargains.Find.ClientBargains()).ToArray();
        }

        public Bargain GetBargain(long bargainId)
        {
            return _finder.FindOne(Specs.Find.ById<Bargain>(bargainId));
        }

        public string GetDuplicateAgentBargainNumber(long bargainId,
                                                     long legalPersonId,
                                                     long branchOfficeOrganizationUnitId,
                                                     DateTime bargainBeginDate,
                                                     DateTime bargainEndDate)
        {
            return
                _finder.Find(OrderSpecs.Bargains.Find.Duplicate(bargainId, legalPersonId, branchOfficeOrganizationUnitId, bargainBeginDate, bargainEndDate) &&
                             Specs.Find.ActiveAndNotDeleted<Bargain>() && OrderSpecs.Bargains.Find.AgentBargains())
                       .Select(x => x.Number)
                       .FirstOrDefault();
        }

        public IDictionary<string, DateTime> GetBargainUsage(long bargainId)
        {
            return _finder.Find(Specs.Find.ById<Bargain>(bargainId))
                          .SelectMany(x => x.Orders)
                          .Where(Specs.Find.ActiveAndNotDeleted<Order>())
                          .ToDictionary(x => x.Number, x => x.EndDistributionDateFact);
        }

        public BargainEndAndCloseDatesDto GetBargainEndAndCloseDates(long bargainId)
        {
            return _finder.Find(Specs.Find.ById<Bargain>(bargainId))
                          .Select(x =>
                                  new BargainEndAndCloseDatesDto
                                      {
                                          BargainEndDate = x.BargainEndDate,
                                          BargainCloseDate = x.ClosedOn
                                      })
                          .SingleOrDefault();
        }

        public IEnumerable<OrderSuitableBargainDto> GetSuitableBargains(long legalPersonId,
                                                                        long branchOfficeOrganizationUnitId,
                                                                        DateTime orderEndDistributionDate)
        {
            return
                _finder.Find(OrderSpecs.Bargains.Find.ByLegalPersons(legalPersonId, branchOfficeOrganizationUnitId) && Specs.Find.ActiveAndNotDeleted<Bargain>()
                             && OrderSpecs.Bargains.Find.NotClosedByCertainDate(orderEndDistributionDate))
                       .Select(x => new OrderSuitableBargainDto
        {
                               Id = x.Id,
                               EndDate = x.BargainEndDate,
                               Number = x.Number,
                               BargainKind = x.BargainKind
                           })
                       .ToArray();
        }

        public OrderOrganizationUnitDerivedFieldsDto GetFieldValuesByOrganizationUnit(long organizationUnitId)
        {
            var dto = _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId))
                             .Select(x => new
                                              {
                                                  OrganizationUnit = new { x.Id, x.Name },
                                                  Currency = new { x.Country.Currency.Id, x.Country.Currency.Name },
                                                  ProjectExists = x.Projects.Any(),
                                              })
                             .Single();

            return new OrderOrganizationUnitDerivedFieldsDto
                       {
                           Currency = new EntityReference(dto.Currency.Id, dto.Currency.Name),
                           OrganizationUnit = dto.ProjectExists ? new EntityReference(dto.OrganizationUnit.Id, dto.OrganizationUnit.Name) : null,
                       };
        }

        public OrderParentEntityDerivedFieldsDto GetOrderFieldValuesByParentEntity(IEntityType parentEntityName, long parentEntityId)
        {
            if (parentEntityName.Equals(EntityType.Instance.Client()))
            {
                return GetReferenceByClient(parentEntityId);
            }

            if (parentEntityName.Equals(EntityType.Instance.Firm()))
            {
                return GetReferencesByFirm(parentEntityId);
            }

            if (parentEntityName.Equals(EntityType.Instance.LegalPerson()))
            {
                return GetReferencesByLegalPerson(parentEntityId);
            }

            if (parentEntityName.Equals(EntityType.Instance.Deal()))
            {
                return GetReferencesByDeal(parentEntityId);
            }
            
            return new OrderParentEntityDerivedFieldsDto();
        }

        public IEnumerable<Bill> GetBillsForOrder(long orderId)
        {
            return _finder.FindMany(OrderSpecs.Bills.Find.ByOrder(orderId) & Specs.Find.ActiveAndNotDeleted<Bill>());
        }

        public SalesModel GetOrderSalesModel(long orderId)
        {
            return
                _finder.Find(Specs.Find.ById<Order>(orderId))
                       .SelectMany(x => x.OrderPositions)
                       .Where(Specs.Find.ActiveAndNotDeleted<OrderPosition>())
                       .Select(x => x.PricePosition.Position.SalesModel)
                       .Distinct()
                       .SingleOrDefault();
        }

        public OrderDocumentsDebtDto GetOrderDocumentsDebtInfo(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .Select(x => new OrderDocumentsDebtDto
                                           {
                                               Order = new EntityReference
                                                           {
                                                               Id = x.Id,
                                                               Name = x.Number
                                                           },
                                               DocumentsComment = x.DocumentsComment,
                                               HasDocumentsDebt = x.HasDocumentsDebt
                                           })
                          .Single();
        }

        public long? GetBargainIdByOrder(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId)).Select(x => x.BargainId).Single();
        }

        public long GetLegalPersonIdByBargain(long bargainId)
        {
            return _finder.Find(Specs.Find.ById<Bargain>(bargainId)).Select(x => x.CustomerLegalPersonId).Single();
        }

        public OrderDtoToCheckPossibilityOfOrderPositionCreation GetOrderInfoToCheckPossibilityOfOrderPositionCreation(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .Select(x => new OrderDtoToCheckPossibilityOfOrderPositionCreation
                                           {
                                               OrderId = x.Id,
                                               FirmId = x.FirmId,
                                               OrderPositions =
                                                   x.OrderPositions.Where(y => y.IsActive && !y.IsDeleted)
                                                    .Select(y => new OrderPositionSalesModelDto
                                                                     {
                                                                         OrderPositionId = y.Id,
                                                                         SalesModel =
                                                                             y.PricePosition
                                                                              .Position
                                                                              .SalesModel
                                                                     })
                                           }).Single();
        }

        public long? GetLegalPersonProfileIdByOrder(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .Select(order => order.LegalPersonProfileId)
                          .SingleOrDefault();
        }

        public IEnumerable<Order> GetActiveOrdersForLegalPersonProfile(long legalPersonProfileId)
        {
            return _finder.FindMany(OrderSpecs.Orders.Find.NotInArchive()
                                    && Specs.Find.ActiveAndNotDeleted<Order>()
                                    && OrderSpecs.Orders.Find.ByLegalPersonProfileId(legalPersonProfileId));
        }

        public OrderAmountToWithdrawInfo GetOrderAmountToWithdrawInfo(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                           .Select(o => new OrderAmountToWithdrawInfo
                                                {
                                                    Order = o,
                                                    AmountToWithdraw = o.OrderReleaseTotals
                                                                          .Where(
                                                                              orderReleaseTotal =>
                                                                              orderReleaseTotal.ReleaseNumber ==
                                                                              o.BeginReleaseNumber +
                                                                              o.Locks.Count(@lock => !@lock.IsDeleted && !@lock.IsActive))
                                                                          .Select(orderReleaseTotal => orderReleaseTotal.AmountToWithdraw)
                                                                          .FirstOrDefault()
                                                })
                            .Single();
        }

        public OrderRecalculateWithdrawalsDto GetOrderRecalculateWithdrawalsInfo(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                        .Select(x => new OrderRecalculateWithdrawalsDto
                        {
                            LocksCount = x.Locks.Count(@lock => !@lock.IsDeleted && !@lock.IsActive),
                            ReleaseTotals = x.OrderReleaseTotals,
                            OrderPositions = x.OrderPositions.Where(y => y.IsActive && !y.IsDeleted)
                                                .Select(y => new OrderRecalculateWithdrawalsDto.OrderPositionDto
                                                {
                                                    Id = y.Id,
                                                    PayablePlan = y.PayablePlan,
                                                    PayablePlanWoVat = y.PayablePlanWoVat,
                                                    PlatformId = y.PricePosition.Position.PlatformId,
                                                    PriceId = y.PricePosition.PriceId,
                                                    OrderId = y.OrderId,
                                                    PositionId = y.PricePosition.PositionId,
                                                    Amount = y.Amount,
                                                    DiscountSum = y.DiscountSum,
                                                    DiscountPercent = y.DiscountPercent,
                                                    CalculateDiscountViaPercent = y.CalculateDiscountViaPercent,
                                                    CategoryRate = y.CategoryRate,
                                                    IsComposite = y.PricePosition.Position.IsComposite,
                                                    ReleaseWithdrawals = y.ReleasesWithdrawals.Select(z => new OrderReleaseWithdrawalDto
                                                                                                            {
                                                                                                                WidrawalInfo = z, 
                                                                                                                WithdrawalsPositions = z.ReleasesWithdrawalsPositions
                                                                                                            })
                                                })
                        })
                        .Single();
        }

        public OrderDeleteOrderPositionDto GetOrderPositionDeleteInfo(long orderPositionId)
        {
            return _finder.Find(Specs.Find.ById<OrderPosition>(orderPositionId))
                          .Select(position => new OrderDeleteOrderPositionDto
                                                  {
                                                      OrderPosition = position,
                                                      Order = position.Order,
                                                      IsDiscountViaPercentCalculation = position.Order.OrderPositions
                                                                                         .Where(y => !y.IsDeleted && y.IsActive)
                                                                                         .All(y => y.CalculateDiscountViaPercent),
                                                  })
                          .SingleOrDefault();
        }

        public OrderRepairOutdatedOrderPositionDto GetOrderInfoForRepairOutdatedPositions(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .Select(o => new OrderRepairOutdatedOrderPositionDto
                                           {
                                               ReleaseTotals = o.OrderReleaseTotals,
                                               OrderPositions = 
                                                    o.OrderPositions
                                                        .Where(op => op.IsActive && !op.IsDeleted)
                                                        .Select(op => new OrderRepairOutdatedOrderPositionDto.OrderPositionDto
                                                                            {
                                                                                OrderPosition = op,
                                                                                PricePosition = op.PricePosition,
                                                                                Advertisements = op.OrderPositionAdvertisements,
                                                                                ClonedAdvertisements = 
                                                                                    op.OrderPositionAdvertisements
                                                                                            .Select(adv => new Platform.Model.Entities.DTOs.AdvertisementDescriptor
                                                                                            {
                                                                                                AdvertisementId = adv.AdvertisementId,
                                                                                                CategoryId = adv.CategoryId,
                                                                                                ThemeId = adv.ThemeId,
                                                                                                FirmAddressId = adv.FirmAddressId,
                                                                                                PositionId = adv.PositionId,
                                                                                                IsAdvertisementRequired =
                                                                                                    adv.Position.AdvertisementTemplate != null &&
                                                                                                    adv.Position.AdvertisementTemplate.IsAdvertisementRequired
                                                                                            }),
                                                                                ReleaseWithdrawals = 
                                                                                    op.ReleasesWithdrawals
                                                                                            .Select(z => new OrderReleaseWithdrawalDto
                                                                                            {
                                                                                                WidrawalInfo = z, 
                                                                                                WithdrawalsPositions = z.ReleasesWithdrawalsPositions
                                                                                            })
                                                                            })
                                           })
                          .Single();
        }

        public decimal? TakeAmountToWithdrawForOrder(long orderId, int skip, int take)
        {
            return _finder.Find<OrderReleaseTotal>(x => x.OrderId == orderId)
               .OrderBy(x => x.Id)
               .Skip(skip)
               .Take(take)
               .Select(x => (decimal?)x.AmountToWithdraw)
               .SingleOrDefault();
        }

        public OrderLegalPersonProfileDto GetLegalPersonProfileByOrder(long orderId)
        {
            var dto = _secureFinder.Find(Specs.Find.ById<Order>(orderId))
                                   .Select(order => new 
                                       {
                                           LegalPersonId = order.LegalPersonId,
                                           LegalPersonName = order.LegalPerson.LegalName,
                                           LegalPersonProfileId = order.LegalPersonProfileId,
                                           LegalPersonProfileName = order.LegalPersonProfile.Name,
                                       })
                                   .Single();

            if (dto.LegalPersonId == null)
            {
                throw new EntityNotLinkedException(BLResources.LegalPersonFieldsMustBeFilled);
            }

            return new OrderLegalPersonProfileDto
                       {
                           LegalPerson = new EntityReference(dto.LegalPersonId, dto.LegalPersonName),
                           LegalPersonProfile = new EntityReference(dto.LegalPersonProfileId, dto.LegalPersonProfileName)
                       };
        }

        public OrderLegalPersonProfileDto GetLegalPersonProfileByBargain(long bargainId)
        {
            var dto = _secureFinder.Find(Specs.Find.ById<Bargain>(bargainId))
                                   .Select(x => new
                                                    {
                                                        LegalPersonId = x.CustomerLegalPersonId,
                                                        LegalPersonName = x.LegalPerson.LegalName
                                                    })
                                   .Single();

            var profiles = _secureFinder.Find(LegalPersonSpecs.Profiles.Find.ByLegalPersonId(dto.LegalPersonId)
                                              && Specs.Find.ActiveAndNotDeleted<LegalPersonProfile>())
                                        .Select(x => new { x.Id, x.Name })
                                        .Take(2)
                                        .ToArray();

            return new OrderLegalPersonProfileDto
                       {
                           LegalPerson = new EntityReference(dto.LegalPersonId, dto.LegalPersonName),
                           LegalPersonProfile = profiles.Length == 1 ? new EntityReference(profiles[0].Id, profiles[0].Name) : new EntityReference(),
                       };
        }

        private OrderParentEntityDerivedFieldsDto GetReferencesByDeal(long dealId)
        {
            var dto = _finder.Find(Specs.Find.ById<Deal>(dealId) & Specs.Find.NotDeleted<Deal>())
                             .Select(x => new
                                              {
                                                  Deal = new { x.Id, x.Name },
                                                  Currency = new { x.Currency.Id, x.Currency.Name },
                                                  Client = new { x.Client.Id, x.Client.Name },
                                                  x.OwnerCode,

                                                  AnyLinkedFirm = x.FirmDeals.Any(firmDeal => !firmDeal.IsDeleted),
                                                  x.MainFirmId,
                                                  MainFirmName = x.Firm.Name,
                                              })
                             .SingleOrDefault();

            if (dto == null)
            {
                throw new EntityNotFoundException(typeof(Deal), dealId);
            }

            return new OrderParentEntityDerivedFieldsDto
                       {
                           DealCurrency = new EntityReference(dto.Currency.Id, dto.Currency.Name),
                           Deal = new EntityReference(dto.Deal.Id, dto.Deal.Name),
                           Client = new EntityReference(dto.Client.Id, dto.Client.Name),
                           Owner = new EntityReference(dto.OwnerCode),
                           Firm = dto.MainFirmId.HasValue && !dto.AnyLinkedFirm ? new EntityReference(dto.MainFirmId, dto.MainFirmName) : null,
                       };
        }

        private OrderParentEntityDerivedFieldsDto GetReferencesByLegalPerson(long legalPersonId)
        {
            var data = _finder.Find(Specs.Find.ById<LegalPerson>(legalPersonId))
                              .Select(person => new
                              {
                                  Client = new { person.Client.Id, person.Client.Name },
                                  Firms = person.Client.Firms.Select(firm => new
                                  {
                                      firm.Id,
                                      firm.Name,
                                      firm.OrganizationUnitId,
                                      OrganizationUnitName = firm.OrganizationUnit.Name
                                  }),
                                  LegalPerson = new { person.Id, person.LegalName },
                              })
                              .SingleOrDefault();

            var result = new OrderParentEntityDerivedFieldsDto();
            if (data != null)
            {
                result.Firm = data.Firms.Count() == 1 ? new EntityReference(data.Firms.Single().Id, data.Firms.Single().Name) : null;
                result.DestOrganizationUnit = data.Firms.Count() == 1 ? new EntityReference(data.Firms.Single().OrganizationUnitId, data.Firms.Single().OrganizationUnitName) : null;
                result.Client = data.Client != null ? new EntityReference(data.Client.Id, data.Client.Name) : null;
                result.LegalPerson = new EntityReference(data.LegalPerson.Id, data.LegalPerson.LegalName);
            }

            return result;
        }

        private OrderParentEntityDerivedFieldsDto GetReferencesByFirm(long firmId)
        {
            var data = _finder.Find(Specs.Find.ById<Firm>(firmId))
                              .Select(firm => new
                              {
                                  Firm = new { firm.Id, firm.Name, firm.OrganizationUnitId, OrganizationUnitName = firm.OrganizationUnit.Name },
                                  Client = new { firm.Client.Id, firm.Client.Name },
                                  LegalPersons = firm.Client.LegalPersons.Select(person => new { person.Id, person.LegalName })
                              }).SingleOrDefault();

            var result = new OrderParentEntityDerivedFieldsDto();
            if (data != null)
            {
                result.Client = new EntityReference(data.Client.Id, data.Client.Name);
                result.Firm = new EntityReference(data.Firm.Id, data.Firm.Name);
                result.LegalPerson = data.LegalPersons.Count() == 1 ? new EntityReference(data.LegalPersons.Single().Id, data.LegalPersons.Single().LegalName) : null;
                result.DestOrganizationUnit = new EntityReference(data.Firm.OrganizationUnitId, data.Firm.OrganizationUnitName);
            }

            return result;
        }

        private OrderParentEntityDerivedFieldsDto GetReferenceByClient(long clientId)
        {
            var data = _finder.Find(Specs.Find.ById<Client>(clientId))
                              .Select(client => new
                                           {
                                  Client = new { client.Id, client.Name },
                                  Firms = client.Firms.Select(firm => new
                                                                     {
                                      firm.Id,
                                      firm.Name,
                                      firm.OrganizationUnitId,
                                      OrganizationUNitName = firm.OrganizationUnit.Name
                                  }),
                                  LegalPersons = client.LegalPersons.Select(person => new { person.Id, person.LegalName })
                                                                     })
                              .SingleOrDefault();

            var result = new OrderParentEntityDerivedFieldsDto();
            if (data != null)
            {
                result.Client = new EntityReference(data.Client.Id, data.Client.Name);
                result.Firm = data.Firms.Count() == 1 ? new EntityReference(data.Firms.Single().Id, data.Firms.Single().Name) : null;
                result.LegalPerson = data.LegalPersons.Count() == 1 ? new EntityReference(data.LegalPersons.Single().Id, data.LegalPersons.Single().LegalName) : null;
                result.DestOrganizationUnit = data.Firms.Count() == 1 ? new EntityReference(data.Firms.Single().OrganizationUnitId, data.Firms.Single().OrganizationUNitName) : null;
            }

            return result;
        }

        private Dictionary<long, ContributionTypeEnum?> GetBranchOfficesContributionTypes(params long[] organizationUnitIds)
        {
            var list = _finder.Find<OrganizationUnit>(unit => organizationUnitIds.Contains(unit.Id))
                              .Select(x => new
                                  {
                                      OrgUnitId = x.Id,
                                      ContributionType = x.BranchOfficeOrganizationUnits
                                                          .Where(boou => boou.IsPrimary && boou.IsActive && !boou.IsDeleted)
                                                          .Select(boou => boou.BranchOffice.ContributionTypeId)
                                                          .FirstOrDefault()
                                  })
                              .ToArray();

            return list.ToDictionary(x => x.OrgUnitId, x => (ContributionTypeEnum?)x.ContributionType);
        }
    }

    internal class OrderPositionBatchItem
    {
        public long OrderPositionId { get; set; }
        public long FirmId { get; set; }
        public long PositionId { get; set; }
        public IEnumerable<long?> CategoryIds { get; set; }
        public long SourceOrganizationUnitId { get; set; }
    }
}