using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class PlatformDomainEntityDto : IDomainEntityDto<Erm.Platform>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public PositionPlatformPlacementPeriod PlacementPeriodEnum { get; set; }

        [DataMember]
        public PositionPlatformMinPlacementPeriod MinPlacementPeriodEnum { get; set; }

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

        [DataMember]
        public long DgppId { get; set; }

        [DataMember]
        public bool IsSupportedByExport { get; set; }
    }
}