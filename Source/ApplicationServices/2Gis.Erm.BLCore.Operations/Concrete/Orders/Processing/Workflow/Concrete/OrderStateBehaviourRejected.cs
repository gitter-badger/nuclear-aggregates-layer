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
            // do nothing
            // данное состояние не требует какой-то дополнительной обработки, 
            // фактически вся его ответсвенность - обеспечить доступность перехода из Rejected->OnRegistration (без exception, которые были бы без переопределения этого метода)
            // COMMENT {all, 22.07.2014}: подумать при рефакторинге workflow заказа, если паттерн state останется, и нужно просто обеспечить достижимость перехода, может быть можно запилить нечто вроде VerifyTransitionState, который  параметризовать разными состояниями, переходы в которые допустимы
        }
    }
}
