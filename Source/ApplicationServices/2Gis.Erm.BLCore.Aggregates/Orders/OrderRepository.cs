using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;

using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.OrderPositionAdvertisementValidation;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

// ReSharper disable CheckNamespace
namespace DoubleGis.Erm.BLCore.Aggregates.Orders
// ReSharper restore CheckNamespace
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IOrderPersistenceService _orderPersistenceService;
        private readonly IFinder _finder;
        private readonly ISecureFinder _secureFinder;
        private readonly IFileContentFinder _fileContentFinder;
        private readonly IRepository<Order> _orderGenericRepository;
        private readonly ISecureRepository<Order> _orderSecureGenericRepository;
        private readonly IRepository<OrderPosition> _orderPositionGenericRepository;
        private readonly IRepository<OrderPositionAdvertisement> _orderPositionAdvertisementGenericRepository;
        private readonly IRepository<Bill> _billGenericRepository;
        private readonly IRepository<OrderFile> _orderFileGenericRepository;
        private readonly IRepository<OrderReleaseTotal> _orderReleaseTotalGenericRepository;
        private readonly IRepository<FileWithContent> _fileRepository;
        private readonly ISecurityServiceEntityAccess _entityAccessService;
        private readonly IUserContext _userContext;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IValidateOrderPositionAdvertisementsService _validateOrderPositionAdvertisementsService;

        public OrderRepository(IFinder finder,
                               ISecureFinder secureFinder,
                               IFileContentFinder fileContentFinder,
                               IRepository<Order> orderGenericRepository,
                               IRepository<OrderPosition> orderPositionEntityRepository,
                               IRepository<OrderPositionAdvertisement> orderPositionAdvertisementEntityRepository,
                               IRepository<Bill> billGenericRepository,
                               IRepository<OrderFile> orderFileGenericRepository,
                               IRepository<OrderReleaseTotal> orderReleaseTotalGenericRepository,
                               IRepository<FileWithContent> fileRepository,
                               ISecurityServiceEntityAccess entityAccessService,
                               IUserContext userContext,
                               IOrderPersistenceService orderPersistenceService,
                               ISecureRepository<Order> orderSecureGenericRepository,
                               IIdentityProvider identityProvider,
                               IOperationScopeFactory scopeFactory,
                               IValidateOrderPositionAdvertisementsService validateOrderPositionAdvertisementsService)
        {
            _finder = finder;
            _secureFinder = secureFinder;
            _orderGenericRepository = orderGenericRepository;
            _orderPositionGenericRepository = orderPositionEntityRepository;
            _orderPositionAdvertisementGenericRepository = orderPositionAdvertisementEntityRepository;
            _billGenericRepository = billGenericRepository;
            _orderFileGenericRepository = orderFileGenericRepository;
            _orderReleaseTotalGenericRepository = orderReleaseTotalGenericRepository;
            _entityAccessService = entityAccessService;
            _userContext = userContext;
            _orderPersistenceService = orderPersistenceService;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
            _validateOrderPositionAdvertisementsService = validateOrderPositionAdvertisementsService;
            _fileContentFinder = fileContentFinder;
            _orderSecureGenericRepository = orderSecureGenericRepository;
            _fileRepository = fileRepository;
        }

        public long GenerateNextOrderUniqueNumber()
        {
            return _orderPersistenceService.GenerateNextOrderUniqueNumber();
        }

        public Order GetOrder(long orderId)
        {
            return _secureFinder.Find(Specs.Find.ById<Order>(orderId)).Single();
        }

        public Order GetOrderUnsecure(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId)).SingleOrDefault();
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

        public OrderUsageDto GetOrderUsage(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                .Select(order => new OrderUsageDto { Order = order, AnyLocks = order.Locks.Any(@lock => !@lock.IsDeleted) })
                .Single();
        }

        public int CreateOrUpdate(OrderFile entity)
        {
            using (var scope = _scopeFactory.CreateOrUpdateOperationFor(entity))
            {
                if (entity.IsNew())
                {
                    _identityProvider.SetFor(entity);
                    _orderFileGenericRepository.Add(entity);
                    scope.Added<OrderFile>(entity.Id);
                }
                else
                {
                    _orderFileGenericRepository.Update(entity);
                    scope.Updated<OrderFile>(entity.Id);
                }

                var cnt = _orderFileGenericRepository.Save();
                scope.Complete();

                return cnt;
            }
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
                                                                                ud => ud.AncestorId == ownerCode && ud.DescendantId == x.OwnerCode))) &&
                (x.SourceOrganizationUnitId == organizationUnitId || x.DestOrganizationUnitId == organizationUnitId) &&
                x.OrderPositions.Any(y => y.IsActive && !y.IsDeleted && y.OrderPositionAdvertisements.Any(z => dummyAdvertisements.Contains(z.AdvertisementId))))
                                    .Select(x => new
                                        {
                                            Id = x.Id,
                                            DestOrganizationUnitName = x.DestOrganizationUnit.Name,
                                            SourceOrganizationUnitName = x.SourceOrganizationUnit.Name,
                                            FirmName = x.Firm.Name,
                                            LegalPersonName = x.LegalPerson.LegalName,
                                            Number = x.Number,
                                            BeginDistributionDate = x.BeginDistributionDate,
                                            x.WorkflowStepId,
                                            OwnerCode = x.OwnerCode
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
                                                WorkflowStep = ((OrderState)order.WorkflowStepId).ToStringLocalized(EnumResources.ResourceManager, CultureInfo.CurrentCulture),
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

        public int Create(Order order)
        {
            EnsureOrderApprovalDateSpecified(order);
            EnsureOrderPlatformSpecified(order);

            using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, Order>())
            {
                _identityProvider.SetFor(order);
                _orderGenericRepository.Add(order);

                int cnt = _orderGenericRepository.Save();

                scope.Added<Order>(order.Id)
                     .Complete();
                return cnt;
            }
        }

        public int Update(OrderPosition orderPosition)
        {
            return CreateOrUpdate(orderPosition);
        }

        public int CreateOrUpdate(OrderPosition orderPosition)
        {
            using (var operationScope = _scopeFactory.CreateOrUpdateOperationFor(orderPosition))
            {
                if (orderPosition.IsNew())
                {
                    _identityProvider.SetFor(orderPosition);
                    _orderPositionGenericRepository.Add(orderPosition);
                    operationScope.Added<OrderPosition>(orderPosition.Id);
                }
                else
                {
                    _orderPositionGenericRepository.Update(orderPosition);
                    operationScope.Updated<OrderPosition>(orderPosition.Id);
                }

                var result = _orderPositionGenericRepository.Save();
                operationScope.Complete();
                return result;
            }
        }

        public Order CreateCopiedOrder(Order order, IEnumerable<OrderPositionWithAdvertisementsDto> orderPositionDtos)
        {
            // Чтобы система считала заказ новым
            order.ResetToNew();

            // Чтобы сгенерировать новые номера
            order.Number = string.Empty;
            order.RegionalNumber = null;

            // Чтобы заказ по логике являлся новым
            order.WorkflowStepId = (int)OrderState.OnRegistration;
            order.TerminationReason = (int)OrderTerminationReason.None;
            order.HasDocumentsDebt = (byte)DocumentsDebt.Absent;
            order.SignupDate = DateTime.UtcNow;
            order.Comment = null;
            order.ApprovalDate = null;
            order.RejectionDate = null;
            order.DocumentsComment = null;
            order.DgppId = null;
            order.IsTerminated = false;

            // Деньги должны пересчитываться заново
            order.AmountWithdrawn = 0;
            order.PayableFact = 0;
            order.PayablePlan = 0;

            Create(order);

            // TODO {all, 24.09.2013}: при рефакторинге ApplicationServices, попробовать перевести на Bulk режим внесения изменений
            foreach (var orderPositionDto in orderPositionDtos)
            {
                // Превращаем полученную из БД сущность в новую и привязываем её к новому заказу
                var orderPosition = orderPositionDto.OrderPosition;
                orderPosition.ResetToNew();
                _identityProvider.SetFor(orderPosition);

                orderPosition.OrderId = order.Id;
                orderPosition.DgppId = null;
                orderPosition.Comment = null;

                using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, OrderPosition>())
                {
                    _orderPositionGenericRepository.Add(orderPosition);
                    _orderPositionGenericRepository.Save();

                    scope.Added<OrderPosition>(orderPosition.Id)
                         .Complete();
                }

                foreach (var advertisement in orderPositionDto.Advertisements)
                {
                    // Превращаем полученную из БД сущность в новую и связываем её с новой позицией заказа
                    advertisement.ResetToNew();
                    _identityProvider.SetFor(advertisement);

                    advertisement.OrderPositionId = orderPositionDto.OrderPosition.Id;

                    using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, OrderPositionAdvertisement>())
                    {
                        _orderPositionAdvertisementGenericRepository.Add(advertisement);
                        _orderPositionAdvertisementGenericRepository.Save();

                        scope.Added<OrderPositionAdvertisement>(advertisement.Id)
                             .Complete();
                    }
                }
            }

            return order;
        }

        public void SetInspector(long orderId, long? inspectorId)
        {
            using (var operationScope = _scopeFactory.CreateNonCoupled<SetInspectorIdentity>())
            {
                var order = GetOrder(orderId);
                order.InspectorCode = inspectorId;

                Update(order);

                operationScope
                    .Updated<Order>(orderId)
                    .Complete();
            }
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

        public void DetermineOrderPlatform(Order order)
        {
            var platformIds = DetermineOrderPlatforms(order.Id).ToArray();
            var platformId = platformIds.Count() > 1
                                 ? _finder.Find<DoubleGis.Erm.Platform.Model.Entities.Erm.Platform>(x => x.DgppId == (long)PlatformEnum.Independent).Single().Id
                                 : platformIds.FirstOrDefault();

            order.PlatformId = platformId == 0 ? null : platformId as long?;
        }

        public int Update(Order order)
        {
            EnsureOrderApprovalDateSpecified(order);
            EnsureOrderPlatformSpecified(order);
            EnsureOrderDistributionPeriodNotOverlapsThemeDistributionPeriod(order);

            using (var scope = _scopeFactory.CreateOrUpdateOperationFor(order))
            {
                _orderGenericRepository.Update(order);
                var cnt = _orderGenericRepository.Save();

                scope.Updated<Order>(order.Id)
                     .Complete();
                return cnt;
            }
        }

        public int Assign(Order order, long ownerCode)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<AssignIdentity, Order>())
            {
                var orderPositions = _finder.Find<OrderPosition>(x => x.OrderId == order.Id && !x.IsDeleted && x.IsActive).ToArray();

                foreach (var orderPosition in orderPositions)
                {
                    orderPosition.OwnerCode = ownerCode;
                    _orderPositionGenericRepository.Update(orderPosition);
                    scope.Updated<OrderPosition>(orderPosition.Id);
                }

                _orderPositionGenericRepository.Save();

                order.OwnerCode = ownerCode;

                EnsureOrderApprovalDateSpecified(order);
                EnsureOrderPlatformSpecified(order);
                _orderSecureGenericRepository.Update(order);
                var count = _orderSecureGenericRepository.Save();

                scope
                    .Updated<Order>(order.Id)
                    .Complete();

                return count;
            }
        }

        public int Delete(OrderPosition entity)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, OrderPosition>())
            {
                _orderPositionGenericRepository.Delete(entity);
                scope.Deleted<OrderPosition>(entity.Id)
                     .Complete();
            }

            return _orderPositionGenericRepository.Save();
        }

        public int Delete(Bill entity)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, Bill>())
            {
                _billGenericRepository.Delete(entity);
                var cnt = _billGenericRepository.Save();

                scope.Deleted<Bill>(entity.Id)
                     .Complete();

                return cnt;
            }
        }

        public int Delete(IEnumerable<OrderPositionAdvertisement> advertisements)
        {
            int cnt = 0;
            foreach (var advertisement in advertisements)
            {
                using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, OrderPositionAdvertisement>())
                {
                    _orderPositionAdvertisementGenericRepository.Delete(advertisement);
                    cnt += _orderPositionAdvertisementGenericRepository.Save();

                    scope.Deleted<OrderPositionAdvertisement>(advertisement.Id)
                         .Complete();
                }
            }

            return cnt;
        }

        public long[] DeleteOrderReleaseTotalsForOrder(long orderId)
        {
            var orderReleaseTotals = _finder.Find(Specs.Find.ById<Order>(orderId))
                                            .SelectMany(order => order.OrderReleaseTotals)
                                            .ToArray();

            foreach (var orderReleaseTotal in orderReleaseTotals)
            {
                using (var scope = _scopeFactory.CreateSpecificFor<DeleteIdentity, OrderReleaseTotal>())
                {
                    _orderReleaseTotalGenericRepository.Delete(orderReleaseTotal);
                    _orderReleaseTotalGenericRepository.Save();

                    scope.Deleted<OrderReleaseTotal>(orderReleaseTotal.Id)
                         .Complete();
                }
            }

            return orderReleaseTotals.Select(total => total.Id).ToArray();
        }

        /// <summary>
        /// Обновляет в объекте заказа поля Number, RegionalNumber. После обновления нужно отдельно вызвать <see><cref>Update</cref></see>.
        /// </summary>
        public void UpdateOrderNumber(Order order)
        {
            var numbers = UpdateOrderNumber(order.Number, order.RegionalNumber, order.PlatformId);
            order.Number = numbers.Number;
            order.RegionalNumber = numbers.RegionalNumber;
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

        public void DetermineOrderBudgetType(Order order)
        {
            var orderType = _finder.Find(Specs.Find.ById<Order>(order.Id))
                .SelectMany(o => o.OrderPositions
                                     .Where(op => op.IsActive && !op.IsDeleted &&
                                                  (op.PricePosition.Position.AccountingMethodEnum == (int)PositionAccountingMethod.PlannedProvision ||
                                                  op.PricePosition.Position.AccountingMethodEnum == (int)PositionAccountingMethod.GuaranteedProvision)))
                .Select(x => x.PricePosition.Position.AccountingMethodEnum)
                .FirstOrDefault();

            switch (orderType)
            {
                case (int)PositionAccountingMethod.PlannedProvision:
                    order.BudgetType = (int)OrderBudgetType.Budget;
                    break;
                case (int)PositionAccountingMethod.GuaranteedProvision:
                    order.BudgetType = (int)OrderBudgetType.Sell;
                    break;
                default:
                    order.BudgetType = (int)OrderBudgetType.None;
                    break;
            }
        }

        public bool IsOrganizationUnitsBothBranches(long sourceOrganizationUnitId, long destOrganizationUnitId)
        {
            if (sourceOrganizationUnitId == destOrganizationUnitId)
            {
                throw new InvalidOperationException("sourceOrganizationUnitId and destOrganizationUnitId should be not equal.");
            }

            var contributionTypes = GetBranchOfficesContributionTypes(sourceOrganizationUnitId, destOrganizationUnitId);

            ContributionTypeEnum? destContribType = contributionTypes[destOrganizationUnitId];
            ContributionTypeEnum? sourceContribType = contributionTypes[sourceOrganizationUnitId];

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

        public OrderPositionPriceDto CalculatePricePerUnit(long orderId, CategoryRate categoryRate, decimal pricePositionCost)
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

            decimal pricePerUnit, vatRatio;
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

            if (categoryRate.MustBeCalculated)
            {
                // Максимальный коэффициент по всем рубрикам этой фирмы в том отделении 2гис, где размещается заказ.
                const int defaultCategoryRate = 1;
                var categoryRates = _finder.Find(Specs.Find.ById<Firm>(orderBatch.FirmId))
                                          .SelectMany(firm => firm.FirmAddresses)
                                          .Where(Specs.Find.ActiveAndNotDeleted<FirmAddress>())
                                          .SelectMany(address => address.CategoryFirmAddresses)
                                          .Where(Specs.Find.ActiveAndNotDeleted<CategoryFirmAddress>())
                                          .Select(addressCategory => addressCategory.Category)
                                          .Where(Specs.Find.ActiveAndNotDeleted<Category>())
                                          .SelectMany(category => category.CategoryOrganizationUnits)
                                          .Where(Specs.Find.ActiveAndNotDeleted<CategoryOrganizationUnit>())
                                          .Where(categoryOrganizationUnit => categoryOrganizationUnit.OrganizationUnitId == orderBatch.DestOrganizationUnitId)
                                           .Select(
                                               categoryOrganizationUnit =>
                                               categoryOrganizationUnit.CategoryGroup != null
                                                   ? categoryOrganizationUnit.CategoryGroup.GroupRate
                                                   : defaultCategoryRate)
                                          .ToArray();

                if (!categoryRates.Any())
                {
                    throw new BusinessLogicException(BLResources.PricePositionCannotBeChoosedSinceThereIsNoFirmCategory);
                }

                categoryRate = CategoryRate.Known(categoryRates.Max());
            }

            return new OrderPositionPriceDto
                {
                    PricePerUnit = pricePerUnit * categoryRate.Value,
                    VatRatio = vatRatio,
                    CategoryRate = categoryRate.Value,
                };
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
            var isDeleteAllowed = _entityAccessService.HasEntityAccess(EntityAccessTypes.Delete, EntityName.Order, _userContext.Identity.Code, orderDto.Id, orderDto.OwnerCode, null);
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

            return new OrderDeactivationPosibility { EntityCode = orderNumber, IsDeactivationAllowed = true, DeactivationConfirmation = BLResources.OrderCloseConformation };
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

        public int SetOrderState(Order order, OrderState orderState)
        {
            order.WorkflowStepId = (int)orderState;

            EnsureOrderApprovalDateSpecified(order);
            EnsureOrderPlatformSpecified(order);

            // TODO {all, 09.09.2013}: SetOrderStateIdentity
            using (var scope = _scopeFactory.CreateOrUpdateOperationFor(order))
            {
                _orderGenericRepository.Update(order);
                var cnt = _orderGenericRepository.Save();

                scope.Updated<Order>(order.Id)
                     .Complete();
                return cnt;
            }
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

        public void CloseOrder(Order order, string reason)
        {
            using (var operationScope = _scopeFactory.CreateSpecificFor<UpdateIdentity, Order>())
            {
                order.Comment = reason;
                order.IsActive = false;

                // Удалить позиции
                var orderPositions = _finder.Find<OrderPosition>(x => x.OrderId == order.Id && !x.IsDeleted).ToArray();
                foreach (var orderPosition in orderPositions)
                {
                    _orderPositionGenericRepository.Delete(orderPosition);
                }

                _orderPositionGenericRepository.Save();
                operationScope.Deleted<OrderPosition>(orderPositions.Select(x => x.Id).ToArray());

                // Деактивировать счета на оплату
                var bills = _finder.Find<Bill>(x => x.OrderId == order.Id && x.IsActive && !x.IsDeleted).ToArray();
                foreach (var bill in bills)
                {
                    bill.IsActive = false;
                    _billGenericRepository.Update(bill);
                }

                _billGenericRepository.Save();
                operationScope.Updated<Bill>(bills.Select(x => x.Id).ToArray());

                // Деактивировать файлы к заказу
                var orderFiles = _finder.Find<OrderFile>(x => x.OrderId == order.Id && x.IsActive && !x.IsDeleted).ToArray();
                foreach (var orderFile in orderFiles)
                {
                    orderFile.IsActive = false;
                    _orderFileGenericRepository.Update(orderFile);
                }

                _orderFileGenericRepository.Save();
                operationScope.Updated<OrderFile>(orderFiles.Select(x => x.Id).ToArray());

                EnsureOrderApprovalDateSpecified(order);
                EnsureOrderPlatformSpecified(order);
                _orderGenericRepository.Update(order);
                _orderGenericRepository.Save();
                operationScope.Updated<Order>(order.Id);

                operationScope.Complete();
            }
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

        public void ChangeOrderPositionBindingObjects(long orderPositionId, IEnumerable<AdvertisementDescriptor> advertisements)
        {
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                var orderPositionAdvertisements = _finder.Find<OrderPositionAdvertisement>(x => x.OrderPositionId == orderPositionId).ToArray();
                Delete(orderPositionAdvertisements);

                var advertisementsToCreate = advertisements
                    .Select(dto => new OrderPositionAdvertisement
                        {
                            OrderPositionId = orderPositionId,
                            AdvertisementId = dto.AdvertisementId,
                            CategoryId = dto.CategoryId,
                            FirmAddressId = dto.FirmAddressId,
                            PositionId = dto.PositionId
                        });

                foreach (var advertisement in advertisementsToCreate)
                {
                    using (var scope = _scopeFactory.CreateOrUpdateOperationFor(advertisement))
                    {
                        _orderPositionAdvertisementGenericRepository.Add(advertisement);
                        _orderPositionAdvertisementGenericRepository.Save();
                        scope.Added<OrderPositionAdvertisement>(advertisement.Id)
                             .Complete();
                    }
                }

                transaction.Complete();
            }
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
            else
            {
                return TryGetActualPriceId(orderInfo.DestOrganizationUnitId, orderInfo.BeginDistributionDate, out actualPriceId);
            }
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

        public bool IsBranchToBranchOrder(Order order)
        {
            return CheckIsBranchToBranchOrder(order.SourceOrganizationUnitId, order.DestOrganizationUnitId, true);
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

        public bool OrderPriceWasPublished(long organizationUnitId, DateTime orderBeginDistributionDate)
        {
            return _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId))
                .SelectMany(unit => unit.Prices)
                .Where(Specs.Find.ActiveAndNotDeleted<Price>())
                .Any(price => price.IsPublished && price.BeginDate <= orderBeginDistributionDate);
        }

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
                               item.RatePricePositions,
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

            var categoryRate = pricePositionInfo.RatePricePositions ? CategoryRate.NeedsCalculation : CategoryRate.Default;
            var priceCalulations = CalculatePricePerUnit(order.Id, categoryRate, pricePositionInfo.PricePositionCost);

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
                    PricePerUnit = priceCalulations.PricePerUnit,
                    VatRatio = priceCalulations.VatRatio,
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

        public IEnumerable<Order> GetActiveOrdersForLegalPerson(long legalPersonId)
        {
            return _finder.Find(OrderSpecs.Orders.Find.ActiveOrdersForLegalPerson(legalPersonId)).ToArray();
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

        public int CreateOrUpdate(Bill bill)
        {
            var orderInfo = _finder.Find(Specs.Find.ById<Order>(bill.OrderId))
                .Select(x => new
                    {
                        IsOrderActive = x.WorkflowStepId == (int)OrderState.OnRegistration,
                        BillBeginDistributionDate = bill.BeginDistributionDate,
                        SignupDate = x.SignupDate,
                    })
                .Single();

            if (!orderInfo.IsOrderActive)
            {
                throw new NotificationException(BLResources.CantEditBillsWhenOrderIsNotOnRegistration);
            }

            var endOfPaymentDatePlan = new DateTime(bill.PaymentDatePlan.Year, bill.PaymentDatePlan.Month, bill.PaymentDatePlan.Day)
                                .AddDays(1)
                                .AddSeconds(-1);

            var endOfCheckPeriod = orderInfo.BillBeginDistributionDate.AddMonths(-1).GetEndPeriodOfThisMonth();

            if (orderInfo.SignupDate > bill.PaymentDatePlan && endOfPaymentDatePlan <= endOfCheckPeriod)
            {
                throw new NotificationException(BLResources.BillPaymentDatePlanMustBeInCorrectPeriod);
            }


            using (var scope = _scopeFactory.CreateOrUpdateOperationFor(bill))
            {
                if (bill.IsNew())
                {
                    _identityProvider.SetFor(bill);
                    _billGenericRepository.Add(bill);
                    scope.Added<Bill>(bill.Id);
                }
                else
                {
                    _billGenericRepository.Update(bill);
                    scope.Updated<Bill>(bill.Id);
                }

                var cnt = _billGenericRepository.Save();
                scope.Complete();
                return cnt;
            }
        }

        public void CreateOrUpdateOrderPositionAdvertisements(long orderPositionId, AdvertisementDescriptor[] newAdvertisementsLinks, bool orderIsLocked)
        {
            var oldAdvertisementsLinks = _finder.Find<OrderPosition>(x => x.Id == orderPositionId).SelectMany(x => x.OrderPositionAdvertisements).ToArray();
            ValidateOrderPositionAdvertisementsInLockedOrder(oldAdvertisementsLinks, newAdvertisementsLinks, orderIsLocked);

            // повторяю прежнюю логику. По-хорошему все ошибки можно показать в окошечке. Сейчас этого не делаем, т.к.надо тестировать и релизить.
            var firstError = _validateOrderPositionAdvertisementsService.Validate(orderPositionId, newAdvertisementsLinks).FirstOrDefault();
            if (firstError != null)
            {
                throw new BusinessLogicException(firstError.ErrorMessage);
            }

            using (var operationScope = _scopeFactory.CreateSpecificFor<CreateIdentity, OrderPositionAdvertisement>())
            {
                var deletedOrderPositionAdvertisements = new List<OrderPositionAdvertisement>();
                var insertedOrderPositionAdvertisements = new List<OrderPositionAdvertisement>();
                foreach (var oldAdvertisementsLink in oldAdvertisementsLinks)
                {
                    _orderPositionAdvertisementGenericRepository.Delete(oldAdvertisementsLink);
                    deletedOrderPositionAdvertisements.Add(oldAdvertisementsLink);
                }

                var orderPositionAdvertisements = newAdvertisementsLinks.Select(x => new OrderPositionAdvertisement
                    {
                        OrderPositionId = orderPositionId,
                        PositionId = x.PositionId,
                        AdvertisementId = x.AdvertisementId,
                        FirmAddressId = x.FirmAddressId,
                        CategoryId = x.CategoryId,
                        ThemeId = x.ThemeId,
                    }).ToArray();

                _identityProvider.SetFor(orderPositionAdvertisements);

                foreach (var orderPositionAdvertisement in orderPositionAdvertisements)
                {
                    _orderPositionAdvertisementGenericRepository.Add(orderPositionAdvertisement);
                    insertedOrderPositionAdvertisements.Add(orderPositionAdvertisement);
                }

                _orderPositionAdvertisementGenericRepository.Save();
                operationScope
                    .Deleted<OrderPositionAdvertisement>(deletedOrderPositionAdvertisements.Select(x => x.Id).ToArray())
                    .Added<OrderPositionAdvertisement>(insertedOrderPositionAdvertisements.Select(x => x.Id).ToArray())
                    .Complete();
            }
        }

        public void CreateOrderReleaseTotals(IEnumerable<OrderReleaseTotal> orderReleaseTotals)
        {
            // OrderReleaseTotal в числе тех сущностей, что не имеют Timestamp и не предназначены для редактирования: поддерживают только операции создания, чтения и удаления
            foreach (var orderReleaseTotal in orderReleaseTotals)
            {
                using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, OrderReleaseTotal>())
                {
                    _identityProvider.SetFor(orderReleaseTotal);
                    _orderReleaseTotalGenericRepository.Add(orderReleaseTotal);
                    _orderReleaseTotalGenericRepository.Save();

                    scope.Added<OrderReleaseTotal>(orderReleaseTotal.Id)
                         .Complete();
                }
            }
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

            var apiPlatformId = _finder.Find<DoubleGis.Erm.Platform.Model.Entities.Erm.Platform>(x => x.DgppId == (int)PlatformEnum.Api).Select(x => x.Id).Single();

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
                                                                   x.EndDistributionDateFact >= startDateForNotActualClientPeriod && x.EndDistributionDateFact <= endDateForNotActualClientPeriod)
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

        int IAssignAggregateRepository<Order>.Assign(long entityId, long ownerCode)
        {
            var entity = _finder.Find(Specs.Find.ById<Order>(entityId)).Single();
            return Assign(entity, ownerCode);
        }

        int IDeleteAggregateRepository<OrderPosition>.Delete(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<OrderPosition>(entityId)).Single();
            return Delete(entity);
        }

        StreamResponse IDownloadFileAggregateRepository<OrderFile>.DownloadFile(DownloadFileParams<OrderFile> downloadFileParams)
        {
            var file = _fileContentFinder.Find(Specs.Find.ById<FileWithContent>(downloadFileParams.FileId)).Single();
            return new StreamResponse { FileName = file.FileName, ContentType = file.ContentType, Stream = file.Content };
        }

        UploadFileResult IUploadFileAggregateRepository<OrderFile>.UploadFile(UploadFileParams<OrderFile> uploadFileParams)
        {
            if (uploadFileParams.Content != null && uploadFileParams.Content.Length > 10485760)
            {
                throw new BusinessLogicException(BLResources.FileSizeMustBeLessThan10MB);
            }

            var file = new FileWithContent
            {
                Id = uploadFileParams.FileId,
                ContentType = uploadFileParams.ContentType,
                ContentLength = uploadFileParams.ContentLength,
                Content = uploadFileParams.Content,
                FileName = Path.GetFileName(uploadFileParams.FileName)
            };

            using (var operationScope = _scopeFactory.CreateOrUpdateOperationFor(file))
            {
                if (file.IsNew())
                {
                    _identityProvider.SetFor(file);
                    _fileRepository.Add(file);
                    operationScope.Added<FileWithContent>(file.Id);
                }
                else
                {
                    _fileRepository.Update(file);
                    operationScope.Updated<FileWithContent>(file.Id);
                }

                var orderFile = _finder.Find(Specs.Find.ByFileId<OrderFile>(uploadFileParams.FileId)).FirstOrDefault();
                if (orderFile != null)
                {
                    orderFile.ModifiedOn = DateTime.UtcNow;
                    orderFile.ModifiedBy = _userContext.Identity.Code;

                    _orderFileGenericRepository.Update(orderFile);
                    _orderFileGenericRepository.Save();
                    operationScope.Updated<OrderFile>(orderFile.Id);
                }

                operationScope.Complete();

                return new UploadFileResult
                    {
                        ContentType = file.ContentType,
                        ContentLength = file.ContentLength,
                        FileName = file.FileName,
                        FileId = file.Id
                    };
            }
        }

        int IDeleteAggregateRepository<Bill>.Delete(long entityId)
        {
            var entity = _finder.Find(Specs.Find.ById<Bill>(entityId)).Single();
            return Delete(entity);
        }

        private static void EnsureOrderApprovalDateSpecified(Order order)
        {
            var state = (OrderState)order.WorkflowStepId;
            switch (state)
            {
                case OrderState.OnTermination:
                case OrderState.Approved:
                case OrderState.Archive:
                    if (!order.ApprovalDate.HasValue)
                    {
                        var message = string.Format(BLResources.ApprovalDateMustBeSpecified,
                            state.ToStringLocalized(EnumResources.ResourceManager, EnumResources.Culture));
                        throw new ArgumentException(message);
                    }

                    break;
            }
        }

        private static void EnsureOrderPlatformSpecified(Order order)
        {
            if (order.WorkflowStepId != (int)OrderState.OnRegistration && !order.PlatformId.HasValue)
            {
                throw new ArgumentException(BLResources.PlatformMustBeSpecified, "order");
            }
        }

        // ошибка если как-то смогли изменить позиции у заблокированного заказа
        private static void ValidateOrderPositionAdvertisementsInLockedOrder(OrderPositionAdvertisement[] oldAdvertisementsLinks,
                                                                             AdvertisementDescriptor[] newAdvertisementsLinks,
                                                                             bool orderIsLocked)
        {
            if (!orderIsLocked)
            {
                return;
            }

            bool throwError;

            if (newAdvertisementsLinks.Length != oldAdvertisementsLinks.Length)
            {
                throwError = true;
            }
            else
            {
                // поэлементная сортировка 
                oldAdvertisementsLinks = oldAdvertisementsLinks
                    .OrderBy(x => x.PositionId)
                    .ThenBy(x => x.FirmAddressId)
                    .ThenBy(x => x.CategoryId)
                    .ToArray();

                newAdvertisementsLinks = newAdvertisementsLinks
                    .OrderBy(x => x.PositionId)
                    .ThenBy(x => x.FirmAddressId)
                    .ThenBy(x => x.CategoryId)
                    .ToArray();

                throwError = false;

                for (var i = 0; i < newAdvertisementsLinks.Length; i++)
                {
                    var newAdvertisementsLink = newAdvertisementsLinks[i];
                    var oldAdvertisementsLink = oldAdvertisementsLinks[i];

                    if (newAdvertisementsLink.PositionId != oldAdvertisementsLink.PositionId ||
                        newAdvertisementsLink.FirmAddressId != oldAdvertisementsLink.FirmAddressId ||
                        newAdvertisementsLink.CategoryId != oldAdvertisementsLink.CategoryId)
                    {
                        throwError = true;
                        break;
                    }
                }
            }

            if (throwError)
            {
                throw new NotificationException(BLResources.ChangingAdvertisementLinksIsDeniedWhileOrderIsLocked);
            }
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

        private void EnsureOrderDistributionPeriodNotOverlapsThemeDistributionPeriod(Order order)
        {
            var usedThemes = _finder.Find<OrderPosition>(position => position.OrderId == order.Id)
                                    .Where(position => position.IsActive && !position.IsDeleted)
                                    .SelectMany(position => position.OrderPositionAdvertisements)
                                    .Where(advertisement => advertisement.ThemeId != null)
                                    .Select(advertisement => new { advertisement.Theme.BeginDistribution, advertisement.Theme.EndDistribution })
                                    .ToArray();

            if (!usedThemes.Any())
            {
                return;
            }

            var allowedBeginDistibutionDate = usedThemes.Select(arg => arg.BeginDistribution).Max();
            var allowedEndDistibutionDate = usedThemes.Select(arg => arg.EndDistribution).Min();

            if (order.BeginDistributionDate < allowedBeginDistibutionDate)
            {
                var message = string.Format(BLResources.OrderBeginDistibutionDateIsTooSmall, allowedBeginDistibutionDate.ToShortDateString());
                throw new BusinessLogicException(message);
            }

            if (order.EndDistributionDateFact > allowedEndDistibutionDate)
            {
                var message = string.Format(BLResources.OrderBeginDistibutionDateIsTooLarge, allowedEndDistibutionDate.ToShortDateString());
                throw new BusinessLogicException(message);
            }
        }

        private OrderNumberDto UpdateOrderNumber(string orderNumber, string orderRegionalNumber, long? orderPlatformId)
        {
            const string mobilePostfix = "-Mobile";
            const string apiPostfix = "-API";
            const string onlinePostfix = "-Online";

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
                                        ? (PlatformEnum?)_finder.Find(Specs.Find.ById<DoubleGis.Erm.Platform.Model.Entities.Erm.Platform>(orderPlatformId.Value)).Single().DgppId
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
                                Number = orderNumberRegex.Replace(orderNumber, mobilePostfix),
                                RegionalNumber = string.IsNullOrEmpty(orderRegionalNumber)
                                                     ? null
                                                     : orderNumberRegex.Replace(orderRegionalNumber, mobilePostfix)
                            };

                        case PlatformEnum.Api:
                            return new OrderNumberDto
                            {
                                Number = orderNumberRegex.Replace(orderNumber, apiPostfix),
                                RegionalNumber = string.IsNullOrEmpty(orderRegionalNumber)
                                                     ? null
                                                     : orderNumberRegex.Replace(orderRegionalNumber, apiPostfix)
                            };
                        case PlatformEnum.Online:
                            return new OrderNumberDto
                            {
                                Number = orderNumberRegex.Replace(orderNumber, onlinePostfix),
                                RegionalNumber = string.IsNullOrEmpty(orderRegionalNumber)
                                                     ? null
                                                     : orderNumberRegex.Replace(orderRegionalNumber, onlinePostfix)
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
                                Number = orderNumber + mobilePostfix,
                                RegionalNumber = string.IsNullOrEmpty(orderRegionalNumber)
                                                     ? null
                                                     : orderRegionalNumber + mobilePostfix
                            };
                        case PlatformEnum.Api:
                            return new OrderNumberDto
                            {
                                Number = orderNumber + apiPostfix,
                                RegionalNumber = string.IsNullOrEmpty(orderRegionalNumber)
                                                     ? null
                                                     : orderRegionalNumber + apiPostfix
                            };
                        case PlatformEnum.Online:
                            return new OrderNumberDto
                            {
                                Number = orderNumber + onlinePostfix,
                                RegionalNumber = string.IsNullOrEmpty(orderRegionalNumber)
                                                     ? null
                                                     : orderRegionalNumber + onlinePostfix
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
    }
}
