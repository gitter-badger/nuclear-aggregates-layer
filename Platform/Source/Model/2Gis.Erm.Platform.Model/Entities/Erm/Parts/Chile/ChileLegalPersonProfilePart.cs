using System;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm.Parts.Chile
{
    public sealed class ChileLegalPersonProfilePart : IEntityPart
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

        public long? BankId { get; set; }
        public AccountType AccountType { get; set; }
        public string RepresentativeRut { get; set; }
        public DateTime? RepresentativeAuthorityDocumentIssuedOn { get; set; }
        public string RepresentativeAuthorityDocumentIssuedBy { get; set; }
    }
}