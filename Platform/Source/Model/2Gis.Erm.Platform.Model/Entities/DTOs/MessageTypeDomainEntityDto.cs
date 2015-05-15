using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class MessageTypeDomainEntityDto : IDomainEntityDto<MessageType>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public int IntegrationType { get; set; }

        [DataMember]
        public int SenderSystem { get; set; }

        [DataMember]
        public int ReceiverSystem { get; set; }
    }
}