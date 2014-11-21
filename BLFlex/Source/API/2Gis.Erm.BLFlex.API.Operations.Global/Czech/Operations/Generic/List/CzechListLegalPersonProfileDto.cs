using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Czech.Operations.Generic.List
{
    public sealed class CzechListLegalPersonProfileDto : ICzechAdapted, IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long LegalPersonId { get; set; }
        public bool IsMainProfile { get; set; }
        public long OwnerCode { get; set; }
        public string OwnerName { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}