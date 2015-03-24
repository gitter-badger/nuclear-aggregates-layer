using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing.Workflow.Concrete;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing.Workflow
{
    internal sealed class OrderStateBehaviourFactory
    {
        private readonly IUseCaseResumeContext<EditOrderRequest> _resumeContext;

        public OrderStateBehaviourFactory(IUseCaseResumeContext<EditOrderRequest> resumeContext)
        {
            _resumeContext = resumeContext;
        }

        public static IEnumerable<OrderState> GetTransitionsForUI(OrderState initialState)
        {
            switch (initialState)
            {
                case OrderState.OnRegistration:
                    return new[] { OrderState.OnApproval };
                case OrderState.OnApproval:
                    return new[] { OrderState.OnRegistration, OrderState.Rejected, OrderState.Approved };
                case OrderState.Approved:
                    return new[] { OrderState.OnRegistration, OrderState.OnTermination };
                case OrderState.OnTermination:
                    return new[] { OrderState.Approved };
                case OrderState.Rejected:
                    return new[] { OrderState.OnRegistration };
                case OrderState.Archive:
                    return new OrderState[] { };
                default:
                    throw new ArgumentException();                    
            }
        }

        public OrderStateBehaviour GetOrderStateBehaviour(OrderState originalOrderState, Order order)
        {
            switch (originalOrderState)
            {
                case OrderState.OnRegistration:
                    return new OrderStateBehaviourOnRegistration(_resumeContext, order);
                case OrderState.OnApproval:
                    return new OrderStateBehaviourOnApproval(_resumeContext, order);
                case OrderState.Approved:
                    return new OrderStateBehaviourApproved(_resumeContext, order);
                case OrderState.OnTermination:
                    return new OrderStateBehaviourOnTermination(_resumeContext, order);
                case OrderState.Rejected:
                    return new OrderStateBehaviourRejected(_resumeContext, order);
                case OrderState.Archive:
                    return new OrderStateBehaviourArchive(_resumeContext, order);
                default:
                    throw new ArgumentException();
            }
        }
    }
}
