using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLFlex.Model.Entities.DTOs.Chile
{
    [DataContract]
    public sealed class BankDomainEntityDto : IDomainEntityDto<Bank>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public byte[] Timestamp { get; set; }

        public EntityReference CreatedByRef { get; set; }
        public DateTime CreatedOn { get; set; }
        public EntityReference ModifiedByRef { get; set; }
        public DateTime? ModifiedOn { get; set; }

        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}
