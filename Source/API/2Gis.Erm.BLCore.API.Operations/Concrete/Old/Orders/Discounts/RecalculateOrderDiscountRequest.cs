using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.Discounts
{
    public sealed class RecalculateOrderDiscountRequest : Request
    {
        public long OrderId { get; set; }
        public OrderType OrderType { get; set; }

        public int ReleaseCountFact { get; set; }
        public bool InPercents { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountSum { get; set; }
    }

    [DataContract]
    public sealed class RecalculateOrderDiscountResponse : Response
    {
        [DataMember]
        public decimal CorrectedDiscountPercent { get; set; }
        [DataMember]
        public decimal CorrectedDiscountSum { get; set; }
    }
}
