using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListOrderDto : IRussiaAdapted, IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public string OrderNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public long FirmId { get; set; }
        public string FirmName { get; set; }
        public long DestOrganizationUnitId { get; set; }
        public string DestOrganizationUnitName { get; set; }
        public long SourceOrganizationUnitId { get; set; }
        public string SourceOrganizationUnitName { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDatePlan { get; set; }
        public DateTime EndDistributionDateFact { get; set; }
        public long? LegalPersonId { get; set; }
        public string LegalPersonName { get; set; }
        public long? BargainId { get; set; }
        public string BargainNumber { get; set; }
        public long OwnerCode { get; set; }
        public string OwnerName { get; set; }
        public string WorkflowStep { get; set; }
        public decimal PayablePlan { get; set; }
        public decimal AmountWithdrawn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public long? InspectorCode { get; set; }

        public OrderState WorkflowStepEnum { get; set; }
        public long? ClientId { get; set; }
        public long? AccountId { get; set; }
        public long? DealId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsTerminated { get; set; }
        public DocumentsDebt HasDocumentsDebtEnum { get; set; }
        public OrderType OrderTypeEnum { get; set; }
        public OrderTerminationReason TerminationReasonEnum { get; set; }
    }
}