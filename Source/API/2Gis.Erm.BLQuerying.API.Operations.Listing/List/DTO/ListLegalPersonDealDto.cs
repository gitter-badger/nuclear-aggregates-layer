using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListLegalPersonDealDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }

        public long LegalPersonId { get; set; }
        public long DealId { get; set; }
        public string LegalName { get; set; }
        public bool IsMain { get; set; }

        public bool IsDeleted { get; set; }
    }
}