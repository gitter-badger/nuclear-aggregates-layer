using NuClear.Model.Common.Operations.Identity;

namespace DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.HotClientRequest
{
    public class ImportHotClientIdentity : OperationIdentityBase<ImportHotClientIdentity>, INonCoupledOperationIdentity
    {
        public override int Id
        {
            get { return OperationIdentityIds.ImportHotClientIdentity; }
        }

        public override string Description
        {
            get { return "Импорт сообщения flowStickers.HotClient"; }
        }
    }
}