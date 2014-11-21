using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO
{
    public class AccountInfoForImportFrom1C
    {
        public long Id { get; set; }
        public IEnumerable<AccountDetail> AccountDetails { get; set; }
        public string LegalPersonName { get; set; }
        public string BranchOfficeLegalName { get; set; }
        public string BranchOfficeSyncCode1C { get; set; }
        public long OwnerCode { get; set; }
    }
}