using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.OrderProcessingRequest
{
    // 2+ \BL\Source\ApplicationServices\2Gis.Erm.BLCore.Aggregates\Common\Specs\Simplified
    public static class OrderProcessingRequestMessageSpecifications
    {
        public static class Find
        {
            public static FindSpecification<OrderProcessingRequestMessage> ByRequestId(long orderProcessingRequestId)
            {
                return new FindSpecification<OrderProcessingRequestMessage>(x => x.OrderRequestId == orderProcessingRequestId);
            }

            public static FindSpecification<OrderProcessingRequestMessage> ActiveMessages()
            {
                return new FindSpecification<OrderProcessingRequestMessage>(x => x.IsActive);
            }
        }
    }
}
