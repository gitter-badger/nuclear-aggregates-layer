namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class RemoveBargainIdentity : OperationIdentityBase<RemoveBargainIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.RemoveBargainIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "RemoveBargain";
            }
        }
    }
}