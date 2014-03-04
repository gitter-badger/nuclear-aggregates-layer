using System.Collections.Generic;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListUserOrganizationUnitDto : IListItemEntityDto<UserOrganizationUnit>
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
        public string UserName { get; set; }
        public string UserDepartmentName { get; set; }
        public IEnumerable<string> UserRoleName { get; set; }
    }
}