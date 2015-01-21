using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AccountDetail
{
    public class ImportOperationsInfoIdentity : OperationIdentityBase<ImportOperationsInfoIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportOperationsInfoIdentity; }
        }

        public override string Description
        {
            get { return "Импорт сообщения flowFinancialData1C.OperationsInfo"; }
        }
    }
}