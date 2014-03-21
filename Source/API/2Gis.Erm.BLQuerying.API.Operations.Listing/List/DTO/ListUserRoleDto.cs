using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListUserRoleDto : IOperationSpecificEntityDto
    {
        public long UserId { get; set; }
        public long Id { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; }
    }
}