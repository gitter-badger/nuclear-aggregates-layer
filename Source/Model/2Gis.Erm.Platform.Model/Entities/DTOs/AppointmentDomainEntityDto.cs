﻿using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Erm.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public sealed class AppointmentDomainEntityDto : IDomainEntityDto<Appointment>
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public ActivityType Type { get; set; }
        [DataMember]
        public string Header { get; set; }
        [DataMember]
        public DateTime ScheduledStart { get; set; }
        [DataMember]
        public DateTime ScheduledEnd { get; set; }
        [DataMember]
        public DateTime? ActualEnd { get; set; }
        [DataMember]
        public ActivityPriority Priority { get; set; }
        [DataMember]
        public ActivityStatus Status { get; set; }
        [DataMember]
        public ActivityPurpose Purpose { get; set; }
        [DataMember]
        public byte AfterSaleServiceType { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public EntityReference ClientRef { get; set; }
        [DataMember]
        public EntityReference DealRef { get; set; }
        [DataMember]
        public EntityReference FirmRef { get; set; }
        [DataMember]
        public EntityReference ContactRef { get; set; }
        [DataMember]
        public bool IsActive { get; set; }
        [DataMember]
        public bool IsDeleted { get; set; }
        [DataMember]
        public EntityReference OwnerRef { get; set; }
        [DataMember]
        public EntityReference CreatedByRef { get; set; }
        [DataMember]
        public EntityReference ModifiedByRef { get; set; }
        [DataMember]
        public DateTime CreatedOn { get; set; }
        [DataMember]
        public DateTime? ModifiedOn { get; set; }
        [DataMember]
        public byte[] Timestamp { get; set; }
    }
}
