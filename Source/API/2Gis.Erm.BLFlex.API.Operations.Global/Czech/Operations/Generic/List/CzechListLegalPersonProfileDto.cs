using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Czech.Operations.Generic.List
{
    public sealed class CzechListLegalPersonProfileDto : IListItemEntityDto<LegalPersonProfile>, ICzechAdapted
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsMainProfile { get; set; }
        public long OwnerCode { get; set; }
        public string OwnerName { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}