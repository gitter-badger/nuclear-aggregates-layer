using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Accounts.ReadModel
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

                public static FindSpecification<Lock> ByDestinationOrganizationUnit(long destinationOrganizationUnitId, TimePeriod period)
                {
                    return new FindSpecification<Lock>(x => x.PeriodStartDate == period.Start &&
                                                            x.PeriodEndDate == period.End &&
                                                            x.Order.DestOrganizationUnitId == destinationOrganizationUnitId);
                }
            }
        }
    }
}
