using System.Runtime.Serialization;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal
{
    [DataContract]
    public sealed class BulkWithdrawIdentity : OperationIdentityBase<BulkWithdrawIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.BulkWithdrawIdentity; }
        }

        public override string Description
        {
            get { return "BulkWithdraw"; }
        }
    }
}
