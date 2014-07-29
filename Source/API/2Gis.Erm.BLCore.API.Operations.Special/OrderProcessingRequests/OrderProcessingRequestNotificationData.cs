namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    // 2+ Ляжет в 2Gis.Erm.BLCore.Aggregates\Orders\DTO
    public class OrderProcessingRequestNotificationData
    {
        public string FirmName { get; set; }
        public string LegalPersonName { get; set; }
        public string BaseOrderNumber { get; set; }
        public string RenewedOrderNumber { get; set; }
    }
}