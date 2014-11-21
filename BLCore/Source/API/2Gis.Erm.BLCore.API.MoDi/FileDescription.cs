using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.MoDi
{
    [DataContract]
    public class FileDescription
    {
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string ContentType { get; set; }
        [DataMember]
        public byte[] Stream { get; set; }
    }
}