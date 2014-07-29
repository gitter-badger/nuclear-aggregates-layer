using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.WorkflowProcessing;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing.Workflow.Concrete
{
    internal sealed class OrderStateBehaviourArchive : OrderStateBehaviour
    {
        public OrderStateBehaviourArchive(IUseCaseResumeContext<EditOrderRequest> resumeContext, Order order)
            : base(resumeContext, order)
        {
        }

        protected override void ChangeToApproved()
        {
            ResumeContext.UseCaseResume(new ProcessOrderOnArchiveToApprovedRequest { Order = Order });
        }
    }
}
