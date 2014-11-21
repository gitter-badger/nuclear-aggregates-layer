using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core;

namespace DoubleGis.Erm.BLCore.API.Common.Enums
{
    [StableContract]
    [DataContract(Name = "MsCrmAfterSaleServiceType", Namespace = ServiceNamespaces.BasicOperations.MsCrm201309)]
    public enum AfterSaleServiceType
    {
        [EnumMember]
        None = 0,
        [EnumMember]
        ASS1 = 1,
        [EnumMember]
        ASS2 = 2,
        [EnumMember]
        ASS3 = 3,
        [EnumMember]
        ASS4 = 4
    }
}
