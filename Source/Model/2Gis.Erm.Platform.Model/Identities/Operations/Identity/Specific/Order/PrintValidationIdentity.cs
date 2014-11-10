namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class PrintValidationIdentity : OperationIdentityBase<PrintValidationIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.PrintValidationIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Валидация полей заказа перед печатью";
            }
        }
    }
}