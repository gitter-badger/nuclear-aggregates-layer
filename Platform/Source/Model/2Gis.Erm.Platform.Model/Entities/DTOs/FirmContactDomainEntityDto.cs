using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class FirmContactDomainEntityDto : IDomainEntityDto<FirmContact>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference FirmAddressRef { get; set; }

        [DataMember]
        public FirmAddressContactType ContactType { get; set; }

        [DataMember]
        public string Contact { get; set; }

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
        public int SortingPosition { get; set; }

        [DataMember]
        public EntityReference CardRef { get; set; }

        [DataMember]
        public bool IsFirmAddressDeleted { get; set; }

        [DataMember]
        public bool IsFirmDeleted { get; set; }

        [DataMember]
        public bool IsFirmAddressActive { get; set; }

        [DataMember]
        public bool IsFirmActive { get; set; }

        [DataMember]
        public bool FirmAddressClosedForAscertainment { get; set; }

        [DataMember]
        public bool FirmClosedForAscertainment { get; set; }
    }
}