using System;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Ukraine
{
    public sealed class UkraineLegalPersonPart : IEntityPart
    {
        public long Id { get; set; }
        public long EntityId { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public string Egrpou { get; set; }
        public TaxationType TaxationType { get; set; }
    }
}