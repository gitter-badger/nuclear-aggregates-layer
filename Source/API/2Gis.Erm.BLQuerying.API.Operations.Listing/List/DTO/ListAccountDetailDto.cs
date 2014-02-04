using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListAccountDetailDto : IListItemEntityDto<AccountDetail>
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