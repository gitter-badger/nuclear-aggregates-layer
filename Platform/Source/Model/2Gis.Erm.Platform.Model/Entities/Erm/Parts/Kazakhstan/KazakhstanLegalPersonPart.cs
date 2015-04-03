using System;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.Kazakhstan
{
    public class KazakhstanLegalPersonPart : IEntityPart
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

        [Obsolete("Not used in domain model")]
        public string Rnn { get; set; }
        public string IdentityCardNumber { get; set; }
        public string IdentityCardIssuedBy { get; set; }
        public DateTime? IdentityCardIssuedOn { get; set; }
    }
}