using System;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Entities.Aspects.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Activity
{
    public sealed class Appointment : IBaseEntity, IAuditableEntity, IStateTrackingEntity, ICuratedEntity, IDeactivatableEntity, IDeletableEntity, IReplicableEntity
    {
        public long Id { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public byte[] Timestamp { get; set; }
        public long OwnerCode { get; set; }
        public long? OldOwnerCode { get; set; }
        public Guid ReplicationCode { get; set; }

        public string Header { get; set; }
        public string Description { get; set; }
        public DateTime ScheduledStart { get; set; }
        public DateTime ScheduledEnd { get; set; }
        public ActivityPriority Priority { get; set; }
        public AppointmentPurpose Purpose { get; set; }
        public ActivityStatus Status { get; set; }
        public string Location { get; set; }
    }
}
