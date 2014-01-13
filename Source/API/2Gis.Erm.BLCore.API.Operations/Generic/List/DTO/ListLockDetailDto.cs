using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO
{
    public sealed class ListLockDetailDto : IListItemEntityDto<LockDetail>
    {
        public long Id { get; set; }
        public DateTime CreateDate { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public long LockId { get; set; }
    }
}