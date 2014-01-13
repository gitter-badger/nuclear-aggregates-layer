using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO
{
    public sealed class ListOperationDto : IListItemEntityDto<Operation>
    {
        public long Id { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public string Type { get; set; }
        public long? OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
        public string Status { get; set; }
        public long OwnerCode { get; set; }
        public string Owner { get; set; }
        public string Description { get; set; }
    }
}