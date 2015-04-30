using System.Collections.Generic;
using System.Linq;

using NuClear.Storage.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel
{
    public static partial class AccountSpecs
    {
        public static class Accounts
        {
            public static class Find
            {
                public static FindSpecification<Account> ByLegalPerson(long legalPersonId)
                {
                    return new FindSpecification<Account>(x => x.LegalPersonId == legalPersonId);
                }

                public static FindSpecification<Account> ByLegalPersonSyncCode1C(string legalPersonSyncCode1C)
                {
                    return new FindSpecification<Account>(x => x.LegalPesonSyncCode1C == legalPersonSyncCode1C);
                }

                public static FindSpecification<Account> ByLegalPersonSyncCodes1C(IEnumerable<string> legalPersonSyncCodes1C)
                {
                    return new FindSpecification<Account>(x => legalPersonSyncCodes1C.Contains(x.LegalPesonSyncCode1C));
                }

                public static FindSpecification<Account> Existing(long legalPersonId, long branchOfficeOrganizationUnitId)
                {
                    return new FindSpecification<Account>(x => !x.IsDeleted &&
                                                               x.LegalPersonId == legalPersonId &&
                                                               x.BranchOfficeOrganizationUnitId == branchOfficeOrganizationUnitId);
                }

                public static FindSpecification<Account> ForLegalPersons(long legalPersonId, long branchOfficeOrganizationUnitId)
                {
                    return new FindSpecification<Account>(x => x.LegalPersonId == legalPersonId && x.BranchOfficeOrganizationUnitId == branchOfficeOrganizationUnitId);
                }
            }
        }
    }
}