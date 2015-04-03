using System;
using System.Runtime.Serialization;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public class OrderDomainEntityDto : IDomainEntityDto<Order>
    {
        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public Guid ReplicationCode { get; set; }

        [DataMember]
        public string Number { get; set; }

        [DataMember]
        public EntityReference FirmRef { get; set; }

        [DataMember]
        public EntityReference SourceOrganizationUnitRef { get; set; }

        [DataMember]
        public EntityReference DestOrganizationUnitRef { get; set; }

        [DataMember]
        public EntityReference CurrencyRef { get; set; }

        [DataMember]
        public EntityReference AccountRef { get; set; }

        [DataMember]
        public DateTime BeginDistributionDate { get; set; }

        [DataMember]
        public DateTime EndDistributionDatePlan { get; set; }

        [DataMember]
        public DateTime EndDistributionDateFact { get; set; }

        [DataMember]
        public int BeginReleaseNumber { get; set; }

        [DataMember]
        public int EndReleaseNumberPlan { get; set; }

        [DataMember]
        public int EndReleaseNumberFact { get; set; }

        [DataMember]
        public short ReleaseCountPlan { get; set; }

        [DataMember]
        public short ReleaseCountFact { get; set; }

        [DataMember]
        public EntityReference LegalPersonRef { get; set; }

        [DataMember]
        public EntityReference BranchOfficeOrganizationUnitRef { get; set; }

        [DataMember]
        public OrderState WorkflowStepId { get; set; }

        [DataMember]
        public OrderDiscountReason DiscountReasonEnum { get; set; }

        [DataMember]
        public string DiscountComment { get; set; }

        [DataMember]
        public DateTime? ApprovalDate { get; set; }

        [DataMember]
        public DateTime? RejectionDate { get; set; }

        [DataMember]
        public bool IsTerminated { get; set; }

        [DataMember]
        public EntityReference DealRef { get; set; }

        [DataMember]
        public long? DgppId { get; set; }

        [DataMember]
        public EntityReference BargainRef { get; set; }

        [DataMember]
        public EntityReference OwnerRef { get; set; }

        [DataMember]
        public EntityReference CreatedByRef { get; set; }

        [DataMember]
        public DateTime CreatedOn { get; set; }

        [DataMember]
        public EntityReference ModifiedByRef { get; set; }

        [DataMember]
        public DateTime? ModifiedOn { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public bool IsDeleted { get; set; }

        [DataMember]
        public byte[] Timestamp { get; set; }

        [DataMember]
        public DocumentsDebt HasDocumentsDebt { get; set; }

        [DataMember]
        public string DocumentsComment { get; set; }

        [DataMember]
        public EntityReference TechnicallyTerminatedOrderRef { get; set; }

        [DataMember]
        public DateTime SignupDate { get; set; }

        [DataMember]
        public string RegionalNumber { get; set; }

        [DataMember]
        public decimal PayablePrice { get; set; }

        [DataMember]
        public decimal PayablePlan { get; set; }

        [DataMember]
        public decimal PayableFact { get; set; }

        [DataMember]
        public decimal? DiscountSum { get; set; }

        [DataMember]
        public decimal? DiscountPercent { get; set; }

        [DataMember]
        public decimal VatPlan { get; set; }

        [DataMember]
        public decimal AmountToWithdraw { get; set; }

        [DataMember]
        public decimal AmountWithdrawn { get; set; }

        [DataMember]
        public long? InspectorCode { get; set; }

        [DataMember]
        public string Comment { get; set; }

        [DataMember]
        public OrderType OrderType { get; set; }

        [DataMember]
        public OrderTerminationReason TerminationReason { get; set; }

        [DataMember]
        public EntityReference PlatformRef { get; set; }

        [DataMember]
        public EntityReference LegalPersonProfileRef { get; set; }

        [DataMember]
        public PaymentMethod PaymentMethod { get; set; }

        [DataMember]
        public string OrderNumber { get; set; }

        [DataMember]
        public EntityReference ClientRef { get; set; }

        [DataMember]
        public bool HasAnyOrderPosition { get; set; }

        [DataMember]
        public bool HasDestOrganizationUnitPublishedPrice { get; set; }

        [DataMember]
        public long? DealCurrencyId { get; set; }

        [DataMember]
        public OrderState PreviousWorkflowStepId { get; set; }

        [DataMember]
        public bool DiscountPercentChecked { get; set; }

        [DataMember]
        public EntityReference InspectorRef { get; set; }

        [DataMember]
        public string Platform { get; set; }

        [DataMember]
        public bool CanSwitchToAccount { get; set; }

        [DataMember]
        public bool ShowRegionalAttributes { get; set; }
    }
}