using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class ThemeDomainEntityDto : IDomainEntityDto<Theme>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference ThemeTemplateRef { get; set; }

        [DataMember]
        public long FileId { get; set; }

        [DataMember]
        public String FileName { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public DateTime BeginDistribution { get; set; }

        [DataMember]
        public DateTime EndDistribution { get; set; }

        [DataMember]
        public bool IsDefault { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

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
        public ThemeTemplateCode ThemeTemplateCode { get; set; }

        [DataMember]
        public string FileContentType { get; set; }

        [DataMember]
        public long FileContentLength { get; set; }

        [DataMember]
        public int OrganizationUnitCount { get; set; }
    }
}