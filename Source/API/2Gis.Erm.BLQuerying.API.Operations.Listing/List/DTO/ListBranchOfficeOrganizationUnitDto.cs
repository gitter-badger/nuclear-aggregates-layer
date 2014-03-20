using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListBranchOfficeOrganizationUnitDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public long BranchOfficeId { get; set; }
        public long OrganizationUnitId { get; set; }
        public string ShortLegalName { get; set; }
        public string BranchOfficeName { get; set; }
        public string OrganizationUnitName { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

        public bool OrganizationUnitIsDeleted { get; set; }
        public bool BranchOfficeIsDeleted { get; set; }
    }
}