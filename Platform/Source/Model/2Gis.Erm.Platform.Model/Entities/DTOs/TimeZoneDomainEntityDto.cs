using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using TimeZone = DoubleGis.Erm.Platform.Model.Entities.Security.TimeZone;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class TimeZoneDomainEntityDto : IDomainEntityDto<TimeZone>
    {
    	[DataMember]
        public long Id { get; set; }

    	[DataMember]
        public EntityReference TimeZoneRef { get; set; }

    	[DataMember]
        public EntityReference CreatedByRef { get; set; }

    	[DataMember]
        public DateTime CreatedOn { get; set; }

    	[DataMember]
        public EntityReference ModifiedByRef { get; set; }

    	[DataMember]
        public DateTime? ModifiedOn { get; set; }

    	[DataMember]
        public byte[] Timestamp { get; set; }
    }
}
