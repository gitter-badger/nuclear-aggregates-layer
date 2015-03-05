using System;
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
        public static class Locks
        {
            public static class Find
            {
                public static FindSpecification<Lock> BySourceOrganizationUnit(long organizationUnitId)
                {
                    return new FindSpecification<Lock>(x => x.Order.SourceOrganizationUnitId == organizationUnitId);
                }

                public static FindSpecification<Lock> BySourceOrganizationUnits(IEnumerable<long> organizationUnitIds)
                {
                    return new FindSpecification<Lock>(x => organizationUnitIds.Contains(x.Order.SourceOrganizationUnitId));
                }

                public static FindSpecification<Lock> ForOrder(long orderId)
                {
                    return new FindSpecification<Lock>(x => x.OrderId == orderId);
                }

                public static FindSpecification<Lock> ByAccount(long accountId)
                {
                    return new FindSpecification<Lock>(x => x.AccountId == accountId);
                }

                public static FindSpecification<Lock> ByAccountingMethod(AccountingMethod accountingMethod)
                {
                    var salesModels = accountingMethod.ToSalesModels();
                    return new FindSpecification<Lock>(x => x.Order.OrderPositions.Any(y => salesModels.Contains(y.PricePosition.Position.SalesModel)));
                }

                public static FindSpecification<Lock> ForPreviousPeriods(DateTime periodStart, DateTime periodEnd)
                {
                    return new FindSpecification<Lock>(x => x.PeriodStartDate < periodStart && x.PeriodEndDate < periodEnd);
                }

                public static FindSpecification<Lock> ForPeriod(DateTime periodStart, DateTime periodEnd)
                {
                    return new FindSpecification<Lock>(x => x.PeriodStartDate == periodStart && x.PeriodEndDate == periodEnd);
                }

                // TODO {all, 04.02.2015}: Разделить
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