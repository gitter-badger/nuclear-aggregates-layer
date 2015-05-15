using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class CategoryFirmAddressDomainEntityDto : IDomainEntityDto<CategoryFirmAddress>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference CategoryRef { get; set; }

        [DataMember]
        public EntityReference FirmAddressRef { get; set; }

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
        public int SortingPosition { get; set; }

        [DataMember]
        public bool IsPrimary { get; set; }
    }
}