using System;

using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListAccountDetailDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string OperationType { get; set; }
        public decimal AmountCharged { get; set; }
        public decimal AmountWithdrawn { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
    }
}