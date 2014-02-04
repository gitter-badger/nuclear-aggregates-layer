using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListUserDto : IListItemEntityDto<User>
    {
        public long Id { get; set; }
        public string DisplayName { get; set; }
        public string Account { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DepartmentName { get; set; }
        public string ParentName { get; set; }
        public IOrderedEnumerable<string> RoleName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}