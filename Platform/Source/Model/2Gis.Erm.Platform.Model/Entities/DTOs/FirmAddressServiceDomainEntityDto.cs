using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class FirmAddressServiceDomainEntityDto : IDomainEntityDto<FirmAddressService>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public EntityReference FirmAddressRef { get; set; }

        [DataMember]
        public EntityReference ServiceRef { get; set; }

        [DataMember]
        public bool DisplayService { get; set; }

        [DataMember]
        public byte[] Timestamp { get; set; }
    }
}