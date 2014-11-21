namespace DoubleGis.Erm.BLCore.API.Aggregates.Users.Dto
{
    public sealed class OrganizationUnitDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public bool ProjectExists { get; set; }
    }
}