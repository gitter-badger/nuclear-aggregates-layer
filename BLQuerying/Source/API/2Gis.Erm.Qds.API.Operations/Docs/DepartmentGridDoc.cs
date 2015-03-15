using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.Qds.API.Operations.Docs
{
    public sealed class DepartmentGridDoc : IOperationSpecificEntityDto, IAuthorizationDoc
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        // relations
        public string ParentId { get; set; }
        public string ParentName { get; set; }

        public DocumentAuthorization Authorization { get; set; }
    }
}