using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.FirmInfo
{
    [DataContract]
    public class FirmInfoDto
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public IEnumerable<FirmAddressInfoDto> FirmAddresses { get; set; }

        [DataMember]
        public ProjectInfoDto Project { get; set; }

        [DataMember]
        public string Owner { get; set; }
    }

    [DataContract]
    public class FirmAddressInfoDto
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public IEnumerable<CategoryInfoDto> Categories { get; set; }
    }
}