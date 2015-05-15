using System;
using System.Collections.Generic;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Entities.Aspects.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class Currency :
        IEntity,
        IEntityKey,
        IAuditableEntity,
        IDeletableEntity,
        IDeactivatableEntity,
        IReplicableEntity,
        IStateTrackingEntity
    {
        public Currency()
        {
            Countries = new HashSet<Country>();
            CurrencyRates = new HashSet<CurrencyRate>();
            BaseurrencyRates = new HashSet<CurrencyRate>();
            Deals = new HashSet<Deal>();
            Prices = new HashSet<Price>();
            Orders = new HashSet<Order>();
        }

        public long Id { get; set; }
        public Guid ReplicationCode { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public bool IsBase { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public long? DgppId { get; set; }
        public short ISOCode { get; set; }

        public ICollection<Country> Countries { get; set; }
        public ICollection<CurrencyRate> CurrencyRates { get; set; }
        public ICollection<CurrencyRate> BaseurrencyRates { get; set; }
        public ICollection<Deal> Deals { get; set; }
        public ICollection<Price> Prices { get; set; }
        public ICollection<Order> Orders { get; set; }

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