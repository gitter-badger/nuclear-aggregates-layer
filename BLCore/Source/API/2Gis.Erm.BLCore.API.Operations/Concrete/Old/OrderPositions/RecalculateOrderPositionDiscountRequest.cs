using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions
{
    public sealed class RecalculateOrderPositionDiscountRequest: Request
    {
        public decimal PricePerUnitWithVat { get; set; }
        public decimal Amount { get; set; }
        public int OrderReleaseCountPlan { get; set; }
        public bool InPercent { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountSum { get; set; }
    }

    [DataContract]
    public sealed class RecalculateOrderPositionDiscountResponse: Response
    {
        [DataMember]
        public decimal CorrectedDiscountPercent { get; set; }
        [DataMember]
        public decimal CorrectedDiscountSum { get; set; }
    }
}
