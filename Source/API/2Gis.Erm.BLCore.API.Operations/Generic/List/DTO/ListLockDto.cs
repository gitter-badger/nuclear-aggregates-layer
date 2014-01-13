using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO
{
    public sealed class ListLockDto : IListItemEntityDto<Lock>
    {
        public string OrderNumber { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime PeriodStartDate { get; set; }
        public DateTime PeriodEndDate { get; set; }
        public decimal PlannedAmount { get; set; }
        public decimal Balance { get; set; }
        public long OwnerCode { get; set; }
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long AccountId { get; set; }
    }
}