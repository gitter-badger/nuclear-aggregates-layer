namespace DoubleGis.Erm.BLCore.API.Aggregates.Deals.DTO
{
    public class DealLegalPersonDto
    {
        public LegalPersonDto LegalPerson { get; set; }
        public long Id { get; set; }
        public string Name { get; set; }
        public long ClientId { get; set; }
        public long CurrencyId { get; set; }
        public long OwnerCode { get; set; }

        public long? MainFirmId { get; set; }

        public class LegalPersonDto
        {
            public long Id { get; set; }
            public string Name { get; set; }
        }
    }
}