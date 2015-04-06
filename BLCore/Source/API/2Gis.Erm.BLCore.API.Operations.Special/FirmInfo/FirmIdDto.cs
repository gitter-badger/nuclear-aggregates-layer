using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.FirmInfo
{
    [DataContract]
    public class FirmIdDto
    {
        [DataMember]
        public long Id { get; set; }
    }
}