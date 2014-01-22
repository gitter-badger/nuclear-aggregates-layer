using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO
{
    public sealed class ListPositionDto : IListItemEntityDto<Position>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string PlatformName { get; set; }
        public bool IsComposite { get; set; }
        public string CategoryName { get; set; }
        public int ExportCode { get; set; }
        public bool RestrictChildPositionPlatforms { get; set; }
    }
}