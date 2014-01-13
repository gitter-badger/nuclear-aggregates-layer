namespace DoubleGis.Erm.BLCore.Aggregates.Releases.DTO
{
    public sealed class ValidationReportLine
    {
        public string ValidationMessage { get; set; }

        public long OrderId { get; set; }
        public string Number { get; set; }
        public string OwnerName { get; set; }
        public string FirmName { get; set; }
        public string LegalPersonName { get; set; }
    }
}