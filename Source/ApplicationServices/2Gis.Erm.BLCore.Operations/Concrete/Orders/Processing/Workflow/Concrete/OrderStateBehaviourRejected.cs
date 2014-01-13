using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.WorkflowProcessing;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing.Workflow.Concrete
{
    internal sealed class OrderStateBehaviourRejected : OrderStateBehaviour
    {
        public OrderStateBehaviourRejected(IUseCaseResumeContext<EditOrderRequest> resumeContext, Order order)
            : base(resumeContext, order)
        {
        }

        protected override void ChangeToOnRegistration()
        {
            ResumeContext.UseCaseResume(new ProcessOrderOnRejectedToOnRegistrationRequest { Order = Order });
        }
    }
}
