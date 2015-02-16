using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel
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

                public static FindSpecification<WithdrawalInfo> ByOrganizations(IEnumerable<long> organizationUnitIds)
                {
                    return new FindSpecification<WithdrawalInfo>(x => organizationUnitIds.Contains(x.OrganizationUnitId));
                }

                public static FindSpecification<WithdrawalInfo> ForPeriod(TimePeriod period)
                {
                    return new FindSpecification<WithdrawalInfo>(x => x.PeriodStartDate == period.Start && x.PeriodEndDate == period.End);
                }

                public static FindSpecification<WithdrawalInfo> Succeed()
                {
                    return InStates(WithdrawalStatus.Success);
                }

                public static FindSpecification<WithdrawalInfo> InStates(params WithdrawalStatus[] states)
                {
                    return new FindSpecification<WithdrawalInfo>(x => states.Contains(x.Status));
                }
            }
        }
    }
}