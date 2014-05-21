using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Dictionary;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Simplified;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO.ForRelease;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositions.Dto;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
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
                                                                op.PricePosition.Position.AccountingMethodEnum == (int)PositionAccountingMethod.PlannedProvision
                                                        })
                              })
                          .ToArray();
        }

        public IEnumerable<Order> GetOrdersForRelease(long organizationUnitId, TimePeriod period)
        {
            return _finder.Find(OrderSpecs.Orders.Find.ForRelease(organizationUnitId, period) && Specs.Find.ActiveAndNotDeleted<Order>());
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

        public IEnumerable<OrderInfo> GetOrderInfosForRelease(long organizationUnitId, TimePeriod period, int skipCount, int takeCount)
        {
            return _finder.Find<Order, OrderInfo>(OrderSpecs.Orders.Select.OrderInfosForRelease(),
                                                  OrderSpecs.Orders.Find.ForRelease(organizationUnitId, period) && Specs.Find.ActiveAndNotDeleted<Order>())
                          .OrderBy(o => o.Id)
                          .Skip(skipCount)
                          .Take(takeCount)
                          .ToArray();
        }

        public IEnumerable<Order> GetOrdersCompletelyReleasedBySourceOrganizationUnit(long sourceOrganizationUnitId)
        {
            return _finder
                .Find(OrderSpecs.Orders.Find.CompletelyReleasedByOrganizationUnit(sourceOrganizationUnitId))
                .ToArray();
        }

        public CreateBargainInfo GetBargainInfoForCreate(long orderId)
        {
            return _finder
                .Find<Order, CreateBargainInfo>(OrderSpecs.Bargains.Select.CreateBargainInfoSelector, Specs.Find.ById<Order>(orderId))
                .Single();
        }

        public bool TryGetExistingBargain(long legalPersonId, long branchOfficeOrganizationUnitId, DateTime orderSignupDate, out Bargain existingBargain)
        {
            existingBargain = _finder
                .Find(OrderSpecs.Bargains.Find.Actual(legalPersonId, branchOfficeOrganizationUnitId, orderSignupDate))
                .FirstOrDefault();

            return existingBargain != null;
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
                                                    ((OrderState)order.WorkflowStepId).ToStringLocalized(EnumResources.ResourceManager,
                                                                                                         CultureInfo.CurrentCulture),
                                                FirmName = order.FirmName,
                                            })
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

        public IEnumerable<long> DetermineOrderPlatforms(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .SelectMany(o => o.OrderPositions)
                          .Where(item => item.IsActive && !item.IsDeleted)
                          .Select(item => item.PricePosition.Position.PlatformId)
                          .Distinct()
                          .ToList();
        }

        public void UpdateOrderPlatform(Order order)
        {
            var platformIds = DetermineOrderPlatforms(order.Id).ToArray();
            var platformId = platformIds.Count() > 1
                                 ? _finder.Find<Platform.Model.Entities.Erm.Platform>(x => x.DgppId == (long)PlatformEnum.Independent).Single().Id
                                 : platformIds.FirstOrDefault();

            order.PlatformId = platformId == 0 ? null : platformId as long?;
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
            return _finder.Find<Note>(n => n.ParentId == orderId && n.ParentType == (int)EntityName.Order && n.ModifiedOn > sinceDate)
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

        // TODO {d.ivanov, 11.11.2013}: Перенести отсюда в read-model
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

        // TODO {d.ivanov, 11.11.2013}: Перенести отсюда в read-model
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

        public Order GetOrder(long orderId)
        {
            return _secureFinder.Find(Specs.Find.ById<Order>(orderId)).Single();
        }

        public bool OrderPriceWasPublished(long organizationUnitId, DateTime orderBeginDistributionDate)
        {
            return _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId))
                          .SelectMany(unit => unit.Prices)
                          .Where(Specs.Find.ActiveAndNotDeleted<Price>())
                          .Any(price => price.IsPublished && price.BeginDate <= orderBeginDistributionDate);
        }

        // FIXME {a.tukaev, 20.03.2014}: Этот метод содержит очень много аспектов получения и обработки данных для последующего использования в UI. 
        //                               Необходимо размотать эти спагетти и те, что есть в ViewOrderPositionHandler, выделив ответственности и реализовать их в отлдельных типах с четкими контрактами
        public OrderPositionDetailedInfo GetOrderPositionDetailedInfo(long? orderPositionId, long orderId, long pricePositionId, bool includeHiddenAddresses)
        {
            var order = GetOrder(orderId);
            var pricePositionInfo =
                _finder.Find(Specs.Find.ById<PricePosition>(pricePositionId))
                       .Select(item => new
                           {
                               item.PositionId,
                               item.Position.Name,
                               Platform = item.Position.Platform.Name,
                               item.Amount,
                               item.AmountSpecificationMode,
                               PricePositionCost = item.Cost,
                               IsBudget = item.Position.AccountingMethodEnum == (int)PositionAccountingMethod.PlannedProvision,
                               item.Position.IsComposite,
                               LinkingObjectType = item.Position.BindingObjectTypeEnum,
                               item.Position.AdvertisementTemplateId,
                               DummyAdvertisementId =
                                           item.Position.AdvertisementTemplate != null
                                               ? item.Position.AdvertisementTemplate.DummyAdvertisementId.Value
                                               : (long?)null,
                               item.Position.CategoryId
                           })
                       .Single();

            var firmAddresses =
                _finder.Find(Specs.Find.ById<Order>(orderId))
                       .Select(item => item.Firm)
                       .SelectMany(firm => firm.FirmAddresses)
                       .Where(address => (address.IsActive && !address.IsDeleted) ||
                                         (includeHiddenAddresses && address.OrderPositionAdvertisements.Any()))
                       .Select(address => new
                           {
                               IsDeleted = address.IsDeleted || (address.ClosedForAscertainment && !address.IsActive),

                               // Удалённые и скрытые навсегда адреса обрабатываем одинаково.
                               IsHidden = address.ClosedForAscertainment && address.IsActive && !address.IsDeleted, // Скрыт временно.
                               address.IsLocatedOnTheMap,
                               address.Id,
                               Address = address.Address + ((address.ReferencePoint == null) ? string.Empty : " — " + address.ReferencePoint),
                               Categories = address.CategoryFirmAddresses
                                                   .Where(link => link.IsActive && !link.IsDeleted)
                                                   .Select(link => link.Category)
                                                   .Where(category => category.IsActive && !category.IsDeleted &&
                                                                      category.CategoryOrganizationUnits
                                                                              .Any(cou => cou.OrganizationUnitId == order.DestOrganizationUnitId))
                                                   .Select(category => category.Id)
                                                   .Union(address.CategoryFirmAddresses
                                                                 .Where(link => link.IsActive && !link.IsDeleted && link.Category.Level == 3)
                                                                 .Select(link => link.Category.ParentCategory.ParentCategory)
                                                                 .Where(category => category.IsActive && !category.IsDeleted &&
                                                                                    category.CategoryOrganizationUnits
                                                                                            .Any(cou => cou.OrganizationUnitId == order.DestOrganizationUnitId))
                                                                 .Select(category => category.Id))
                           })
                       .ToArray();

            var firmCategoriesIds = firmAddresses.SelectMany(item => item.Categories).Distinct().ToArray();

            IEnumerable<long> allCategoriesIds;
            if (orderPositionId != null)
            {
                var advertisementsCategoriesIds = _finder.Find(Specs.Find.ById<OrderPosition>(orderPositionId.Value))
                                                         .SelectMany(item => item.OrderPositionAdvertisements)
                                                         .Where(opa => opa.CategoryId.HasValue)
                                                         .Select(opa => opa.CategoryId.Value)
                                                         .Distinct()
                                                         .ToArray();

                allCategoriesIds = firmCategoriesIds.Union(advertisementsCategoriesIds);
            }
            else
            {
                allCategoriesIds = firmCategoriesIds;
            }

            var categories = _finder.Find(Specs.Find.ActiveAndNotDeleted<Category>())
                                    .Where(category => allCategoriesIds.Contains(category.Id))
                                    .Select(item => new LinkingObjectsSchemaDto.CategoryDto { Id = item.Id, Name = item.Name, Level = item.Level, })
                                    .ToArray();

            IEnumerable<LinkingObjectsSchemaDto.PositionDto> positions;
            if (pricePositionInfo.IsComposite)
            {
                var rawPositions = _finder.Find(Specs.Find.ById<PricePosition>(pricePositionId))
                                          .Select(x => x.Position)
                                          .SelectMany(x => x.ChildPositions)
                                          .Where(x => !x.IsDeleted)
                                          .Select(x => x.ChildPosition)
                                          .Select(x => new
                                              {
                                                  x.Id,
                                                  x.Name,
                                                  x.BindingObjectTypeEnum,
                                                  x.AdvertisementTemplateId,
                                                  x.AdvertisementTemplate.DummyAdvertisementId
                                              })
                                          .ToArray();

                positions = rawPositions
                    .Select(x => new LinkingObjectsSchemaDto.PositionDto
                        {
                            Id = x.Id,
                            Name = x.Name,
                            LinkingObjectType = ((PositionBindingObjectType)x.BindingObjectTypeEnum).ToString(),
                            AdvertisementTemplateId = x.AdvertisementTemplateId,
                            DummyAdvertisementId = x.DummyAdvertisementId,
                            IsLinkingObjectOfSingleType = IsPositionBindingOfSingleType((PositionBindingObjectType)x.BindingObjectTypeEnum)
                        });
            }
            else
            {
                positions = new[]
                    {
                        new LinkingObjectsSchemaDto.PositionDto
                            {
                                Id = pricePositionInfo.PositionId,
                                Name = pricePositionInfo.Name,
                                LinkingObjectType = ((PositionBindingObjectType)pricePositionInfo.LinkingObjectType).ToString(),
                                AdvertisementTemplateId = pricePositionInfo.AdvertisementTemplateId,
                                DummyAdvertisementId = pricePositionInfo.DummyAdvertisementId,
                                IsLinkingObjectOfSingleType = IsPositionBindingOfSingleType((PositionBindingObjectType)pricePositionInfo.LinkingObjectType)
                            }
                    };
            }

            IEnumerable<LinkingObjectsSchemaDto.WarningDto> warnings = null;
            var themeDtos = FindThemesCanBeUsedWithOrder(order);

            if (firmAddresses.Length == 0)
            {
                warnings = new[] { new LinkingObjectsSchemaDto.WarningDto { Text = BLResources.FirmDoesntHaveActiveAddresses } };
            }
            else if (pricePositionInfo.LinkingObjectType == (int)PositionBindingObjectType.ThemeMultiple && !themeDtos.Any())
            {
                warnings = new[] { new LinkingObjectsSchemaDto.WarningDto { Text = BLResources.ThereIsNoSuitableThemes } };
            }

            return new OrderPositionDetailedInfo
                {
                    Amount = pricePositionInfo.Amount,
                    AmountSpecificationMode = pricePositionInfo.AmountSpecificationMode,
                    IsBudget = pricePositionInfo.IsBudget,
                    IsComposite = pricePositionInfo.IsComposite,
                    Platform = pricePositionInfo.Platform ?? string.Empty,
                    PricePositionCost = pricePositionInfo.PricePositionCost,
                    ReleaseCountFact = order.ReleaseCountFact,
                    ReleaseCountPlan = order.ReleaseCountPlan,
                    LinkingObjectsSchema = new LinkingObjectsSchemaDto
                        {
                            Warnings = warnings,
                            FirmCategories = categories.Where(item => firmCategoriesIds.Contains(item.Id)),
                            Themes = themeDtos,
                            AdditionalCategories = categories.Where(item => !firmCategoriesIds.Contains(item.Id)),
                            Positions = positions,
                            FirmAddresses = firmAddresses.Select(fa => new LinkingObjectsSchemaDto.FirmAddressDto
                                {
                                    Id = fa.Id,
                                    Address = string.IsNullOrWhiteSpace(fa.Address)
                                                  ? BLResources.ViewOrderPositionHandler_EmptyAddress
                                                  : fa.Address,
                                    IsDeleted = fa.IsDeleted,
                                    IsHidden = fa.IsHidden,
                                    IsLocatedOnTheMap = fa.IsLocatedOnTheMap,
                                    Categories = fa.Categories,
                                })
                        },
                };
        }

        public OrderForProlongationDto GetOrderForProlongationInfo(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId)).Select(x => new OrderForProlongationDto
                {
                    OrderId = x.Id,
                    OrderType = (OrderType)x.OrderType,
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
            return (OrderState)_finder.Find(Specs.Find.ById<Order>(orderId)).Select(x => x.WorkflowStepId).Single();
        }

        public OrderType GetOrderType(long orderId)
        {
            return (OrderType)_finder.Find(Specs.Find.ById<Order>(orderId)).Select(x => x.OrderType).Single();
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
                                  BindingObjectType = (PositionBindingObjectType)position.PricePosition.Position.BindingObjectTypeEnum
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
                                       IsOnRegistration = x.WorkflowStepId == (int)OrderState.OnRegistration,
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
                                                                x.OrderType != (int)OrderType.SelfAds && x.OrderType != (int)OrderType.SocialAds &&
                                                                (includeRegional || x.SourceOrganizationUnitId == x.DestOrganizationUnitId) &&
                                                                (x.WorkflowStepId == (int)OrderState.Approved ||
                                                                 x.WorkflowStepId == (int)OrderState.Archive ||
                                                                 x.WorkflowStepId == (int)OrderState.OnTermination) &&
                                                                x.BeginDistributionDate <= endDate && x.EndDistributionDateFact >= startDate)
                                              .Select(x => new
                                                  {
                                                      BranchName = x.Firm.OrganizationUnit.Name,
                                                      FirmCode = x.Firm.Id,
                                                      FirmName = x.Firm.Name,
                                                      Contact = x.Firm.Client.Contacts.Where(y => y.IsActive && !y.IsDeleted && !y.IsFired &&
                                                                                                  y.AccountRole == (int)AccountRole.MakingDecisions)
                                                                 .OrderByDescending(y => y.WorkEmail)
                                                                 .FirstOrDefault(),
                                                      IsClientActually = true,
                                                      ContainsApiPlatform = x.OrderPositions
                                                                             .Any(y => y.IsActive && !y.IsDeleted &&
                                                                                       y.OrderPositionAdvertisements
                                                                                        .Any(z => z.Position.PlatformId == apiPlatformId)),
                                                      BranchOfficeOrganizationUnit =
                                                               x.Firm.OrganizationUnit.BranchOfficeOrganizationUnits.FirstOrDefault(y => y.IsPrimary),
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
                                                                   x.OrderType != (int)OrderType.SelfAds && x.OrderType != (int)OrderType.SocialAds &&
                                                                   (includeRegional || x.SourceOrganizationUnitId == x.DestOrganizationUnitId) &&
                                                                   (x.WorkflowStepId == (int)OrderState.Archive ||
                                                                    x.WorkflowStepId == (int)OrderState.OnTermination) &&
                                                                   x.EndDistributionDateFact >= startDateForNotActualClientPeriod &&
                                                                   x.EndDistributionDateFact <= endDateForNotActualClientPeriod)
                                                 .Select(x => new
                                                     {
                                                         BranchName = x.Firm.OrganizationUnit.Name,
                                                         FirmCode = x.Firm.Id,
                                                         FirmName = x.Firm.Name,
                                                         Contact = x.Firm.Client.Contacts.Where(y => y.IsActive && !y.IsDeleted && !y.IsFired &&
                                                                                                     y.AccountRole == (int)AccountRole.MakingDecisions)
                                                                    .OrderByDescending(y => y.WorkEmail)
                                                                    .FirstOrDefault(),
                                                         IsClientActually = false,
                                                         ContainsApiPlatform = x.OrderPositions
                                                                                .Any(y => y.IsActive && !y.IsDeleted &&
                                                                                          y.OrderPositionAdvertisements
                                                                                           .Any(z => z.Position.PlatformId == apiPlatformId)),
                                                         BranchOfficeOrganizationUnit =
                                                                  x.Firm.OrganizationUnit.BranchOfficeOrganizationUnits.FirstOrDefault(y => y.IsPrimary),
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
                        ContactClientSex = x.Value.Contact == null ? null : ((Gender)x.Value.Contact.GenderCode).ToString(),
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
                                                       .All(y => y.CalculateDiscountViaPercent),
                                  IsBudget = x.OrderPositions
                                              .Any(y => !y.IsDeleted && y.IsActive &&
                                                        y.PricePosition.Position.AccountingMethodEnum == (int)PositionAccountingMethod.PlannedProvision)
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
                                                                       EntityName.Order,
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

            if (!(orderInfo.WorkflowStepId == (int)OrderState.OnRegistration || orderInfo.WorkflowStepId == (int)OrderState.Rejected))
            {
                return new OrderDeactivationPosibility
                    {
                        EntityCode = orderNumber,
                        IsDeactivationAllowed = false,
                        DeactivationDisallowedReason = BLResources.OrderCloseIncorrectState
                    };
            }

            // Выбрать причину из списка причин досрочного расторжения
            if (((orderInfo.TerminationReason == (int)OrderTerminationReason.RejectionOther) ||
                 (orderInfo.TerminationReason == (int)OrderTerminationReason.TemporaryRejectionOther)) && string.IsNullOrEmpty(orderInfo.Comment))
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

        // TODO {all, 20.03.2014}: CalculatePricePerUnit -  полноценная операция в аггрегате Order, а не часть read модели. К тому же, скорее всего, эта операция должна быть в компоненте BLFlex
        public OrderPositionPriceDto CalculatePricePerUnit(long orderId, decimal categoryRate, decimal pricePositionCost)
        {
            // в заказе "BranchOfficeOrganizationUnit" соответсвует городу источнику
            var orderBatch = _finder.Find(Specs.Find.ById<Order>(orderId))
                                    .Select(item => new
                                        {
                                            item.Id,
                                            item.OrderType,
                                            SourceVatRate = item.BranchOfficeOrganizationUnit.BranchOffice.BargainType.VatRate,
                                            DestVatRate = item.DestOrganizationUnit.BranchOfficeOrganizationUnits
                                                              .FirstOrDefault(unit => unit.IsPrimaryForRegionalSales)
                                                              .BranchOffice
                                                              .BargainType
                                                              .VatRate,
                                            item.SourceOrganizationUnitId,
                                            item.DestOrganizationUnitId,
                                            item.FirmId
                                        })
                                    .Single();

            if (orderBatch.OrderType == (int)OrderType.SelfAds || orderBatch.OrderType == (int)OrderType.SocialAds)
            {
                return new OrderPositionPriceDto { CategoryRate = 1m, PricePerUnit = 0m, VatRatio = 0m };
            }

            decimal pricePerUnit,
                    vatRatio;
            if (orderBatch.SourceVatRate == decimal.Zero)
            {
                // Город источник - франчайзи
                if (orderBatch.DestVatRate == decimal.Zero)
                {
                    // Город-назначение - франчайзи
                    pricePerUnit = pricePositionCost;
                }
                else
                {
                    // Город-назначение - филиал
                    pricePerUnit = pricePositionCost * (100 + orderBatch.DestVatRate) / 100m;
                }

                vatRatio = decimal.Zero;
            }
            else
            {
                // Город источник - филиал
                vatRatio = orderBatch.SourceVatRate / 100m;
                pricePerUnit = pricePositionCost;
            }

            return new OrderPositionPriceDto
                {
                    PricePerUnit = pricePerUnit * categoryRate,
                    VatRatio = vatRatio,
                    CategoryRate = categoryRate,
                };
        }

        public IEnumerable<Order> GetOrdersForBargain(long bargainId)
        {
            return _finder.Find<Order>(x => !x.IsDeleted && x.BargainId.HasValue && x.BargainId == bargainId).ToArray();
        }

        public IEnumerable<Order> GetOrdersForDeal(long dealId)
        {
            return _finder.Find<Order>(x => x.DealId == dealId && x.IsActive && !x.IsDeleted).ToArray();
        }

        public OrderPositionRebindingDto GetOrderPositionInfo(long orderPositionId)
        {
            return _finder.Find(Specs.Find.ById<OrderPosition>(orderPositionId))
                          .Select(position => new OrderPositionRebindingDto
                              {
                                  AdverisementCount = position.OrderPositionAdvertisements.Count,
                                  BindingType = (PositionBindingObjectType)position.PricePosition.Position.BindingObjectTypeEnum,
                                  OrderWorkflowSate = (OrderState)position.Order.WorkflowStepId,
                                  OrderId = position.Order.Id
                              })
                          .SingleOrDefault();
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

        public decimal GetVatRate(long? sourceOrganizationUnitId, long destOrganizationUnitId, out bool showVat)
        {
            const decimal DefaultVatRate = 18M;

            var sourceVat = DefaultVatRate;
            if (sourceOrganizationUnitId.HasValue)
            {
                sourceVat = _finder.Find<OrganizationUnit, decimal>(OrganizationUnitSpecs.Select.VatRate(),
                                                                    Specs.Find.ById<OrganizationUnit>(sourceOrganizationUnitId.Value))
                                   .Single();
            }

            var destVat = _finder.Find<OrganizationUnit, decimal>(OrganizationUnitSpecs.Select.VatRate(),
                                                                  Specs.Find.ById<OrganizationUnit>(destOrganizationUnitId))
                                 .Single();

            if (sourceVat == decimal.Zero)
            {
                // Город источник - франчайзи
                showVat = false;
                return destVat;
            }

            // Город источник - филиал
            showVat = true;
            return sourceVat;
        }

        public bool TryAcquireOrderPositions(long projectId,
                                             TimePeriod timePeriod,
                                             IReadOnlyCollection<OrderPositionChargeInfo> orderPositionChargeInfos,
                                             out IReadOnlyDictionary<OrderPositionChargeInfo, long> acquiredOrderPositions,
                                             out string report)
        {
            report = null;
            acquiredOrderPositions = null;
            var errors = new List<string>();

            var organizationUnitId = _finder.Find(Specs.Find.ById<Project>(projectId)).Select(x => x.OrganizationUnitId).SingleOrDefault();

            if (organizationUnitId == null)
            {
                report = string.Format("Can't find appropriate organization unit for project with id = {0}", projectId);
                return false;
            }

            const int ChunkSize = 512;
            var orderPositionsForCharge = new List<OrderPositionBatchItem>();
            for (int position = 0; position < orderPositionChargeInfos.Count; position += ChunkSize)
            {
                var chargeInfoQuery = orderPositionChargeInfos.Skip(position).Take(ChunkSize);
                var firmIds = chargeInfoQuery.Select(x => x.FirmId).Distinct().ToArray();
                var positionIds = chargeInfoQuery.Select(x => x.PositionId).Distinct().ToArray();

                var orderPositionsBatch = _finder.Find(Specs.Find.ActiveAndNotDeleted<Lock>() &&
                                                       AccountSpecs.Locks.Find.ByDestinationOrganizationUnit(organizationUnitId.Value, timePeriod))
                                                 .Select(x => new
                                                     {
                                                         FirmId = x.Order.FirmId,
                                                         OrderPositions = x.Order.OrderPositions.Where(op => op.IsActive && !op.IsDeleted),
                                                     })
                                                 .Where(x => firmIds.Contains(x.FirmId))
                                                 .SelectMany(x => x.OrderPositions.Select(op => new OrderPositionBatchItem
                                                     {
                                                         OrderPositionId = op.Id,
                                                         FirmId = x.FirmId,
                                                         PositionId = op.PricePosition.PositionId,
                                                         CategoryIds = op.OrderPositionAdvertisements.Select(opa => opa.CategoryId).Distinct()
                                                     }))
                                                 .Where(x => positionIds.Contains(x.PositionId))
                                                 .ToArray();

                orderPositionsForCharge.AddRange(orderPositionsBatch);
            }

            var itemsWithNoCategory = orderPositionsForCharge.Where(x => !x.CategoryIds.Any(y => y.HasValue)).ToArray();
            if (itemsWithNoCategory.Any())
            {
                errors.Add(string.Format("Order positions for following charges have no category: [{0}]",
                                         string.Join(", ", itemsWithNoCategory.AsEnumerable())));
            }

            var itemsWithMultipleCategoreis = orderPositionsForCharge.Where(x => x.CategoryIds.Skip(1).Any()).ToArray();
            if (itemsWithMultipleCategoreis.Any())
            {
                errors.Add(string.Format("Order positions for following charges have more than one category: [{0}]",
                                         string.Join(", ", itemsWithMultipleCategoreis.AsEnumerable())));
            }

            var result = new Dictionary<OrderPositionChargeInfo, long>();

            // TODO {a.tukaev, 30.04.2014}: Попробовать заменить на join
            foreach (var orderPositionChargeInfo in orderPositionChargeInfos)
            {
                var chargeInfo = orderPositionChargeInfo;
                var appropriateOrderPositions = orderPositionsForCharge.Where(x => x.FirmId == chargeInfo.FirmId &&
                                                                                   x.PositionId == chargeInfo.PositionId &&
                                                                                   x.CategoryIds.First() == chargeInfo.CategoryId).ToArray();
                if (appropriateOrderPositions.Length == 0)
                {
                    errors.Add(string.Format("Cant't find appropriate order position for charge [{0}]", chargeInfo));
                    continue;
                }

                if (appropriateOrderPositions.Length > 1)
                {
                    errors.Add(string.Format("Multiple appropriate order positions are found for charge [{0}] - [{1}]",
                                             chargeInfo,
                                             string.Join(", ", appropriateOrderPositions.Select(x => x.OrderPositionId))));
                    continue;
                }

                result.Add(chargeInfo, appropriateOrderPositions[0].OrderPositionId);
            }

            if (errors.Any())
            {
                report = string.Join(Environment.NewLine, errors);
                return false;
            }

            acquiredOrderPositions = result;
            return true;
        }




        public long GetOrderOwnerCode(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId)).Select(x => x.OwnerCode).Single();
        }

        private IEnumerable<LinkingObjectsSchemaDto.ThemeDto> FindThemesCanBeUsedWithOrder(Order order)
        {
            var themes = _finder.Find(Specs.Find.ById<OrganizationUnit>(order.DestOrganizationUnitId))
                                .SelectMany(unit => unit.ThemeOrganizationUnits)
                                .Where(Specs.Find.ActiveAndNotDeleted<ThemeOrganizationUnit>())
                                .Select(link => link.Theme)
                                .Where(Specs.Find.ActiveAndNotDeleted<Theme>())
                                .Where(theme => theme.BeginDistribution <= order.BeginDistributionDate
                                                && theme.EndDistribution >= order.EndDistributionDatePlan
                                                && !theme.IsDefault
                                                && !theme.ThemeTemplate.IsSkyScraper)
                                .Select(theme => new LinkingObjectsSchemaDto.ThemeDto
                                    {
                                        Id = theme.Id,
                                        Name = theme.Name
                                    })
                                .ToArray();

            return themes;
        }

        private static bool IsPositionBindingOfSingleType(PositionBindingObjectType type)
        {
            switch (type)
            {
                case PositionBindingObjectType.Firm:
                case PositionBindingObjectType.AddressCategorySingle:
                case PositionBindingObjectType.AddressSingle:
                case PositionBindingObjectType.CategorySingle:
                case PositionBindingObjectType.AddressFirstLevelCategorySingle:
                    return true;
                case PositionBindingObjectType.AddressMultiple:
                case PositionBindingObjectType.CategoryMultiple:
                case PositionBindingObjectType.CategoryMultipleAsterix:
                case PositionBindingObjectType.AddressCategoryMultiple:
                case PositionBindingObjectType.AddressFirstLevelCategoryMultiple:
                case PositionBindingObjectType.ThemeMultiple:
                    return false;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        private Dictionary<long, ContributionTypeEnum?> GetBranchOfficesContributionTypes(params long[] organizationUnitIds)
        {
            var list = _finder.Find<OrganizationUnit>(unit => organizationUnitIds.Contains(unit.Id))
                              .Select(x => new
                                  {
                                      OrgUnitId = x.Id,
                                      ContributionType = x.BranchOfficeOrganizationUnits
                                                          .Where(boou => boou.IsPrimary && boou.IsActive && !boou.IsDeleted)
                                                          .Select(boou => boou.BranchOffice.ContributionTypeId).FirstOrDefault()
                                  })
                              .ToArray();

            return list.ToDictionary(x => x.OrgUnitId, x => (ContributionTypeEnum?)x.ContributionType);
        }
    }

    public class OrderPositionBatchItem
    {
        public long OrderPositionId { get; set; }
        public long FirmId { get; set; }
        public long PositionId { get; set; }
        public IEnumerable<long?> CategoryIds { get; set; }
    }
}