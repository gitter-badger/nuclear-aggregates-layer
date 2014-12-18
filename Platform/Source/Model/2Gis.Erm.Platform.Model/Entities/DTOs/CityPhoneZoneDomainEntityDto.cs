using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class CityPhoneZoneDomainEntityDto : IDomainEntityDto<CityPhoneZone>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public long CityCode { get; set; }

        [DataMember]
        public bool? IsDefault { get; set; }

        [DataMember]
        public bool? IsDeleted { get; set; }
    }
}