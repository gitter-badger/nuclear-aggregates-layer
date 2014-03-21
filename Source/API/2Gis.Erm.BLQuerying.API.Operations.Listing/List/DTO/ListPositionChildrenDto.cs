using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListPositionChildrenDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string PlatformName { get; set; }
        public string CategoryName { get; set; }
        public long MasterPositionId { get; set; }
        public int ExportCode{ get; set; }
        public long ChildPositionId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}