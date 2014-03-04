using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListRoleDto : IListItemEntityDto<Role>
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}