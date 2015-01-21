using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Client
{
    public class CalculateClientPromisingIdentity : OperationIdentityBase<CalculateClientPromisingIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.CalculateClientPromisingIdentity; }
        }

        public override string Description
        {
            get { return "Операция пересчета перспективности клиентов"; }
        }
    }
}