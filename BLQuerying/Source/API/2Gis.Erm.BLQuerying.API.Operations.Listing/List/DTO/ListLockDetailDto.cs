using System;

using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListLockDetailDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public DateTime CreateDate { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public long LockId { get; set; }
        public bool IsActive { get; set; }
    }
}