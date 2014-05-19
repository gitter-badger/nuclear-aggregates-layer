using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Users.Dto
{
    public sealed class OrganizationUnitWithUsersDto
    {
        public OrganizationUnit Unit { get; set; }
        public bool HasLinkedUsers { get; set; }
    }
}