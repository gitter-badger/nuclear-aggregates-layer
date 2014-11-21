namespace DoubleGis.Erm.BLCore.API.Operations.Special.OrderProcessingRequests
{
    // TODO {d.ivanov, 05.12.2013}: Ляжет в 2Gis.Erm.BLCore.Aggregates\Orders\DTO
    public class OrderProcessingRequestFirmDto
    {
        public long OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
        public ClientDto Client { get; set; }
        public long CurrencyId { get; set; }
        public long OwnerCode { get; set; }

        public class ClientDto
        {
            public long Id { get; set; }
            public string Name { get; set; }
            public long OwnerCode { get; set; }
        }
    }
}