using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListProjectDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public string DisplayName { get; set; }
        public long? OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
        public bool IsActive { get; set; }
    }
}