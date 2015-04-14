using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.Qds.API.Operations.Docs
{
    public sealed class UserGridDoc: IOperationSpecificEntityDto, IAuthorizationDoc
    {
        public long Id { get; set; }
        public string Account { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        //public ICollection<string> TerritoryIds { get; set; }
        //public ICollection<string> OrganizationUnitIds { get; set; }

        // relations
        public string ParentId { get; set; }
        public string ParentName { get; set; }
        public string DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public DocumentAuthorization Authorization { get; set; }
    }
}