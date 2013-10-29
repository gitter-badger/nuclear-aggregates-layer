using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public abstract class ActivityBase : IBaseEntity, IEntityKey, IAuditableEntity, IStateTrackingEntity, ICuratedEntity, IDeactivatableEntity, IDeletableEntity
    {
        public long Id { get; set; }
        public ActivityType Type { get; set; }
        public string Header { get; set; }
        public DateTime ScheduledStart { get; set; }
        public DateTime ScheduledEnd { get; set; }
        public DateTime? ActualEnd { get; set; }
        public ActivityPriority Priority { get; set; }
        public ActivityStatus Status { get; set; }
        public string Description { get; set; }

        public long? ClientId { get; set; }
        public string ClientName { get; set; }

        public long? DealId { get; set; }
        public string DealName { get; set; }

        public long? FirmId { get; set; }
        public string FirmName { get; set; }
        
        public long? ContactId { get; set; }
        public string ContactName { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public byte[] Timestamp { get; set; }
        public long OwnerCode { get; set; }
        public long? OldOwnerCode { get; private set; }
    }
}
