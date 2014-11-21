using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.WorkflowProcessing;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing.Workflow.Concrete
{
    internal sealed class OrderStateBehaviourOnTermination : OrderStateBehaviour
    {
        public OrderStateBehaviourOnTermination(IUseCaseResumeContext<EditOrderRequest> resumeContext, Order order)
            : base(resumeContext, order)
        {
        }

        protected override void ChangeToApproved()
        {
            ResumeContext.UseCaseResume(new ProcessOrderOnTerminationToOnApprovedRequest { Order = Order });
        }
    }
}
