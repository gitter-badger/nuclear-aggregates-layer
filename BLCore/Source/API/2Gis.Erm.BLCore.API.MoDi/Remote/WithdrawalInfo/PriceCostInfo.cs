using System.Runtime.Serialization;

using DoubleGis.Erm.BLCore.API.Common.Enums;

namespace DoubleGis.Erm.BLCore.API.MoDi.Remote.WithdrawalInfo
{
    [DataContract]
    public sealed class PriceCostInfo
    {
        [DataMember]
        public long PositionId { get; set; }

        [DataMember]
        public PlatformEnum Platform { get; set; }

        [DataMember]
        public decimal Cost { get; set; }
    }
}