using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    [DataContract]
    public class RequestMessageDetailDto
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public long RequestId { get; set; }

        [DataMember]
        public string CreatedBy { get; set; }

        [DataMember]
        public string MessageType { get; set; }

        [DataMember]
        public string MessageText { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public Guid GroupId { get; set; }
    }
}