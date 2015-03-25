using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class AdditionalFirmServiceDomainEntityDto : IDomainEntityDto<AdditionalFirmService>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string ServiceCode { get; set; }

        [DataMember]
        public bool IsManaged { get; set; }

        [DataMember]
        public string Description { get; set; }
    }
}