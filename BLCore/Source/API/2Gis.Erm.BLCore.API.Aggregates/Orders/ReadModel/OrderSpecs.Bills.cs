using System;
using System.Linq;

using NuClear.Storage.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel
{
    public static partial class OrderSpecs
    {
        public static class Bills
        {
            public static class Find
            {
                public static FindSpecification<Bill> ByOrder(long orderId)
                {
                    return new FindSpecification<Bill>(x => x.OrderId == orderId);
                }
            }

            public static class Select
            {
            }
        }
    }
}