using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Common.Crosscutting.ProcessingResults
{
    [DataContract]
    public enum ProcessingResultsMessageType
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