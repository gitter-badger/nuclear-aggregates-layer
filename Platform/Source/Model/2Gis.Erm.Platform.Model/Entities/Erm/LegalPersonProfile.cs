using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class LegalPersonProfile : IEntity,
                                             IEntityKey,
                                             ICuratedEntity,
                                             IAuditableEntity,
                                             IDeletableEntity,
                                             IDeactivatableEntity,
                                             IStateTrackingEntity,
                                             IPartable
    {
        private long _ownerCode;
        private long? _oldOwnerCode;

        public LegalPersonProfile()
        {
            Orders = new HashSet<Order>();
            OrderProcessingRequests = new HashSet<OrderProcessingRequest>();
        }

        public long Id { get; set; }
        public string Name { get; set; }
        public long LegalPersonId { get; set; }
        public bool IsMainProfile { get; set; }
        public string PositionInNominative { get; set; }
        public string PositionInGenitive { get; set; }
        public string ChiefNameInNominative { get; set; }
        public string ChiefNameInGenitive { get; set; }
        public OperatesOnTheBasisType? OperatesOnTheBasisInGenitive { get; set; }
        public string CertificateNumber { get; set; }
        public DateTime? CertificateDate { get; set; }
        public string WarrantyNumber { get; set; }
        public DateTime? WarrantyEndDate { get; set; }
        public string DocumentsDeliveryAddress { get; set; }
        public string PostAddress { get; set; }
        public string RecipientName { get; set; }
        public DocumentsDeliveryMethod DocumentsDeliveryMethod { get; set; }
        public string EmailForAccountingDocuments { get; set; }
        public string Email { get; set; }
        public string PersonResponsibleForDocuments { get; set; }
        public string Phone { get; set; }
        public string PaymentEssentialElements { get; set; }
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
        public string BargainNumber { get; set; }
        public DateTime? WarrantyBeginDate { get; set; }
        public DateTime? BargainBeginDate { get; set; }
        public DateTime? BargainEndDate { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public string AccountNumber { get; set; }
        public string IBAN { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string Registered { get; set; }
        public string SWIFT { get; set; }
        public DateTime? RegistrationCertificateDate { get; set; }
        public string RegistrationCertificateNumber { get; set; }
        public string BankAddress { get; set; }

        public ICollection<Order> Orders { get; set; }
        public LegalPerson LegalPerson { get; set; }
        public ICollection<OrderProcessingRequest> OrderProcessingRequests { get; set; }

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