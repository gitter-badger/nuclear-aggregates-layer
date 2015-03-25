using System;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.Kazakhstan
{
    public class KazakhstanLegalPersonProfilePart : IEntityPart
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

        public string ActualAddress { get; set; }
        public string OtherAuthorityDocument { get; set; }
        public string DecreeNumber { get; set; }
        public DateTime? DecreeDate { get; set; }
    }
}