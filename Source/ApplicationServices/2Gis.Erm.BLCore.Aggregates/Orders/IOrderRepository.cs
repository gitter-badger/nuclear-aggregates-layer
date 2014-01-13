using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders
{
    public interface IOrderRepository : IAggregateRootRepository<Order>,
                                        IAssignAggregateRepository<Order>,
                                        IDeleteAggregateRepository<Bill>,
                                        IDeleteAggregateRepository<OrderPosition>,
                                        IUploadFileAggregateRepository<OrderFile>,
                                        IDownloadFileAggregateRepository<OrderFile>
    {
        Order GetOrder(long orderId);

        Order GetOrderUnsecure(long orderId);

        OrderDiscountsDto GetOrderDiscounts(long orderId);

        OrderUsageDto GetOrderUsage(long orderId);

        IEnumerable<RecipientDto> GetRecipientsForAutoMailer(DateTime startDate, DateTime endDate, bool includeRegionalAdvertisement);

        IEnumerable<OrderPosition> GetOrderPositions(long orderId);

        OrderForProlongationDto GetOrderForProlongationInfo(long orderId);

        OrderPositionWithAdvertisementsDto[] GetOrderPositionsWithAdvertisements(long orderId);

        IEnumerable<RelatedOrderDescriptor> GetRelatedOrdersToCreateBill(long orderId);

        IEnumerable<RelatedOrderDescriptor> GetRelatedOrdersForPrintJointBill(long orderId);

        IEnumerable<OrderPayablePlanInfo> GetPayablePlans(long[] orderIds);

        OrderInfoToGetInitPayments GetOrderInfoForInitPayments(long orderId);

        OrderInfoToCheckOrderBeginDistributionDate GetOrderInfoToCheckOrderBeginDistributionDate(long orderId);
            
        Order GetOrderByBill(long billId);

        OrderWithBillsDto GetOrderWithBills(long orderId);

        OrderPosition[] GetPositions(long orderId);

        IEnumerable<Order> GetOrdersForBargain(long bargainId);

        IEnumerable<Order> GetOrdersForDeal(long dealId);

        IEnumerable<SubPositionDto> GetSelectedSubPositions(long orderPositionId);

        void CloseOrder(Order order, string reason);

        OrderPositionRebindingDto GetOrderPositionInfo(long orderPositionId);

        IDictionary<long, string> GetOrderOrganizationUnitsSyncCodes(params long[] orderId);

        IEnumerable<Order> GetActiveOrdersForLegalPerson(long legalPersonId);

        IEnumerable<OrderWithDummyAdvertisementDto> GetOrdersWithDummyAdvertisement(long organizationUnitId, long ownerCode, bool includeOwnerDescendants);

        Dictionary<long, Dictionary<PlatformEnum, decimal>> GetOrderPlatformDistributions(IEnumerable<long> orderIds,
                                                                                      DateTime startPeriodDate,
                                                                                      DateTime endPeriodDate);

        int Create(Order order);

        int CreateOrUpdate(Bill bill);

        int CreateOrUpdate(OrderFile entity);

        int Update(Order order);

        int CreateOrUpdate(OrderPosition orderPosition);

        int Update(OrderPosition orderPosition);

        int Assign(Order order, long ownerCode);

        int Delete(OrderPosition orderPosition);

        int Delete(Bill bill);

        void CreateOrUpdateOrderPositionAdvertisements(long orderPositionId, AdvertisementDescriptor[] newAdvertisementsLinks, bool orderIsLocked);

        int Delete(IEnumerable<OrderPositionAdvertisement> advertisements);

        void UpdateOrderNumber(Order order);

        void DetermineOrderBudgetType(Order order);

        void SetInspector(long orderId, long? inspectorId);

        OrderPositionPriceDto CalculatePricePerUnit(long orderId, CategoryRate categoryRate, decimal pricePositionCost);

        IEnumerable<long> DetermineOrderPlatforms(long orderId);

        void DetermineOrderPlatform(Order order);

        decimal GetPayablePlanSum(long orderId, int releaseCount);

        OrderFinancialInfo GetFinancialInformation(long orderId);

        OrderCompletionState GetOrderCompletionState(long orderId);
        OrderDeactivationPosibility IsOrderDeactivationPossible(long orderId);
        OrderStateValidationInfo GetOrderStateValidationInfo(long orderId);

        bool IsOrderForOrganizationUnitsPairExist(long orderId, long sourceOrganizationUnitId, long destOrganizationUnitId);

        int SetOrderState(Order order, OrderState orderState);

        void ChangeOrderPositionBindingObjects(long orderPositionId, IEnumerable<AdvertisementDescriptor> advertisements);

        bool OrderPriceWasPublished(long organizationUnitId, DateTime orderBeginDistributionDate);

        // TODO {d.ivanov, 11.11.2013}: Перенести отсюда в read-model
        bool TryGetActualPriceIdForOrder(long orderId, out long actualPriceId);

        // TODO {d.ivanov, 11.11.2013}: Перенести отсюда в read-model
        bool TryGetActualPriceId(long organizationUnitId, DateTime beginDistributionDate, out long actualPriceId);

        bool IsBranchToBranchOrder(Order order);

        OrderState GetOrderState(long orderId);

        OrderType GetOrderType(long orderId);

        OrderPositionDetailedInfo GetOrderPositionDetailedInfo(long? orderPositionId, long orderId, long pricePositionId, bool includeHiddenAddresses);

        bool IsOrganizationUnitsBothBranches(long sourceOrganizationUnitId, long destOrganizationUnitId);

        bool CheckIsBranchToBranchOrder(long sourceOrganizationUnitId, long destOrganizationUnitId, bool throwOnAbsentContribType);

        long GenerateNextOrderUniqueNumber();

        RecalculatedOrderPositionDataDto Recalculate(
            int amount,
            decimal pricePerUnit,
            decimal pricePerUnitWithVat,
            int orderReleaseCountFact,
            bool calculateInPercent,
            decimal newDiscountPercent,
            decimal newDiscountSum);

        Note GetLastNoteForOrder(long orderId, DateTime sinceDate);

        Order CreateCopiedOrder(Order order, IEnumerable<OrderPositionWithAdvertisementsDto> orderPositionDtos);

        // Удаляет объекты OrderReleaseTotal, имеющие отношение к заказу и возвращает идентификаторы удалённых объектов
        long[] DeleteOrderReleaseTotalsForOrder(long orderId);

        void CreateOrderReleaseTotals(IEnumerable<OrderReleaseTotal> orderReleaseTotals);

        ReleaseNumbersDto CalculateReleaseNumbers(long organizationUnitId, DateTime rawBeginDistributuionDate, int releaseCountPlan, int releaseCountFact);
        void UpdateOrderReleaseNumbers(Order order);

        DistributionDatesDto CalculateDistributionDates(DateTime rawBeginDistributuionDate, int releaseCountPlan, int releaseCountFact);
        void UpdateOrderDistributionDates(Order order);
    }
}