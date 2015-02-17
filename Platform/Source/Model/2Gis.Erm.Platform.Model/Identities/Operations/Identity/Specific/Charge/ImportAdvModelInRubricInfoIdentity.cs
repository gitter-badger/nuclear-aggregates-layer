namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Charge
{
    public class ImportAdvModelInRubricInfoIdentity : OperationIdentityBase<ImportAdvModelInRubricInfoIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportAdvModelInRubricInfoIdentity; }
        }

        public override string Description
        {
            get { return "Импорт потока сообщения AdvModelInRubricInfo"; }
        }
    }
}