using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.ReadModel
{
    public static partial class AccountSpecs
    {
        public static class Withdrawals
        {
            public static class Find
            {
                public static FindSpecification<WithdrawalInfo> ByOrganization(long organizationUnitId)
                {
                    return new FindSpecification<WithdrawalInfo>(x => x.OrganizationUnitId == organizationUnitId);
                }

                public static FindSpecification<WithdrawalInfo> ForPeriod(TimePeriod period)
                {
                    return new FindSpecification<WithdrawalInfo>(x => x.PeriodStartDate == period.Start && x.PeriodEndDate == period.End);
                }
            }

            public static class Select
            {

            }
        }
    }
}
