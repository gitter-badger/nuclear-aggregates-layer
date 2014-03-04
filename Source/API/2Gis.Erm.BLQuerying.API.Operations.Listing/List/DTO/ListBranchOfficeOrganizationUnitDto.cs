using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListBranchOfficeOrganizationUnitDto : IListItemEntityDto<BranchOfficeOrganizationUnit>
    {
        public long Id { get; set; }
        public long BranchOfficeId { get; set; }
        public long OrganizationUnitId { get; set; }
        public string ShortLegalName { get; set; }
        public string BranchOfficeName { get; set; }
        public string OrganizationUnitName { get; set; }
        public bool IsPrimary { get; set; }
    }
}