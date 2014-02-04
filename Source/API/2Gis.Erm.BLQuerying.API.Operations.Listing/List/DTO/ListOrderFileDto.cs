using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListOrderFileDto : IListItemEntityDto<OrderFile>
    {
        public long Id { get; set; }
        public long FileId { get; set; }
        public string FileKind { get; set; }
        public string FileName { get; set; }
    }
}