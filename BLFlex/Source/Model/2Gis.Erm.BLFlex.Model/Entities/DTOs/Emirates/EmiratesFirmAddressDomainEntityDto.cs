using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Emirates
{
    [DataContract]
    public class EmiratesFirmAddressDomainEntityDto : IDomainEntityDto<Platform.Model.Entities.Erm.FirmAddress>, IEmiratesAdapted
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
        [DataMember]
        public bool IsFirmActive { get; set; }
        [DataMember]
        public bool IsFirmDeleted { get; set; }
        [DataMember]
        public bool FirmClosedForAscertainment { get; set; }

        [DataMember]
        public string PoBox { get; set; }
    }
}
