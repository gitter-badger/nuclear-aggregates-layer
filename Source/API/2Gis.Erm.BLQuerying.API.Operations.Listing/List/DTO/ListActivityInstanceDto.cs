using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListActivityInstanceDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        // используется в js
        public ActivityType ActivityTypeEnum { get; set; }
        public string ActivityType { get; set; }
        public string Header { get; set; }
        public DateTime ScheduledStart { get; set; }
        public DateTime ScheduledEnd { get; set; }
        public DateTime? ActualEnd { get; set; }
        public string Priority { get; set; }
        public ActivityStatus StatusEnum { get; set; }
        public string Status { get; set; }
        public string AfterSaleServiceType { get; set; }
        public long OwnerCode { get; set; }
        public string OwnerName { get; set; }

        public long? ClientId { get; set; }
        public long? ContactId { get; set; }
        public long? DealId { get; set; }
        public long? FirmId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public ActivityTaskType TaskType { get; set; }
    }
}