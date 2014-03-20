using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListBargainTypeDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal VatRate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}