using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO
{
    public sealed class ListUserRoleDto : IListItemEntityDto<UserRole>
    {
        public long Id { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; }
    }
}