using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions
{
    [DataContract]
    public sealed class GetInitialDiscountResponse : Response
    {
        [DataMember]
        public decimal DiscountPercent { get; set; }
    }
}
