using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class FirmAddress : IEntity,
                                      IEntityKey,
                                      IAuditableEntity,
                                      IDeletableEntity,
                                      IDeactivatableEntity,
                                      IReplicableEntity,
                                      IStateTrackingEntity,
                                      IPartable
    {
        public FirmAddress()
        {
            CategoryFirmAddresses = new HashSet<CategoryFirmAddress>();
            FirmContacts = new HashSet<FirmContact>();
            OrderPositionAdvertisements = new HashSet<OrderPositionAdvertisement>();
            FirmAddressServices = new HashSet<FirmAddressService>();
        }

        public long Id { get; set; }
        public long FirmId { get; set; }
        public long? TerritoryId { get; set; }
        public string Address { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public Guid ReplicationCode { get; set; }
        public bool ClosedForAscertainment { get; set; }
        public int SortingPosition { get; set; }
        public string PaymentMethods { get; set; }
        public string WorkingTime { get; set; }
        public long? BuildingCode { get; set; }
        public bool IsLocatedOnTheMap { get; set; }
        public long? AddressCode { get; set; }
        public string ReferencePoint { get; set; }

        public ICollection<CategoryFirmAddress> CategoryFirmAddresses { get; set; }
        public ICollection<FirmContact> FirmContacts { get; set; }
        public ICollection<OrderPositionAdvertisement> OrderPositionAdvertisements { get; set; }
        public Firm Firm { get; set; }
        public Building Building { get; set; }
        public Territory Territory { get; set; }
        public ICollection<FirmAddressService> FirmAddressServices { get; set; }

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

        #region Parts

        private IEnumerable<IEntityPart> _parts;

        public IEnumerable<IEntityPart> Parts
        {
            get { return _parts ?? (_parts = Enumerable.Empty<IEntityPart>()); }

            set
            {
                if (value == null)
                {
                    throw new InvalidOperationException("Parts cannot be null");
                }

                _parts = value;
            }
        }

        #endregion
    }
}