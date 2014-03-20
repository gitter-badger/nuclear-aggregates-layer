using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListBargainFileDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public long FileId { get; set; }
        public BargainFileKind FileKindEnum { get; set; }
        public string FileKind { get; set; }
        public string FileName { get; set; }
        public DateTime CreatedOn { get; set; }
        public long BargainId { get; set; }
        public bool IsDeleted { get; set; }
    }
}