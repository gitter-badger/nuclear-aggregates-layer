using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

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