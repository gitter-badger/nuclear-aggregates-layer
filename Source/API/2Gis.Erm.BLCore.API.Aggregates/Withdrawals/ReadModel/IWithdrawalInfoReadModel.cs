using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Withdrawals.Dto;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Withdrawals.ReadModel
{
    public interface IWithdrawalInfoReadModel : IAggregateReadModel<WithdrawalInfo>
    {
        string GetChargesHistoryMessage(Guid sessionId, ChargesHistoryStatus status);
        IReadOnlyCollection<Charge> GetChargesToDelete(long projectId, TimePeriod timePeriod);
        IReadOnlyCollection<WithdrawalInfoDto> GetBlockingWithdrawals(long destProjectId, TimePeriod period);
        bool TryGetLastChargeHistoryId(long projectId, TimePeriod period, ChargesHistoryStatus status, out Guid id);
   
        IReadOnlyDictionary<long, Guid?> GetActualChargesByProject(TimePeriod period);
        IReadOnlyCollection<OrderPositionWithChargeInfoDto> GetPlannedOrderPositionsWithChargesInfo(long organizationUnitId, TimePeriod period);
    }

    public class OrderPositionWithChargeInfoDto
    {
        public Lock Lock { get; set; }

        public ChargeInfoDto ChargeInfo { get; set; }
        public OrderInfoDto OrderInfo { get; set; }
        public OrderPositionInfoDto OrderPositionInfo { get; set; }
    }

    public class ChargeInfoDto
    {
        public long ProjectId { get; set; }
        public Guid SessionId { get; set; }
        public long PositionId { get; set; }
    }

    public class OrderPositionInfoDto
    {
        public long PurchasedPositionId { get; set; }
        public decimal AmountToWithdraw { get; set; }
        public long PriceId { get; set; }
        public decimal CategoryRate { get; set; }
        public int Amount { get; set; }
        public decimal DiscountSum { get; set; }
        public decimal DiscountPercent { get; set; }
        public bool CalculateDiscountViaPercent { get; set; }
        public long OrderPositionId { get; set; }
    }

    public class OrderInfoDto
    {
        public OrderType OrderType { get; set; }
        public short ReleaseCountFact { get; set; }
        public long SourceOrganizationUnitId { get; set; }
        public long DestOrganizationUnitId { get; set; }
         }
}