using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class LocalMessageDomainEntityDto : IDomainEntityDto<LocalMessage>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public DateTime EventDate { get; set; }

        [DataMember]
        public long FileId { get; set; }

        [DataMember]
        public String FileName { get; set; }

        [DataMember]
        public LocalMessageStatus Status { get; set; }

        [DataMember]
        public string ProcessResult { get; set; }

        [DataMember]
        public EntityReference MessageTypeRef { get; set; }

        [DataMember]
        public EntityReference OrganizationUnitRef { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

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
        public long? ProcessingTime { get; set; }

        [DataMember]
        public IntegrationTypeImport IntegrationTypeImport { get; set; }

        [DataMember]
        public IntegrationTypeExport IntegrationTypeExport { get; set; }

        [DataMember]
        public IntegrationSystem SenderSystem { get; set; }

        [DataMember]
        public IntegrationSystem ReceiverSystem { get; set; }
    }
}