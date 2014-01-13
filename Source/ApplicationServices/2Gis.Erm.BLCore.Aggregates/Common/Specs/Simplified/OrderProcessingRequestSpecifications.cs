﻿using DoubleGis.Erm.Model.Entities.Enums;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.Common.Specs.Simplified
{
    public static class OrderProcessingRequestSpecifications
    {
        public static class Find
        {
            public static FindSpecification<OrderProcessingRequest> ById(long orderProcessingRequestId)
            {
                return new FindSpecification<OrderProcessingRequest>(x => x.Id == orderProcessingRequestId);
            }


            public static FindSpecification<OrderProcessingRequest> ForProlongateAndOpened()
            {
                return new FindSpecification<OrderProcessingRequest>(x =>
                    x.IsActive && !x.IsDeleted
                    && x.State == (int)OrderProcessingRequestState.Opened
                    && x.RequestType == (int)OrderProcessingRequestType.ProlongateOrder);
            }
        }
    }
}
