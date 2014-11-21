using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.WorkflowProcessing;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing.Workflow.Concrete
{
    internal sealed class OrderStateBehaviourOnApproval : OrderStateBehaviour
    {
        public OrderStateBehaviourOnApproval(IUseCaseResumeContext<EditOrderRequest> resumeContext, Order order)
            : base(resumeContext, order)
        {
        }

        protected override void ChangeToOnRegistration()
        {
            ResumeContext.UseCaseResume(new SetADPositionsValidationAsInvalidRequest { OrderId = Order.Id });
        }

        protected override void ChangeToApproved()
        {
            ResumeContext.UseCaseResume(new SetADPositionsValidationAsInvalidRequest { OrderId = Order.Id });
            ResumeContext.UseCaseResume(new ProcessOrderOnApprovalToApprovedRequest { Order = Order });
        }

        protected override void ChangeToRejected()
        {
            ResumeContext.UseCaseResume(new SetADPositionsValidationAsInvalidRequest { OrderId = Order.Id });
            ResumeContext.UseCaseResume(new ProcessOrderOnApprovalToRejectedRequest { Order = Order });
        }
    }
}