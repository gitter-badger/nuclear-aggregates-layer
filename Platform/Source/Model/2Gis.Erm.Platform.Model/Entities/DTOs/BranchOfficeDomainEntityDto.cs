using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class BranchOfficeDomainEntityDto : IDomainEntityDto<BranchOffice>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public Guid ReplicationCode { get; set; }

        [DataMember]
        public long? DgppId { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string LegalAddress { get; set; }

        [DataMember]
        public string Inn { get; set; }

        [DataMember]
        public EntityReference BargainTypeRef { get; set; }

        [DataMember]
        public EntityReference ContributionTypeRef { get; set; }

        [DataMember]
        public string Annotation { get; set; }

        [DataMember]
        public string UsnNotificationText { get; set; }

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
        public string Ic { get; set; }
    }
}