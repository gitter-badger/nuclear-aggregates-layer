using System;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.OrderValidation.AssociatedAndDeniedPositions
{
    public sealed class CheckOrdersAssociatedAndDeniedPositionsRequest : Request
    {
        private CheckOrdersAssociatedAndDeniedPositionsRequest()
        {
        }

        public ADPCheckMode Mode { get; private set; }
        public long OrderId { get; private set; }
        public Expression<Func<Order, bool>> FilterExpression { get; private set; }

        public static CheckOrdersAssociatedAndDeniedPositionsRequest CreateRequest(long orderId)
        {
            return new CheckOrdersAssociatedAndDeniedPositionsRequest { Mode = ADPCheckMode.SpecificOrder, OrderId = orderId };
        }

        public static CheckOrdersAssociatedAndDeniedPositionsRequest CreateRequestForOrderBeingCancelled(long orderId)
        {
            return new CheckOrdersAssociatedAndDeniedPositionsRequest { Mode = ADPCheckMode.OrderBeingCancelled, OrderId = orderId };
        }

        public static CheckOrdersAssociatedAndDeniedPositionsRequest CreateRequestForMassiveCheck(Expression<Func<Order, bool>> filterExpression)
        {
            return new CheckOrdersAssociatedAndDeniedPositionsRequest { Mode = ADPCheckMode.Massive, FilterExpression = filterExpression };
        }

        public static CheckOrdersAssociatedAndDeniedPositionsRequest CreateRequestForOrderbeingReapproved(long orderId)
        {
            return new CheckOrdersAssociatedAndDeniedPositionsRequest { Mode = ADPCheckMode.OrderBeingReapproved, OrderId = orderId };
        }
    }
}