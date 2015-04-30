using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Simplified
{
    public static class OrderProcessingRequestMessageSpecifications
    {
        public static class Find
        {
            public static FindSpecification<OrderProcessingRequestMessage> ByRequestId(long orderProcessingRequestId)
            {
                return new FindSpecification<OrderProcessingRequestMessage>(x => x.OrderRequestId == orderProcessingRequestId);
            }
        }
    }
}