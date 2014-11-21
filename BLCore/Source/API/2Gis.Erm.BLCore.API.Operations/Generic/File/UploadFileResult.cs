using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.File
{
    [DataContract]
    public sealed class UploadFileResult
    {
        [DataMember]
        public long FileId { get; set; }
        [DataMember]
        public string FileName { get; set; }
        [DataMember]
        public string ContentType { get; set; }
        [DataMember]
        public long ContentLength { get; set; }
    }
}
