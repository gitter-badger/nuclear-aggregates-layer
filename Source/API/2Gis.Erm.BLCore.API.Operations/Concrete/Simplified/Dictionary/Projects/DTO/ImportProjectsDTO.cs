namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Simplified.Dictionary.Projects.DTO
{
    public sealed class ImportProjectDTO
    {
        public int Code { get; set; }
        public string DisplayName { get; set; }
        public string NameLat { get; set; }
        public bool IsDeleted { get; set; }
        public string DefaultLang { get; set; }
    }
}
