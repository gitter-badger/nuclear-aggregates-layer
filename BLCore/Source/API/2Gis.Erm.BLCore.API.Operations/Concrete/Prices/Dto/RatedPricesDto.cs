using System.Runtime.Serialization;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices.Dto
{
    [DataContract]
    public class RatedPricesDto
    {
        [DataMember]
        public decimal PricePerUnit { get; set; }

        [DataMember]
        public decimal PricePerUnitWithVat { get; set; }
    }
}