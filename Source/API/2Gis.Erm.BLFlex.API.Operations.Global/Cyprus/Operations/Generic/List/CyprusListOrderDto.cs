using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.API.Operations.Global.Cyprus.Operations.Generic.List
{
    public class CyprusListOrderDto : IListItemEntityDto<Order>, ICyprusAdapted
    {
        public long Id { get; set; }
        public string OrderNumber { get; set; }
        public long? FirmId { get; set; }
        public string FirmName { get; set; }
        public long? ClientId { get; set; }
        public string ClientName { get; set; }
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
        public string PaymentMethod { get; set; }
        public long OwnerCode { get; set; }
        public string OwnerName { get; set; }
        public string WorkflowStep { get; set; }
        public decimal PayablePlan { get; set; }
        public decimal AmountWithdrawn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}