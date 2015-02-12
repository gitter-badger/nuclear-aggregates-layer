namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.LegalPerson
{
    public sealed class ValidateLegalPersonsForExportIdentity : OperationIdentityBase<ValidateLegalPersonsForExportIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ValidateLegalPersonsForExportIdentity; }
        }

        public override string Description
        {
            get { return "Проверка юр. лиц перед экспортом"; }
        }
    }
}