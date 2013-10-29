namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Bargain
{
    public sealed class BindToOrderIdentity : OperationIdentityBase<BindToOrderIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.BindBargainToOrderIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Прикрепить к заказу";
            }
        }
    }
}