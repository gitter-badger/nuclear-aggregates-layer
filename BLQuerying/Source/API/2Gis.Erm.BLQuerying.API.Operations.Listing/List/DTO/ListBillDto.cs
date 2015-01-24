using System;

using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListBillDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public string Number { get; set; }
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
        public long OrderId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}