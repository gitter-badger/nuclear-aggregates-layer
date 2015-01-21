using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Bargain
{
    public sealed class BulkCloseClientBargainsIdentity : OperationIdentityBase<BulkCloseClientBargainsIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.BulkCloseClientBargainsIdentity; }
        }

        public override string Description
        {
            get { return "Закрытие клиентских договоров"; }
        }
    }
}