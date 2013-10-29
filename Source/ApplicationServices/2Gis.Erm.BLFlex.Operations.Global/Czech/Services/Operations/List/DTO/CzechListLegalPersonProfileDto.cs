using System;

using DoubleGis.Erm.BL.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.Core.Services.Operations.Concrete.List.DTO
{
    public class CzechListLegalPersonProfileDto : IListItemEntityDto<LegalPersonProfile>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsMainProfile { get; set; }
        public long OwnerCode { get; set; }
        public string OwnerName { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}