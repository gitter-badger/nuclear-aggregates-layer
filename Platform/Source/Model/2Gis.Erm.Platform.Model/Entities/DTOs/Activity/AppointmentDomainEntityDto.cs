﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public sealed class AppointmentDomainEntityDto : IDomainEntityDto<Appointment>
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public string Header { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public DateTime ScheduledStart { get; set; }
        [DataMember]
        public DateTime ScheduledEnd { get; set; }
        [DataMember]
        public ActivityPurpose Purpose { get; set; }
        [DataMember]
        public ActivityStatus Status { get; set; }
        [DataMember]
        public IEnumerable<EntityReference> RegardingObjects { get; set; }
        [DataMember]
        public IEnumerable<EntityReference> Attendees { get; set; }
        [DataMember]
        public string Location { get; set; }
        
        [DataMember]
        public EntityReference OwnerRef { get; set; }
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
    }
}
