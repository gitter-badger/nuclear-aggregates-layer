using System;

using DoubleGis.Erm.Platform.API.Core.Operations;

namespace DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO
{
    public sealed class ListAccountDto : IOperationSpecificEntityDto
    {
        public long Id { get; set; }
        public long CurrencyId { get; set; }
        public long LegalPersonId { get; set; }
        public long? ClientId { get; set; }
        public long BranchOfficeOrganizationUnitId { get; set; }
        public string BranchOfficeOrganizationUnitName { get; set; }
        public string LegalPersonName { get; set; }
        public string ClientName { get; set; }
        public string Inn { get; set; }
        public string CurrencyName { get; set; }
        public long OrganizationUnitId { get; set; }
        public string OrganizationUnitName { get; set; }
        public decimal AccountDetailBalance { get; set; }
        public DateTime CreateDate { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public long OwnerCode { get; set; }
        public decimal Balance { get; set; }
        public string OwnerName { get; set; }
    }
}