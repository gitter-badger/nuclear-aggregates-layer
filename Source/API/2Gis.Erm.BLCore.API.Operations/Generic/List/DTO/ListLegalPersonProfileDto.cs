using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO
{
    public sealed class ListLegalPersonProfileDto : IListItemEntityDto<LegalPersonProfile>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsMainProfile { get; set; }
        public long OwnerCode { get; set; }
        public DateTime CreatedOn { get; set; }
        public string OwnerName { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}