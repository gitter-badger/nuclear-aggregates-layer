using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListUserDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public string DisplayName { get; set; }
        public string Account { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public long? ParentId { get; set; }
        public string ParentName { get; set; }
        public IOrderedEnumerable<string> RoleName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}