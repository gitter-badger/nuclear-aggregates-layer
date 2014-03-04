using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListBargainTypeDto : IListItemEntityDto<BargainType>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal VatRate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}