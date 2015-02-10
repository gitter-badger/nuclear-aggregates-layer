using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public sealed class LetterDomainEntityDto : IDomainEntityDto<Letter>
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public string Header { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public DateTime ScheduledOn { get; set; }
        [DataMember]
        public ActivityPriority Priority { get; set; }
        [DataMember]
        public ActivityStatus Status { get; set; }
        [DataMember]
        public IEnumerable<EntityReference> RegardingObjects { get; set; }
        [DataMember]
        public EntityReference SenderRef { get; set; }
        [DataMember]
        public EntityReference RecipientRef { get; set; }

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
