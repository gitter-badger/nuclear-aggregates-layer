using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class RegionalAdvertisingSharingDomainEntityDto : IDomainEntityDto<RegionalAdvertisingSharing>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public DateTime BeginDistributionDate { get; set; }

        [DataMember]
        public EntityReference SourceOrganizationUnitRef { get; set; }

        [DataMember]
        public EntityReference DestOrganizationUnitRef { get; set; }

        [DataMember]
        public EntityReference SourceBranchOfficeOrganizationUnitRef { get; set; }

        [DataMember]
        public EntityReference DestBranchOfficeOrganizationUnitRef { get; set; }

        [DataMember]
        public decimal TotalAmount { get; set; }

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