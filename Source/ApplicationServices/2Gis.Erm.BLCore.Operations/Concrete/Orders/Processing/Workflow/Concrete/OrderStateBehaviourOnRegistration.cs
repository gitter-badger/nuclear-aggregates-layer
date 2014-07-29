using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing.Workflow.Concrete
{
    internal sealed class OrderStateBehaviourOnRegistration : OrderStateBehaviour
    {
        public OrderStateBehaviourOnRegistration(IUseCaseResumeContext<EditOrderRequest> resumeContext, Order order)
            : base(resumeContext, order)
        {
        }

        protected override void ChangeToOnApproval()
        {
            ResumeContext.UseCaseResume(new SetADPositionsValidationAsInvalidRequest { OrderId = Order.Id });
        }
    }
}
