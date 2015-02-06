using System;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Activity
{
    public sealed class Phonecall : IBaseEntity, IAuditableEntity, IStateTrackingEntity, ICuratedEntity, IDeactivatableEntity, IDeletableEntity, IReplicableEntity
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
        public DateTime ScheduledOn { get; set; }
        public ActivityPriority Priority { get; set; }
        public ActivityStatus Status { get; set; }
        public PhonecallPurpose Purpose { get; set; }
    }
}
