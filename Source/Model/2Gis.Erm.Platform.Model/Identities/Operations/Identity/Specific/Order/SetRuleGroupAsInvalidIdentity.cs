namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class SetRuleGroupAsInvalidIdentity : OperationIdentityBase<SetRuleGroupAsInvalidIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.ResetValidationGroupIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Отметка результатов проверок заказов в группе как уже невалидных";
            }
        }
    }
}
