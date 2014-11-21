using System;

using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListLockDto : IOperationSpecificEntityDto
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
        public bool IsActive { get; set; }
    }
}