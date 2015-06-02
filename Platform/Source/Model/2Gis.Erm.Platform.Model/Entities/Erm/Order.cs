using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Entities.Aspects.Integration;

namespace DoubleGis.Erm.Platform.Model.Entities.Erm
{
    public sealed class Order :
        IEntity,
        IEntityKey,
        ICuratedEntity,
        IAuditableEntity,
        IDeletableEntity,
        IDeactivatableEntity,
        IReplicableEntity,
        IStateTrackingEntity
    {
        private long _ownerCode;
        private long? _oldOwnerCode;

        public Order()
        {
            Bills = new HashSet<Bill>();
            Locks = new HashSet<Lock>();
            OrderFiles = new HashSet<OrderFile>();
            OrderPositions = new HashSet<OrderPosition>();
            OrderReleaseTotals = new HashSet<OrderReleaseTotal>();
            ReplacingOrders = new HashSet<Order>();
            ReleaseValidationResults = new HashSet<ReleaseValidationResult>();
            OrderValidationResults = new HashSet<OrderValidationResult>();
            BaseOrderProcessingRequests = new HashSet<OrderProcessingRequest>();
            RenewedOrderProcessingRequests = new HashSet<OrderProcessingRequest>();
            OrderValidationCacheEntries = new HashSet<OrderValidationCacheEntry>();
        }

        public long Id { get; set; }
        public Guid ReplicationCode { get; set; }
        public string Number { get; set; }
        public long FirmId { get; set; }
        public long SourceOrganizationUnitId { get; set; }
        public long DestOrganizationUnitId { get; set; }
        public long? CurrencyId { get; set; }
        public long? AccountId { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDatePlan { get; set; }
        public DateTime EndDistributionDateFact { get; set; }
        public int BeginReleaseNumber { get; set; }
        public int EndReleaseNumberPlan { get; set; }
        public int EndReleaseNumberFact { get; set; }
        public short ReleaseCountPlan { get; set; }
        public short ReleaseCountFact { get; set; }
        public long? LegalPersonId { get; set; }
        public long? BranchOfficeOrganizationUnitId { get; set; }
        // TODO {a.tukaev, 10.11.2014}: может избавиться от суффикса Id
        public OrderState WorkflowStepId { get; set; }
        public OrderDiscountReason DiscountReasonEnum { get; set; }
        public string DiscountComment { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? RejectionDate { get; set; }
        public bool IsTerminated { get; set; }
        public long? DealId { get; set; }
        public long? DgppId { get; set; }
        public long? BargainId { get; set; }

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
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public byte[] Timestamp { get; set; }
        public DocumentsDebt HasDocumentsDebt { get; set; }
        public string DocumentsComment { get; set; }
        public long? TechnicallyTerminatedOrderId { get; set; }
        public DateTime SignupDate { get; set; }
        public string RegionalNumber { get; set; }
        public decimal PayablePrice { get; set; }
        public decimal PayablePlan { get; set; }
        public decimal PayableFact { get; set; }
        public decimal? DiscountSum { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal VatPlan { get; set; }
        public decimal AmountToWithdraw { get; set; }
        public decimal AmountWithdrawn { get; set; }
        public long? InspectorCode { get; set; }
        public string Comment { get; set; }
        public OrderType OrderType { get; set; }
        public OrderTerminationReason TerminationReason { get; set; }
        public long? PlatformId { get; set; }
        public long? LegalPersonProfileId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public Bargain Bargain { get; set; }
        public ICollection<Bill> Bills { get; set; }
        public Currency Currency { get; set; }
        public Deal Deal { get; set; }
        public ICollection<Lock> Locks { get; set; }
        public ICollection<OrderFile> OrderFiles { get; set; }
        public ICollection<OrderPosition> OrderPositions { get; set; }
        public ICollection<OrderReleaseTotal> OrderReleaseTotals { get; set; }
        public OrganizationUnit DestOrganizationUnit { get; set; }
        public OrganizationUnit SourceOrganizationUnit { get; set; }
        public Account Account { get; set; }
        public BranchOfficeOrganizationUnit BranchOfficeOrganizationUnit { get; set; }
        public Firm Firm { get; set; }
        public ICollection<Order> ReplacingOrders { get; set; }
        public Order TechnicallyTerminatedOrder { get; set; }
        public ICollection<ReleaseValidationResult> ReleaseValidationResults { get; set; }
        public ICollection<OrderValidationResult> OrderValidationResults { get; set; }
        public Platform Platform { get; set; }
        public LegalPersonProfile LegalPersonProfile { get; set; }
        public LegalPerson LegalPerson { get; set; }
        public ICollection<OrderProcessingRequest> BaseOrderProcessingRequests { get; set; }
        public ICollection<OrderProcessingRequest> RenewedOrderProcessingRequests { get; set; }
        public ICollection<OrderValidationCacheEntry> OrderValidationCacheEntries { get; set; }

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