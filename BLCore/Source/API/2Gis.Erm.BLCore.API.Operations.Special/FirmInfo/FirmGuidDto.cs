using System;
using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Special.FirmInfo
{
    [DataContract]
    public class FirmGuidDto
    {
        [DataMember]
        public Guid Id { get; set; }
    }
}