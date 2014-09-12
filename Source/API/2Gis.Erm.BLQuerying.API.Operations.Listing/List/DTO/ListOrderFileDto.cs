using DoubleGis.Erm.Platform.API.Core.Operations;

using System;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListOrderFileDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public long FileId { get; set; }
        public string FileKind { get; set; }
        public string FileName { get; set; }
        public long OrderId { get; set; }
        public bool IsDeleted { get; set; }
    }
}