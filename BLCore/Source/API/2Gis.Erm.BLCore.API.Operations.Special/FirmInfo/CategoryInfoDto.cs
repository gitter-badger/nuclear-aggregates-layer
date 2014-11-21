using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.FirmInfo
{
    [DataContract]
    public class CategoryInfoDto
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}