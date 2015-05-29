using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class AppointmentBaseDomainEntityDto : IDomainEntityDto<AppointmentBase>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public Guid ReplicationCode { get; set; }

        [DataMember]
        public EntityReference CreatedByRef { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public EntityReference ModifiedByRef { get; set; }

        [DataMember]
        public DateTime? ModifiedOn { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public byte[] Timestamp { get; set; }

        [DataMember]
        public EntityReference OwnerRef { get; set; }

        [DataMember]
        public string Subject { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public DateTime ScheduledStart { get; set; }

        [DataMember]
        public DateTime ScheduledEnd { get; set; }

        [DataMember]
        public DateTime? ActualEnd { get; set; }

        [DataMember]
        public int Priority { get; set; }

        [DataMember]
        public int Status { get; set; }

        [DataMember]
        public bool IsAllDayEvent { get; set; }

        [DataMember]
        public string Location { get; set; }

        [DataMember]
        public int Purpose { get; set; }
    }
}