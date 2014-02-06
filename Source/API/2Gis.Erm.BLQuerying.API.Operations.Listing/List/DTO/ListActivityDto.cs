using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListActivityDto : IListItemEntityDto<ActivityInstance>
    {
        public long Id { get; set; }
        public ActivityType Type { get; set; }
        public string ActivityType { get; set; }
        public string Header { get; set; }
        public DateTime ScheduledStart { get; set; }
        public DateTime ScheduledEnd { get; set; }
        public DateTime? ActualEnd { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string AfterSaleServiceType { get; set; }
        public long OwnerCode { get; set; }
        public string OwnerName { get; set; }
    }
}