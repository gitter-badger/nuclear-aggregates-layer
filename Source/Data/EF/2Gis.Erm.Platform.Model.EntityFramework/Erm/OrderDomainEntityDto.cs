//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//-----------------------------------------------------------------------------

// ReSharper disable RedundantUsingDirective
// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantNameQualifier
// ReSharper disable ConvertNullableToShortForm

using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Model.Entities.Enums;

namespace DoubleGis.Erm.Platform.Model.Entities.DTOs
{
    [DataContract]
    public partial class OrderDomainEntityDto : IDomainEntityDto<DoubleGis.Erm.Platform.Model.Entities.Erm.Order>
    {
    	[DataMember]
        public long Id { get; set; }
    	[DataMember]
        public System.Guid ReplicationCode { get; set; }
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
        public System.DateTime BeginDistributionDate { get; set; }
    	[DataMember]
        public System.DateTime EndDistributionDatePlan { get; set; }
    	[DataMember]
        public System.DateTime EndDistributionDateFact { get; set; }
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
        public Nullable<System.DateTime> ApprovalDate { get; set; }
    	[DataMember]
        public Nullable<System.DateTime> RejectionDate { get; set; }
    	[DataMember]
        public bool IsTerminated { get; set; }
    	[DataMember]
        public EntityReference DealRef { get; set; }
    	[DataMember]
        public Nullable<long> DgppId { get; set; }
    	[DataMember]
        public EntityReference BargainRef { get; set; }
    	[DataMember]
        public EntityReference OwnerRef { get; set; }
    	[DataMember]
        public EntityReference CreatedByRef { get; set; }
    	[DataMember]
        public System.DateTime CreatedOn { get; set; }
    	[DataMember]
        public EntityReference ModifiedByRef { get; set; }
    	[DataMember]
        public Nullable<System.DateTime> ModifiedOn { get; set; }
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
        public System.DateTime SignupDate { get; set; }
    	[DataMember]
        public string RegionalNumber { get; set; }
    	[DataMember]
        public decimal PayablePrice { get; set; }
    	[DataMember]
        public decimal PayablePlan { get; set; }
    	[DataMember]
        public decimal PayableFact { get; set; }
    	[DataMember]
        public Nullable<decimal> DiscountSum { get; set; }
    	[DataMember]
        public Nullable<decimal> DiscountPercent { get; set; }
    	[DataMember]
        public decimal VatPlan { get; set; }
    	[DataMember]
        public decimal AmountToWithdraw { get; set; }
    	[DataMember]
        public decimal AmountWithdrawn { get; set; }
    	[DataMember]
        public OrderBudgetType BudgetType { get; set; }
    	[DataMember]
        public Nullable<long> InspectorCode { get; set; }
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
    }
}

// ReSharper enable RedundantUsingDirective
// ReSharper enable InconsistentNaming
// ReSharper enable PartialTypeWithSinglePart
// ReSharper enable RedundantNameQualifier
// ReSharper enable ConvertNullableToShortForm
