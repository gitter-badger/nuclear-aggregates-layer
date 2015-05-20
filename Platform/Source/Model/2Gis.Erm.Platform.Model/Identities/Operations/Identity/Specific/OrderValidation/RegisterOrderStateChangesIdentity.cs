using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderValidation
{
    public class RegisterOrderStateChangesIdentity : OperationIdentityBase<RegisterOrderStateChangesIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.RegisterOrderStateChangesIdentity; }
        }

        public override string Description
        {
            get { return "Означает, что состояние заказа было каким-то образом изменено вы результате какой-то внешней операции, прямо или косвенно. Например, изменили рекламный материал для позиции заказа и т.п."; }
        }
    }
}