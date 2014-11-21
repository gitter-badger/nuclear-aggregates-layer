using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.FirmInfo
{
    [DataContract]
    public class ProjectInfoDto
    {
        [DataMember]
        public long Code { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}