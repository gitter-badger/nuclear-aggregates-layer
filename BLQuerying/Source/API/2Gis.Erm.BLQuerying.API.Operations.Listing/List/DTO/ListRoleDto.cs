using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListRoleDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}