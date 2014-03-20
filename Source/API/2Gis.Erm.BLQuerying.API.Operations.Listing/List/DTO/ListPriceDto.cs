using System;

using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListPriceDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime PublishDate { get; set; }
        public DateTime BeginDate { get; set; }
        public string OrganizationUnitName { get; set; }
        public string CurrencyName { get; set; }
        public bool IsPublished { get; set; }
        public string Name { get; set; }
        public long OrganizationUnitId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}