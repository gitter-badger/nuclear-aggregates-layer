namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderValidation
{
    public class ResetOrderValidationResultsIdentity : OperationIdentityBase<ResetOrderValidationResultsIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ResetOrderValidationResultsIdentity; }
        }

        public override string Description
        {
            get { return "Reset orderValidation results"; }
        }
    }
}