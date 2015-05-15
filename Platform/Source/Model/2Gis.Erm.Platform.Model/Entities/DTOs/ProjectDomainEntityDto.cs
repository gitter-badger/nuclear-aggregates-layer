using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class ProjectDomainEntityDto : IDomainEntityDto<Project>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference OrganizationUnitRef { get; set; }

        [DataMember]
        public string NameLat { get; set; }

        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

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

        [DataMember]
        public string DefaultLang { get; set; }
    }
}