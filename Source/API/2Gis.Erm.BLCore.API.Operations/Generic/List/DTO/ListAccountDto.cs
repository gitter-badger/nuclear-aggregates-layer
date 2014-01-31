using System;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO
{
    public sealed class ListAccountDto : IListItemEntityDto<Account>
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
        public long OwnerCode { get; set; }
        public string OwnerName { get; set; }
    }
}