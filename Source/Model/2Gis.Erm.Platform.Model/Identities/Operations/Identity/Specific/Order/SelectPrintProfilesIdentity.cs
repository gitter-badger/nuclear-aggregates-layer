namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order
{
    public sealed class SelectPrintProfilesIdentity : OperationIdentityBase<SelectPrintProfilesIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get
            {
                return OperationIdentityIds.SelectPrintProfilesIdentity;
            }
        }

        public override string Description
        {
            get
            {
                return "Выбор профиля при печати";
            }
        }
    }
}