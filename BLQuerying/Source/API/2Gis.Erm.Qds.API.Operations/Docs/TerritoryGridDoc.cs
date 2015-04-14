namespace DoubleGis.Erm.Qds.API.Operations.Docs
{
    public sealed class TerritoryGridDoc
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
        public bool IsActive { get; set; }

        public DocumentAuthorization Authorization { get; set; }
    }
}