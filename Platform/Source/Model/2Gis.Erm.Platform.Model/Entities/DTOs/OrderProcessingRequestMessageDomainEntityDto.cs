using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class OrderProcessingRequestMessageDomainEntityDto : IDomainEntityDto<OrderProcessingRequestMessage>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference OrderRequestRef { get; set; }

        [DataMember]
        public RequestMessageType MessageType { get; set; }

        [DataMember]
        public int MessageTemplateCode { get; set; }

        [DataMember]
        public string MessageParameters { get; set; }

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
        public EntityReference GroupRef { get; set; }
    }
}