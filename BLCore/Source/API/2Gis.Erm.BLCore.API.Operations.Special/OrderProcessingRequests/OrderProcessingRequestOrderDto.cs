namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    // TODO {d.ivanov, 05.12.2013}: Ляжет в 2Gis.Erm.BLCore.Aggregates\Orders\DTO
    public class OrderProcessingRequestOrderDto
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public long OwnerCode { get; set; }
    }
}