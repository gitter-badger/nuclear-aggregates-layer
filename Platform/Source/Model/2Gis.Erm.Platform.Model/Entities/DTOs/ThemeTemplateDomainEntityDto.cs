using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class ThemeTemplateDomainEntityDto : IDomainEntityDto<ThemeTemplate>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public long FileId { get; set; }

        [DataMember]
        public String FileName { get; set; }

        [DataMember]
        public int TemplateCode { get; set; }

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
        public bool IsSkyScraper { get; set; }

        [DataMember]
        public bool IsTemplateUsedInThemes { get; set; }

        [DataMember]
        public string FileContentType { get; set; }

        [DataMember]
        public long FileContentLength { get; set; }
    }
}