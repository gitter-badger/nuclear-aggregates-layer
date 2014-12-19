using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    [DataContract]
    public sealed class ActualizeOrderAmountToWithdrawIdentity : OperationIdentityBase<ActualizeOrderAmountToWithdrawIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ActualizeOrderAmountToWithdrawIdentity;
            }
        }
        public override string Description
        {
            get
            {
                return "Актуализировать сумму к списанию по заказу";
            }
        }
    }
}