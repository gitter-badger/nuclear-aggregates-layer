namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderPosition
{
    public sealed class ReplaceOrderPositionAdvertisementLinksIdentity : OperationIdentityBase<ReplaceOrderPositionAdvertisementLinksIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ReplaceOrderPositionAdvertisementLinksIdentity; }
        }

        public override string Description
        {
            get { return "Замена связей позиции заказа с рекламой"; }
        }
    }
}