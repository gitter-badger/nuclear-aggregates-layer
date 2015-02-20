namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class CheckIfOrderPositionCanBeCreatedForOrderIdentity : OperationIdentityBase<CheckIfOrderPositionCanBeCreatedForOrderIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.CheckIfOrderPositionCanBeCreatedForOrderIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "CheckIfOrderPositionCanBeCreatedForOrder";
            }
        }
    }
}