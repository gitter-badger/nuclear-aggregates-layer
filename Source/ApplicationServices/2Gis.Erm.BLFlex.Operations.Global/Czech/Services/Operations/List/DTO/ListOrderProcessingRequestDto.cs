using System;

using DoubleGis.Erm.Model.Entities.Erm;

namespace DoubleGis.Erm.Core.Services.Operations.Concrete.List.DTO
{
    public class ListOrderProcessingRequestDto : IListItemEntityDto<OrderProcessingRequest>
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public long? BaseOrderId { get; set; }
        public string BaseOrderNumber { get; set; }
        public long? RenewedOrderId { get; set; }
        public string RenewedOrderNumber { get; set; }
        public string State { get; set; }
        public long OwnerCode { get; set; }
        public string OwnerName { get; set; }
        public DateTime? BeginDistributionDate { get; set; }
        public long? FirmId { get; set; }
        public string FirmName { get; set; }
        public DateTime DueDate { get; set; }
        public long? LegalPersonProfileId { get; set; }
        public string LegalPersonProfileName { get; set; }
        public long? SourceOrganizationUnitId { get; set; }
        public string SourceOrganizationUnitName { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
