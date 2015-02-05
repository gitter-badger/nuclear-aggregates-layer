using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Russia
{
    [DataContract]
    public class RussiaBranchOfficeDomainEntityDto : IDomainEntityDto<BranchOffice>, IRussiaAdapted
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string LegalAddress { get; set; }
        [DataMember]
        public string Inn { get; set; }
        [DataMember]
        public long? DgppId { get; set; }
        [DataMember]
        public string Annotation { get; set; }
        [DataMember]
        public string UsnNotificationText { get; set; }
        [DataMember]
        public EntityReference BargainTypeRef { get; set; }
        [DataMember]
        public EntityReference ContributionTypeRef { get; set; }
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
    }
}