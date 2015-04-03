using System;

using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListPricePositionDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public long PriceId { get; set; }
        public string PositionName { get; set; }
        public decimal Cost { get; set; }
        public long PlatformId { get; set; }
        public string PriceName { get; set; }
        public long PositionId { get; set; }

        public string OrganizationUnitName { get; set; }
        public DateTime BeginDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public int SortingIndex { get; set; }
    }
}