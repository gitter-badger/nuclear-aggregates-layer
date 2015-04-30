using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel
{
    public static partial class OrderSpecs
    {
        public static class OrderPositionAdvertisements
        {
            public static class Find
            {
                public static FindSpecification<OrderPositionAdvertisement> ByOrderPosition(long orderPositionId)
                {
                    return new FindSpecification<OrderPositionAdvertisement>(x => x.OrderPositionId == orderPositionId);
                }
            }
        }
    }
}