using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListPositionDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string PlatformName { get; set; }
        public bool IsComposite { get; set; }
        public string CategoryName { get; set; }
        public int ExportCode { get; set; }
        public bool RestrictChildPositionPlatforms { get; set; }
        public bool IsSupportedByExport { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public string PositionsGroup { get; set; }
    }
}