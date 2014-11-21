namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Reports.DTO
{
    public sealed class ReportDto
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string ReportName { get; set; }
        public ulong Timestamp { get; set; }
        public bool IsHidden { get; set; }
        public string FormatParameter { get; set; }
    }
}
