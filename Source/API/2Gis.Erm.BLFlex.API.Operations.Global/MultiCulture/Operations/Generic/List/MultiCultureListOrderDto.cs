using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Generic.List
{
    public sealed class MultiCultureListOrderDto : IOperationSpecificEntityDto, IChileAdapted, ICzechAdapted, ICyprusAdapted, IUkraineAdapted
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public DateTime CreatedOn { get; set; }
        public long FirmId { get; set; }
        public string FirmName { get; set; }
        public long? ClientId { get; set; }
        public string ClientName { get; set; }
        public long DestOrganizationUnitId { get; set; }
        public string DestOrganizationUnitName { get; set; }
        public long SourceOrganizationUnitId { get; set; }
        public string SourceOrganizationUnitName { get; set; }
        public string OrderType { get; set; }
        public string WorkflowStep { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDatePlan { get; set; }
        public DateTime EndDistributionDateFact { get; set; }
        public long? LegalPersonId { get; set; }
        public string LegalPersonName { get; set; }
        public long OwnerCode { get; set; }
        public string OwnerName { get; set; }
        public decimal PayableFact { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal AmountWithdrawn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public decimal PayablePlan { get; set; }
        public long? InspectorCode { get; set; }

        public OrderState WorkflowStepEnum { get; set; }
        public long? AccountId { get; set; }
        public long? DealId { get; set; }
        public long? BargainId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsTerminated { get; set; }
        public DocumentsDebt HasDocumentsDebtEnum { get; set; }
        public OrderType OrderTypeEnum { get; set; }
        public OrderTerminationReason TerminationReasonEnum { get; set; }
    }
}
