using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Security;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListDepartmentDto : IListItemEntityDto<Department>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }
        public string ParentName { get; set; }
   }
}