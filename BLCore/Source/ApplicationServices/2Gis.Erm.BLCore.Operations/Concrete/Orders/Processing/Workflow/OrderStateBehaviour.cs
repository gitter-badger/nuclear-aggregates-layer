using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Orders.Processing.Workflow
{
    public abstract class OrderStateBehaviour
    {
        private readonly IUseCaseResumeContext<EditOrderRequest> _resumeContext;
        protected Order Order { get; private set; }
        protected IUseCaseResumeContext<EditOrderRequest> ResumeContext
        {
            get
            {
                return _resumeContext;
            }
        }

        protected OrderStateBehaviour(IUseCaseResumeContext<EditOrderRequest> resumeContext, Order order)
        {
            _resumeContext = resumeContext;
            Order = order;
        }

        public void ChangeStateTo(OrderState desiredState)
        {
            switch (desiredState)
            {
                case OrderState.OnRegistration:
                    ChangeToOnRegistration();
                    break;
                case OrderState.OnApproval:
                    ChangeToOnApproval();
                    break;
                case OrderState.Approved:
                    ChangeToApproved();
                    break;
                case OrderState.OnTermination:
                    ChangeToOnTermination();
                    break;
                case OrderState.Rejected:
                    ChangeToRejected();
                    break;
                case OrderState.Archive:
                    ChangeToArchive();
                    break;
            }
        }

        protected virtual void ChangeToOnRegistration()
        {
            throw new NotSupportedException();
        }

        protected virtual void ChangeToOnApproval()
        {
            throw new NotSupportedException();
        }

        protected virtual void ChangeToApproved()
        {
            throw new NotSupportedException();
        }

        protected virtual void ChangeToOnTermination()
        {
            throw new NotSupportedException();
        }

        protected virtual void ChangeToRejected()
        {
            throw new NotSupportedException();
        }

        protected virtual void ChangeToArchive()
        {
            throw new NotSupportedException();
        }
    }
}
