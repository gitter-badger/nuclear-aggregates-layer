using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class WorkflowProcessingIdentity : OperationIdentityBase<WorkflowProcessingIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.WorkflowProcessingIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Перемещение заказа по потоку исполнения";
            }
        }
    }
}
