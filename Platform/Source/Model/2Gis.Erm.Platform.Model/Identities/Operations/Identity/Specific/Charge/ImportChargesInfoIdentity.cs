using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Charge
{
    public class ImportChargesInfoIdentity : OperationIdentityBase<ImportChargesInfoIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportChargesInfoIdentity; }
        }

        public override string Description
        {
            get { return "Импорт потока сообщения ChargesInfo"; }
        }
    }
}