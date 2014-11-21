using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.OrderValidation
{
    [DataContract]
    public enum MessageType
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        Info = 1,
        [EnumMember]
        Warning = 2,
        [EnumMember]
        Error = 3,
    }
}
