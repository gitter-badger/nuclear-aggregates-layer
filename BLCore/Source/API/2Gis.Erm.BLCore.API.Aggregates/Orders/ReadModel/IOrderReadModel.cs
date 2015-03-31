using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel.DTO;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Bills;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Orders;
using DoubleGis.Erm.BLCore.API.OrderValidation;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel
{
    public interface IOrderReadModel : IAggregateReadModel<Order>
    {
        IReadOnlyDictionary<long, byte[]> GetOrdersCurrentVersions(Expression<Func<Order, bool>> ordersPredicate);
        IReadOnlyDictionary<long, IEnumerable<long>> GetRelatedOrdersByFirm(IEnumerable<long> orderIds);
        IEnumerable<OrderReleaseInfo> GetOrderReleaseInfos(long organizationUnitId, TimePeriod period);
        IEnumerable<long> GetOrderIdsForRelease(long organizationUnitId, TimePeriod period);
        OrderValidationAdditionalInfo[] GetOrderValidationAdditionalInfos(IEnumerable<long> orderIds);
        IEnumerable<Order> GetOrdersCompletelyReleasedBySourceOrganizationUnit(long sourceOrganizationUnitId);
        IEnumerable<OrderWithDummyAdvertisementDto> GetOrdersWithDummyAdvertisement(long organizationUnitId, long ownerCode, bool includeOwnerDescendants);
        IDictionary<long, string> PickInactiveOrDeletedOrderPositionNames(IEnumerable<long> orderPositionIds);
        IEnumerable<long> GetExistingOrderPositionIds(IEnumerable<long> orderPositionIds);

        Dictionary<long, Dictionary<PlatformEnum, decimal>> GetOrderPlatformDistributions(
            IEnumerable<long> orderIds,
            DateTime startPeriodDate,
            DateTime endPeriodDate);

        long? EvaluateOrderPlatformId(long orderId);
        OrderNumberDto EvaluateOrderNumbers(string orderNumber, string orderRegionalNumber, long? orderPlatformId);
        IEnumerable<Order> GetActiveOrdersForLegalPerson(long legalPersonId);
        Order GetOrderByBill(long billId);
        OrderWithBillsDto GetOrderWithBills(long orderId);
        OrderPosition[] GetPositions(long orderId);

        RecalculatedOrderPositionDataDto Recalculate(
            int amount,
            decimal pricePerUnit,
            decimal pricePerUnitWithVat,
            int orderReleaseCountFact,
            bool calculateInPercent,
            decimal newDiscountPercent,
            decimal newDiscountSum);

        Note GetLastNoteForOrder(long orderId, DateTime sinceDate);
        bool IsOrganizationUnitsBothBranches(long sourceOrganizationUnitId, long destOrganizationUnitId);

        /// <summary>
        ///     Проверка на связь "Филиал-филиал".
        /// </summary>
        bool CheckIsBranchToBranchOrder(long sourceOrganizationUnitId, long destOrganizationUnitId, bool throwOnAbsentContribType);

        bool IsBranchToBranchOrder(Order order);
        bool TryGetActualPriceIdForOrder(long orderId, out long actualPriceId);
        bool TryGetActualPriceId(long organizationUnitId, DateTime beginDistributionDate, out long actualPriceId);
        Order GetOrderSecure(long orderId);
        OrderLinkingObjectsDto GetOrderLinkingObjectsDto(long orderId);
        bool OrderPriceWasPublished(long organizationUnitId, DateTime orderBeginDistributionDate);
        OrderForProlongationDto GetOrderForProlongationInfo(long orderId);
        OrderState GetOrderState(long orderId);
        OrderType GetOrderType(long orderId);
        OrderPositionWithAdvertisementsDto[] GetOrderPositionsWithAdvertisements(long orderId);
        IEnumerable<OrderPosition> GetOrderPositions(long orderId);
        IDictionary<long, string> GetOrderOrganizationUnitsSyncCodes(params long[] organizationUnitId);
        IEnumerable<RelatedOrderDescriptor> GetRelatedOrdersToCreateBill(long orderId);
        IEnumerable<RelatedOrderDescriptor> GetRelatedOrdersForPrintJointBill(long orderId);
        OrderInfoToCheckOrderBeginDistributionDate GetOrderInfoToCheckOrderBeginDistributionDate(long orderId);
        IEnumerable<OrderPayablePlanInfo> GetPayablePlans(long[] orderIds);
        OrderInfoToGetInitPayments GetOrderInfoForInitPayments(long orderId);
        IEnumerable<RecipientDto> GetRecipientsForAutoMailer(DateTime startDate, DateTime endDate, bool includeRegional);
        ReleaseNumbersDto CalculateReleaseNumbers(long organizationUnitId, DateTime rawBeginDistributuionDate, int releaseCountPlan, int releaseCountFact);
        void UpdateOrderReleaseNumbers(Order order);
        DistributionDatesDto CalculateDistributionDates(DateTime rawBeginDistributuionDate, int releaseCountPlan, int releaseCountFact);
        void UpdateOrderDistributionDates(Order order);
        decimal GetPayablePlanSum(long orderId, int releaseCount);
        OrderFinancialInfo GetFinancialInformation(long orderId);
        OrderCompletionState GetOrderCompletionState(long orderId);
        OrderDeactivationPosibility IsOrderDeactivationPossible(long orderId);
        OrderStateValidationInfo GetOrderStateValidationInfo(long orderId);
        bool IsOrderForOrganizationUnitsPairExist(long orderId, long sourceOrganizationUnitId, long destOrganizationUnitId);
        OrderPositionPriceDto CalculatePricePerUnit(long orderId, decimal categoryRate, decimal pricePositionCost);
        IEnumerable<Order> GetOrdersForDeal(long dealId);
        OrderPositionAdvertisementLinksDto GetOrderPositionAdvertisementLinksInfo(long orderPositionId);
        OrderUsageDto GetOrderUsage(long orderId);
        OrderDiscountsDto GetOrderDiscounts(long orderId);
        Order GetOrderUnsecure(long orderId);
        IEnumerable<SubPositionDto> GetSelectedSubPositions(long orderPositionId);
        decimal GetVatRate(long? sourceOrganizationUnitId, long destOrganizationUnitId, out bool showVat);

        long GetOrderOwnerCode(long orderId);

        IReadOnlyCollection<Bargain> GetNonClosedClientBargains();
        Bargain GetBargain(long? bargainId);
        string GetDuplicateAgentBargainNumber(long bargainId, long legalPersonId, long branchOfficeOrganizationUnitId, DateTime bargainBeginDate, DateTime bargainEndDate);
        IDictionary<string, DateTime> GetBargainUsage(long bargainId);
        BargainEndAndCloseDatesDto GetBargainEndAndCloseDates(long bargainId);
        IEnumerable<OrderSuitableBargainDto> GetSuitableBargains(long legalPersonId, long branchOfficeOrganizationUnitId, DateTime orderEndDistributionDate);

        OrderOrganizationUnitDerivedFieldsDto GetFieldValuesByOrganizationUnit(long organizationUnitId);
        OrderParentEntityDerivedFieldsDto GetOrderFieldValuesByParentEntity(EntityName parentEntityName, long parentEntityId);
        long? GetBargainIdByOrder(long orderId);
        long GetLegalPersonIdByBargain(long bargainId);

        OrderAmountToWithdrawInfo GetOrderAmountToWithdrawInfo(long orderId);
        OrderRecalculateWithdrawalsDto GetOrderRecalculateWithdrawalsInfo(long orderId);
        OrderDeleteOrderPositionDto GetOrderPositionDeleteInfo(long orderPositionId);
        OrderRepairOutdatedOrderPositionDto GetOrderInfoForRepairOutdatedPositions(long orderId);
        decimal? TakeAmountToWithdrawForOrder(long orderId, int skip, int take);
        OrderDtoToCheckPossibilityOfOrderPositionCreation GetOrderInfoToCheckPossibilityOfOrderPositionCreation(long orderId);
        IEnumerable<Bill> GetBillsForOrder(long orderId);
        OrderLegalPersonProfileDto GetLegalPersonProfileByOrder(long orderId);
        OrderLegalPersonProfileDto GetLegalPersonProfileByBargain(long bargainId);
        long? GetLegalPersonProfileIdByOrder(long orderId);
        IEnumerable<Order> GetActiveOrdersForLegalPersonProfile(long legalPersonProfileId);
        SalesModel GetOrderSalesModel(long orderId);
        LegalPerson GetLegalPerson(long? legalPersonId);
        LegalPersonProfile GetLegalPersonProfile(long? profileId);
        Currency GetCurrency(long? id);
    }
}