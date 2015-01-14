using System;

using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel
{
    public static partial class AccountSpecs
    {
        public static class Locks
        {
            public static class Find
            {
                public static FindSpecification<Lock> BySourceOrganizationUnit(long organizationUnitId, TimePeriod period)
                {
                    return new FindSpecification<Lock>(x => x.PeriodStartDate == period.Start &&
                                                            x.PeriodEndDate == period.End &&
                                                            x.Order.SourceOrganizationUnitId == organizationUnitId);
                }

                public static FindSpecification<Lock> ForOrder(long orderId)
                {
                    return new FindSpecification<Lock>(x => x.OrderId == orderId);
                }

                public static FindSpecification<Lock> ByAccount(long accountId)
                {
                    return new FindSpecification<Lock>(x => x.AccountId == accountId);
                }

                public static FindSpecification<Lock> ForPreviousPeriods(DateTime periodStart, DateTime periodEnd)
                {
                    return new FindSpecification<Lock>(x => x.PeriodStartDate < periodStart && x.PeriodEndDate < periodEnd);
                }

                public static FindSpecification<Lock> ByDestinationOrganizationUnit(long destinationOrganizationUnitId, TimePeriod period)
                {
                    return new FindSpecification<Lock>(x => x.PeriodStartDate == period.Start &&
                                                            x.PeriodEndDate == period.End &&
                                                            x.Order.DestOrganizationUnitId == destinationOrganizationUnitId);
                }
            }
        }

        public static class LockDetails
        {
            public static class Find
            {
                public static FindSpecification<LockDetail> ForChargeSessionId(Guid chargeSessionId)
                {
                    return new FindSpecification<LockDetail>(x => x.ChargeSessionId == chargeSessionId);
                }
            }
        }
    }
}