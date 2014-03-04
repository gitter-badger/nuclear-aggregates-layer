using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListPriceDto : IListItemEntityDto<Price>
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
    }
}