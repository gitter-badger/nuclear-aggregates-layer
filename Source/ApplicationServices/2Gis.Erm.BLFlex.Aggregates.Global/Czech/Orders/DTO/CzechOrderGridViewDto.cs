using System;

using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Aggregates.Global.Czech.Orders.DTO
{
    public sealed class CzechOrderGridViewDto : ICzechAdapted
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public long? FirmId { get; set; }
        public string FirmName { get; set; }
        public long? ClientId { get; set; }
        public string ClientName { get; set; }
        public long? BargainId { get; set; }
        public string BargainNumber { get; set; }
        public long DestOrganizationUnitId { get; set; }
        public string DestOrganizationUnitName { get; set; }
        public long SourceOrganizationUnitId { get; set; }
        public string SourceOrganizationUnitName { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDatePlan { get; set; }
        public long? LegalPersonId { get; set; }
        public string LegalPersonName { get; set; }
        public int PaymentMethod { get; set; }
        public long OwnerCode { get; set; }
        public int WorkflowStepId { get; set; }
        public decimal PayablePlan { get; set; }
        public decimal PayableFact { get; set; }
        public decimal AmountWithdrawn { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int OrderType { get; set; }
        public decimal? DiscountPercent { get; set; }
    }
}