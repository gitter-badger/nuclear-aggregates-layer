using System;

using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListLimitDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public long BranchOfficeId { get; set; }
        public string BranchOfficeName { get; set; }
        public string LegalPersonName { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? CloseDate { get; set; }
        public DateTime StartPeriodDate { get; set; }
        public DateTime EndPeriodDate { get; set; }
        public decimal Amount { get; set; }
        public LimitStatus StatusEnum { get; set; }
        public string Status { get; set; }
        public string ClientName { get; set; }
        public long OwnerCode { get; set; }
        public string OwnerName { get; set; }
        public long InspectorCode { get; set; }
        public string InspectorName { get; set; }
        public long AccountId { get; set; }
        public long LegalPersonId { get; set; }
        public long? ClientId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}