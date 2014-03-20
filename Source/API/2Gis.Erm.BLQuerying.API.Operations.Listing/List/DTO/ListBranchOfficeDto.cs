using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListBranchOfficeDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Ic { get; set; }
        public string Inn { get; set; }
        public string Rut { get; set; }
        public string LegalAddress { get; set; }
        public long? BargainTypeId { get; set; }
        public string BargainType { get; set; }
        public long? ContributionTypeId { get; set; }
        public string ContributionType { get; set; }
        public string Annotation { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}