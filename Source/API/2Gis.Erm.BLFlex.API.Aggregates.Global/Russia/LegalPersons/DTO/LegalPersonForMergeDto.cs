using System.Collections.Generic;

using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.API.Aggregates.Global.Russia.LegalPersons.DTO
{
    public class LegalPersonForMergeDto
    {
        public LegalPerson LegalPerson { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
        public IEnumerable<AccountDetail> AccountDetails { get; set; }
        public IEnumerable<Order> Orders { get; set; }
        public IEnumerable<Bargain> Bargains { get; set; }
        public IEnumerable<LegalPersonProfile> Profiles { get; set; }
    }
}