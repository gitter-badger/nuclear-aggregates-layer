using System;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public class AcceptanceReportsJournalRecord : IBaseEntity, IEntityKey, IAuditableEntity, IStateTrackingEntity, IDeactivatableEntity, IDeletableEntity
    {
        public long Id { get; set; }
        public long OrganizationUnitId { get; set; }
        public DateTime EndDistributionDate { get; set; }
        public int DocumentsAmount { get; set; }
        public long AuthorId { get; set; }

        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}