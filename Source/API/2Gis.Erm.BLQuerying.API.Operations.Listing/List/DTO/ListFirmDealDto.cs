using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListFirmDealDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }

        public long FirmId { get; set; }
        public long DealId { get; set; }
        public long? TerritoryId { get; set; }
        public long? ClientId { get; set; }
        public long OwnerCode { get; set; }
        public string FirmName { get; set; }
        public string OwnerName { get; set; }
        public string ClientName { get; set; }
        public string TerritoryName { get; set; }
        public int PromisingScore { get; set; }
        public bool ClosedForAscertainment { get; set; }

        public bool IsDeleted { get; set; }
    }
}