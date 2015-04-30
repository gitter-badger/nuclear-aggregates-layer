using System;

using NuClear.Storage.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel
{
    public static partial class OrderSpecs
    {
        public static class OrderReleaseTotals
        {
            public static class Find
            {
                public static FindSpecification<OrderReleaseTotal> ByPeriod(DateTime periodStart, DateTime periodEnd)
                {
                    return new FindSpecification<OrderReleaseTotal>(x => x.ReleaseBeginDate == periodStart && x.ReleaseEndDate == periodEnd);
                }
            }

            public static class Select
            {
            }
        }
    }
}
