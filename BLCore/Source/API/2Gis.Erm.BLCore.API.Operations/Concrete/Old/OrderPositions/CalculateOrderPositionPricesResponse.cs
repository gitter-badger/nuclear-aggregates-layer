using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.OrderPositions
{
    [DataContract]
    public sealed class CalculateOrderPositionPricesResponse : Response
    {
        public int ShipmentPlan { get; set; }
        public decimal PricePerUnit { get; set; }
        public decimal PricePerUnitWithVat { get; set; }
        public decimal PayablePrice { get; set; }
        public decimal PayablePlan { get; set; }
        public decimal PayablePlanWoVat { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountSum { get; set; }
    }
}
