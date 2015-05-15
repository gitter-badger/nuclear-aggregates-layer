using System;
using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Entities.Aspects.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class Territory :
        IEntity,
        IEntityKey,
        IAuditableEntity,
        IDeactivatableEntity,
        IReplicableEntity,
        IStateTrackingEntity
    {
        public Territory()
        {
            IsActive = true;
            UserTerritoriesOrganizationUnits = new HashSet<UserTerritoriesOrganizationUnits>();
            Clients = new HashSet<Client>();
            Firms = new HashSet<Firm>();
            FirmAddresses = new HashSet<FirmAddress>();
            Buildings = new HashSet<Building>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long OrganizationUnitId { get; set; }
        public Guid ReplicationCode { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }

        public OrganizationUnit OrganizationUnit { get; set; }
        public ICollection<UserTerritoriesOrganizationUnits> UserTerritoriesOrganizationUnits { get; set; }
        public ICollection<Client> Clients { get; set; }
        public ICollection<Firm> Firms { get; set; }
        public ICollection<FirmAddress> FirmAddresses { get; set; }
        public ICollection<Building> Buildings { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var entityKey = obj as IEntityKey;
            if (entityKey != null)
            {
                return Id == entityKey.Id;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}