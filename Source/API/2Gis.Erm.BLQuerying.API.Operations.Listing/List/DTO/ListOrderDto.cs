using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListOrderDto : IListItemEntityDto<Order>, IRussiaAdapted
    {
        public long Id { get; set; }
        public string OrderNumber { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? FirmId { get; set; }
        public string FirmName { get; set; }
        public long DestOrganizationUnitId { get; set; }
        public string DestOrganizationUnitName { get; set; }
        public long SourceOrganizationUnitId { get; set; }
        public string SourceOrganizationUnitName { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDatePlan { get; set; }
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
    }
}