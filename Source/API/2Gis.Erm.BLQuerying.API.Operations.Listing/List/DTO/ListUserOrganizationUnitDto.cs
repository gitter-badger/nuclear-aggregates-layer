using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListUserOrganizationUnitDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public long OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
        public string UserName { get; set; }
        public string UserDepartmentName { get; set; }
        public IEnumerable<string> UserRoleName { get; set; }

        public bool UserIsActive { get; set; }
        public bool UserIsDeleted { get; set; }
    }
}