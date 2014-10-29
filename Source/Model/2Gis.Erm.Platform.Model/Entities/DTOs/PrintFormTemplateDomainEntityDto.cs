using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public partial class PrintFormTemplateDomainEntityDto : IDomainEntityDto<PrintFormTemplate>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference BranchOfficeOrganizationUnitRef { get; set; }

        [DataMember]
        public long FileId { get; set; }

        [DataMember]
        public String FileName { get; set; }

        [DataMember]
        public TemplateCode TemplateCode { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public EntityReference OwnerRef { get; set; }

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