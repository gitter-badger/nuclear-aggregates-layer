using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO
{
    public sealed class ListBillDto : IListItemEntityDto<Bill>
    {
        public long Id { get; set; }
        public string BillNumber { get; set; }
        public string OrderNumber { get; set; }
        public long? FirmId { get; set; }
        public string FirmName { get; set; }
        public long? ClientId { get; set; }
        public string ClientName { get; set; }
        public DateTime BeginDistributionDate { get; set; }
        public DateTime EndDistributionDate { get; set; }
        public decimal PayablePlan { get; set; }
        public DateTime PaymentDatePlan { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}