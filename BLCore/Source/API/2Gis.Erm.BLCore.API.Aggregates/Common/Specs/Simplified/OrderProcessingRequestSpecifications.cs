using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs.Simplified
{
    public static class OrderProcessingRequestSpecifications
    {
        public static class Find
        {
            public static FindSpecification<OrderProcessingRequest> ForProlongateAndOpened()
            {
                return new FindSpecification<OrderProcessingRequest>(x =>
                                                                     x.State == OrderProcessingRequestState.Opened &&
                                                                     x.RequestType == OrderProcessingRequestType.ProlongateOrder);
            }
        }
    }
}