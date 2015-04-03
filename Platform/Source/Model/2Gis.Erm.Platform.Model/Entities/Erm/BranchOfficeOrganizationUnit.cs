using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class BranchOfficeOrganizationUnit : IEntity,
                                                       IEntityKey,
                                                       ICuratedEntity,
                                                       IAuditableEntity,
                                                       IDeletableEntity,
                                                       IDeactivatableEntity,
                                                       IReplicableEntity,
                                                       IStateTrackingEntity,
                                                       IPartable
    {
        private long _ownerCode;
        private long? _oldOwnerCode;

        public BranchOfficeOrganizationUnit()
        {
            Accounts = new HashSet<Account>();
            Bargains = new HashSet<Bargain>();
            Orders = new HashSet<Order>();
            PrintFormTemplates = new HashSet<PrintFormTemplate>();
        }

        public long Id { get; set; }
        public Guid ReplicationCode { get; set; }
        public string SyncCode1C { get; set; }
        public string RegistrationCertificate { get; set; }
        public long BranchOfficeId { get; set; }
        public long OrganizationUnitId { get; set; }
        public string ShortLegalName { get; set; }
        public string ApplicationCityName { get; set; }
        public string PositionInNominative { get; set; }
        public string PositionInGenitive { get; set; }
        public string ChiefNameInNominative { get; set; }
        public string ChiefNameInGenitive { get; set; }
        public string Registered { get; set; }
        public string OperatesOnTheBasisInGenitive { get; set; }
        public string Kpp { get; set; }
        public string PaymentEssentialElements { get; set; }
        public string PhoneNumber { get; set; }
        public string ActualAddress { get; set; }
        public string PostalAddress { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsPrimaryForRegionalSales { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }

        public long OwnerCode
        {
            get { return _ownerCode; }

            set
            {
                _oldOwnerCode = _ownerCode;
                _ownerCode = value;
            }
        }

        long? ICuratedEntity.OldOwnerCode
        {
            get { return _oldOwnerCode; }
        }

        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public byte[] Timestamp { get; set; }
        public string Email { get; set; }

        public ICollection<Account> Accounts { get; set; }
        public ICollection<Bargain> Bargains { get; set; }
        public BranchOffice BranchOffice { get; set; }
        public OrganizationUnit OrganizationUnit { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<PrintFormTemplate> PrintFormTemplates { get; set; }

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