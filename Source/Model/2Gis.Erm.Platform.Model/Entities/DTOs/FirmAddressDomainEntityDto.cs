using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public partial class FirmAddressDomainEntityDto : IDomainEntityDto<FirmAddress>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference FirmRef { get; set; }

        [DataMember]
        public EntityReference TerritoryRef { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

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
        public Guid ReplicationCode { get; set; }

        [DataMember]
        public bool ClosedForAscertainment { get; set; }

        [DataMember]
        public int SortingPosition { get; set; }

        [DataMember]
        public string PaymentMethods { get; set; }

        [DataMember]
        public string WorkingTime { get; set; }

        [DataMember]
        public long? BuildingCode { get; set; }

        [DataMember]
        public bool IsLocatedOnTheMap { get; set; }

        [DataMember]
        public long? AddressCode { get; set; }

        [DataMember]
        public string ReferencePoint { get; set; }
    }
}