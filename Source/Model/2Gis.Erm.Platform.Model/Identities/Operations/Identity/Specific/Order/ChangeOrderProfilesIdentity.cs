namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class ChangeOrderProfilesIdentity : OperationIdentityBase<ChangeOrderProfilesIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ChangeOrderProfilesIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Смена профилей в заказе";
            }
        }
    }
}