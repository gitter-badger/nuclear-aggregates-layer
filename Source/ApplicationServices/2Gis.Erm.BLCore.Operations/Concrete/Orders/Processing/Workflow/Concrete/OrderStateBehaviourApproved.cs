using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.WorkflowProcessing;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing.Workflow.Concrete
{
    internal sealed class OrderStateBehaviourApproved : OrderStateBehaviour
    {
        public OrderStateBehaviourApproved(IUseCaseResumeContext<EditOrderRequest> resumeContext, Order order)
            : base(resumeContext, order)
        {
        }

        protected override void ChangeToOnRegistration()
        {
            ResumeContext.UseCaseResume(new SetADPositionsValidationAsInvalidRequest { OrderId = Order.Id });
            ResumeContext.UseCaseResume(new ProcessOrderOnApprovedToOnRegistrationRequest { Order = Order });
        }

        protected override void ChangeToOnTermination()
        {
            ResumeContext.UseCaseResume(new SetADPositionsValidationAsInvalidRequest { OrderId = Order.Id });
            ResumeContext.UseCaseResume(new ProcessOrderOnApprovedToOnTerminationRequest { Order = Order });
        }
    }
}
