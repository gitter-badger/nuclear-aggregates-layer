using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Chile.Operations.Generic.List
{
    public sealed class ChileListLegalPersonProfileDto : IListItemEntityDto<LegalPersonProfile>, IChileAdapted
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsMainProfile { get; set; }
        public long OwnerCode { get; set; }
        public string OwnerName { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}